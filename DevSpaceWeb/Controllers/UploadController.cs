using Amazon.Runtime.Internal.Util;
using DevSpaceWeb.Data;
using DevSpaceWeb.Fido2;
using DevSpaceWeb.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using SkiaSharp;
using static QRCoder.PayloadGenerator;

namespace DevSpaceWeb.Controllers;

public class UploadController : Controller
{
    public UploadController(
        UserManager<AuthUser> userManager)
    {
        _userManager = userManager;
    }

    private UserManager<AuthUser> _userManager;

    [HttpPost("/upload/avatar")]
    public async Task<IActionResult> UploadAvatar()
    {
        if (Program.IsPreviewMode)
            return BadRequest("Preview mode is enabled.");

        if (!User.Identity.IsAuthenticated)
            return Unauthorized("No logged in");

            if (!Request.Form.Files.Any())
            return BadRequest("No file");

        if (Request.Form.Files.Count != 1)
            return BadRequest("Too many files");

        var ImageFile = Request.Form.Files.First();

        switch (ImageFile.ContentType)
        {
            case "image/png":
            case "image/webp":
            case "image/jpg":
            case "image/jpeg":
                break;
            default:
                return BadRequest("Invalid image type");
        }

        AuthUser? AuthUser = await _userManager.GetUserAsync(User);
        if (AuthUser == null)
            return BadRequest("Invalid user account");

        if (AuthUser.ResourceId == null)
        {
            AuthUser.ResourceId = CheckResourceId(AuthUser);
            await _userManager.UpdateAsync(AuthUser);
        }

        Guid ImageId = Guid.NewGuid();

        using (Stream stream = ImageFile.OpenReadStream())
        {
            using (SKBitmap bitmap = SKBitmap.Decode(stream))
            {
                var surface = SKSurface.Create(new SKImageInfo(bitmap.Width, bitmap.Height));
                var myCanvas = surface.Canvas;
                myCanvas.DrawBitmap(bitmap, 0, 0);
                var Image = surface.Snapshot();
                using (var Webp = Image.Encode(SKEncodedImageFormat.Webp, 100))
                using (var Save = Webp.AsStream())
                using (var Stream = System.IO.File.OpenWrite(
                    Program.Directory.Public.Resources.Path + AuthUser.ResourceId.ToString() + $"/Avatar_{ImageId.ToString()}.webp"
                    ))
                {
                    Save.CopyTo(Stream);
                }
                using (var Png = Image.Encode(SKEncodedImageFormat.Png, 100))
                using (var Save = Png.AsStream())
                using (var Stream = System.IO.File.OpenWrite(
                    Program.Directory.Public.Resources.Path + AuthUser.ResourceId.ToString() + $"/Avatar_{ImageId.ToString()}.png"
                    ))
                {
                    Save.CopyTo(Stream);
                }
            }
        }

        if (AuthUser.AvatarId != null)
        {
            System.IO.File.Delete(Program.Directory.Public.Resources.Path + AuthUser.ResourceId.ToString() + $"/Avatar_{AuthUser.AvatarId.ToString()}.webp");
            System.IO.File.Delete(Program.Directory.Public.Resources.Path + AuthUser.ResourceId.ToString() + $"/Avatar_{AuthUser.AvatarId.ToString()}.png");
        }

        AuthUser.AvatarId = ImageId;
        AuthUser.UpdatePartial();
        await _userManager.UpdateAsync(AuthUser);
        return Ok();
    }

    public Guid CheckResourceId(AuthUser user)
    {
        Guid ResourceId = Guid.NewGuid();

        if (System.IO.Directory.Exists(Program.Directory.Public.Resources.Path + ResourceId.ToString()))
            return CheckResourceId(user);

        System.IO.Directory.CreateDirectory(Program.Directory.Public.Resources.Path + ResourceId.ToString());

        return ResourceId;
    }
}
