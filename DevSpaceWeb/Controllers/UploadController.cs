using DevSpaceWeb.Data;
using DevSpaceWeb.Data.Teams;
using DevSpaceWeb.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using SkiaSharp;

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

        IFormFile ImageFile = Request.Form.Files.First();

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
            AuthUser.ResourceId = CheckResourceId();
            try
            {
                await _userManager.UpdateAsync(AuthUser);
            }
            catch
            {
                AuthUser.ResourceId = null;
                throw;
            }
        }

        Guid ImageId = Guid.NewGuid();

        using (Stream stream = ImageFile.OpenReadStream())
        {
            using (SKBitmap bitmap = SKBitmap.Decode(stream))
            {
                int Height = bitmap.Height;
                int Width = bitmap.Width;
                if (bitmap.Width > 320)
                    Width = 320;
                if (bitmap.Height > 320)
                    Height = 320;

                SKSurface surface = SKSurface.Create(new SKImageInfo(Width, Height));
                SKCanvas myCanvas = surface.Canvas;

                using (SKBitmap resizeBitmap = bitmap.Resize(new SKImageInfo(Width, Height), SKSamplingOptions.Default))
                    myCanvas.DrawBitmap(resizeBitmap, 0, 0);

                SKImage Image = surface.Snapshot();
                using (SKData Webp = Image.Encode(SKEncodedImageFormat.Webp, 100))
                using (Stream Save = Webp.AsStream())
                using (FileStream Stream = System.IO.File.OpenWrite(
                    Program.Directory.Public.Resources.Path + AuthUser.ResourceId.ToString() + $"/Avatar_{ImageId.ToString()}.webp"
                    ))
                {
                    Save.CopyTo(Stream);
                }

                using (SKData Png = Image.Encode(SKEncodedImageFormat.Png, 100))
                using (Stream Save = Png.AsStream())
                using (FileStream Stream = System.IO.File.OpenWrite(
                    Program.Directory.Public.Resources.Path + AuthUser.ResourceId.ToString() + $"/Avatar_{ImageId.ToString()}.png"
                    ))
                {
                    Save.CopyTo(Stream);
                }
            }
        }

        if (AuthUser.AvatarId != null)
        {
            try
            {
                if (System.IO.File.Exists(AuthUser.Avatar.Path("webp")))
                    System.IO.File.Delete(AuthUser.Avatar.Path("webp"));

                if (System.IO.File.Exists(AuthUser.Avatar.Path("png")))
                    System.IO.File.Delete(AuthUser.Avatar.Path("png"));
            }
            catch { }
        }

        AuthUser.AvatarId = ImageId;
        AuthUser.UpdatePartial();
        await _userManager.UpdateAsync(AuthUser);
        _DB.TriggerSessionEvent(AuthUser.Id, SessionEventType.AccountUpdate);
        return Ok();
    }

    [HttpPost("/upload/background")]
    public async Task<IActionResult> UploadBackground()
    {
        if (Program.IsPreviewMode)
            return BadRequest("Preview mode is enabled.");

        if (!User.Identity.IsAuthenticated)
            return Unauthorized("No logged in");

        if (!Request.Form.Files.Any())
            return BadRequest("No file");

        if (Request.Form.Files.Count != 1)
            return BadRequest("Too many files");

        IFormFile ImageFile = Request.Form.Files.First();

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
            AuthUser.ResourceId = CheckResourceId();
            try
            {
                await _userManager.UpdateAsync(AuthUser);
            }
            catch
            {
                AuthUser.ResourceId = null;
                throw;
            }
        }

        Guid ImageId = Guid.NewGuid();

        using (Stream stream = ImageFile.OpenReadStream())
        {
            using (SKBitmap bitmap = SKBitmap.Decode(stream))
            {
                int Height = bitmap.Height;
                int Width = bitmap.Width;
                Console.WriteLine($"{Height} - {Width}");
                if (Width > 1024)
                {
                    var ratio = (double)1024 / Height;
                    Width = (int)(Width * ratio);
                    Height = (int)(Height * ratio);
                }

                SKSurface surface = SKSurface.Create(new SKImageInfo(Width, Height));
                SKCanvas myCanvas = surface.Canvas;

                using (SKBitmap resizeBitmap = bitmap.Resize(new SKImageInfo(Width, Height), SKSamplingOptions.Default))
                    myCanvas.DrawBitmap(resizeBitmap, 0, 0);

                SKImage Image = surface.Snapshot();
                using (SKData Webp = Image.Encode(SKEncodedImageFormat.Webp, 80))
                using (Stream Save = Webp.AsStream())
                using (FileStream Stream = System.IO.File.OpenWrite(
                    Program.Directory.Public.Resources.Path + AuthUser.ResourceId.ToString() + $"/Background_{ImageId.ToString()}.webp"
                    ))
                {
                    Save.CopyTo(Stream);
                }
            }
        }

        if (AuthUser.BackgroundId != null)
        {
            try
            {
                if (System.IO.File.Exists(AuthUser.Background.Path("webp")))
                    System.IO.File.Delete(AuthUser.Background.Path("webp"));
            }
            catch { }
        }

        AuthUser.BackgroundId = ImageId;
        await _userManager.UpdateAsync(AuthUser);
        AuthUser.UpdatePartial();
        _DB.TriggerSessionEvent(AuthUser.Id, SessionEventType.AccountUpdate);
        return Ok();
    }

    [HttpPost("/upload/icon")]
    public async Task<IActionResult> UploadIcon()
    {
        if (Program.IsPreviewMode)
            return BadRequest("Preview mode is enabled.");

        if (!User.Identity.IsAuthenticated)
            return Unauthorized("No logged in");

        if (!Request.Form.Files.Any())
            return BadRequest("No file");

        if (Request.Form.Files.Count != 1)
            return BadRequest("Too many files");

        string TeamId = Request.Query["teamId"];
        if (string.IsNullOrEmpty(TeamId))
            return BadRequest("Team id is missing");

        IFormFile ImageFile = Request.Form.Files.First();

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

        _DB.Teams.Cache.TryGetValue(ObjectId.Parse(TeamId), out TeamData? Team);
        if (Team == null)
            return BadRequest("Could not find team by id");

        if (!Team.Members.ContainsKey(AuthUser.Id))
            return BadRequest("You do not belong to this team");

        TeamMemberData Member = Team.GetMember(AuthUser);
        if (Member == null)
            return BadRequest("Failed to get member data");

        if (!Member.HasTeamPermission(Data.Permissions.TeamPermission.ManageTeam))
            return BadRequest("You do not have Manage Team permission");

        if (Team.ResourceId == null)
        {
            Guid GeneratedId = CheckResourceId();
            await Team.UpdateAsync(new UpdateDefinitionBuilder<TeamData>().Set(x => x.ResourceId, GeneratedId));
            Team.ResourceId = GeneratedId;
        }

        Guid ImageId = Guid.NewGuid();

        using (Stream stream = ImageFile.OpenReadStream())
        {
            using (SKBitmap bitmap = SKBitmap.Decode(stream))
            {
                int Height = bitmap.Height;
                int Width = bitmap.Width;
                if (bitmap.Width > 320)
                    Width = 320;
                if (bitmap.Height > 320)
                    Height = 320;

                SKSurface surface = SKSurface.Create(new SKImageInfo(Width, Height));
                SKCanvas myCanvas = surface.Canvas;

                using (SKBitmap resizeBitmap = bitmap.Resize(new SKImageInfo(Width, Height), SKSamplingOptions.Default))
                    myCanvas.DrawBitmap(resizeBitmap, 0, 0);

                SKImage Image = surface.Snapshot();
                using (SKData Webp = Image.Encode(SKEncodedImageFormat.Webp, 100))
                using (Stream Save = Webp.AsStream())
                using (FileStream Stream = System.IO.File.OpenWrite(
                    Program.Directory.Public.Resources.Path + Team.ResourceId.ToString() + $"/Icon_{ImageId.ToString()}.webp"
                    ))
                {
                    Save.CopyTo(Stream);
                }
                using (SKData Png = Image.Encode(SKEncodedImageFormat.Png, 100))
                using (Stream Save = Png.AsStream())
                using (FileStream Stream = System.IO.File.OpenWrite(
                    Program.Directory.Public.Resources.Path + Team.ResourceId.ToString() + $"/Icon_{ImageId.ToString()}.png"
                    ))
                {
                    Save.CopyTo(Stream);
                }
            }
        }

        await Team.UpdateAsync(new UpdateDefinitionBuilder<TeamData>().Set(x => x.IconId, ImageId));

        if (Team.IconId != null)
        {
            try
            {
                if (System.IO.File.Exists(Team.Icon.Path("webp")))
                    System.IO.File.Delete(Team.Icon.Path("webp"));

                if (System.IO.File.Exists(Team.Icon.Path("png")))
                    System.IO.File.Delete(Team.Icon.Path("png"));
            }
            catch { }
        }

        
        Team.IconId = ImageId;
        return Ok();
    }

    public Guid CheckResourceId()
    {
        Guid ResourceId = Guid.NewGuid();

        if (System.IO.Directory.Exists(Program.Directory.Public.Resources.Path + ResourceId.ToString()))
            return CheckResourceId();

        System.IO.Directory.CreateDirectory(Program.Directory.Public.Resources.Path + ResourceId.ToString());

        return ResourceId;
    }
}
