using DevSpaceWeb.Data;
using DevSpaceWeb.Data.Permissions;
using DevSpaceWeb.Data.Teams;
using DevSpaceWeb.Data.Users;
using DevSpaceWeb.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using SkiaSharp;

namespace DevSpaceWeb.Controllers.Auth;

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

        if (User.Identity == null || !User.Identity.IsAuthenticated)
            return Unauthorized("Not logged in");

        if (!Request.Form.Files.Any())
            return BadRequest("No file");

        if (Request.Form.Files.Count != 1)
            return BadRequest("Too many files");

        IFormFile ImageFile = Request.Form.Files.First();

        if (ImageFile.Length > 5242880)
            return BadRequest("Image size is more than 5 MB");

        Console.WriteLine("Length:" + ImageFile.Length);

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
            IdentityResult UpdateResult = await _userManager.UpdateAsync(AuthUser);
            if (!UpdateResult.Succeeded)
                return BadRequest("Failed to update user.");

            AuthUser.UpdatePartial();

            Directory.CreateDirectory(Program.Directory.Public.Files.Path + AuthUser.ResourceId.ToString());

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
                    Program.Directory.Public.Files.Path + AuthUser.ResourceId.ToString() + $"/Avatar_{ImageId.ToString()}.webp"
                    ))
                {
                    Save.CopyTo(Stream);
                }

                using (SKData Png = Image.Encode(SKEncodedImageFormat.Png, 100))
                using (Stream Save = Png.AsStream())
                using (FileStream Stream = System.IO.File.OpenWrite(
                    Program.Directory.Public.Files.Path + AuthUser.ResourceId.ToString() + $"/Avatar_{ImageId.ToString()}.png"
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
            }
            catch { }
            try
            {
                if (System.IO.File.Exists(AuthUser.Avatar.Path("png")))
                    System.IO.File.Delete(AuthUser.Avatar.Path("png"));
            }
            catch { }
        }

        AuthUser.AvatarId = ImageId;
        IdentityResult UpdateResultAvatar = await _userManager.UpdateAsync(AuthUser);
        if (!UpdateResultAvatar.Succeeded)
            return BadRequest("Failed to update user.");

        AuthUser.UpdatePartial();
        _DB.TriggerSessionEvent(AuthUser.Id, SessionEventType.AccountUpdate);
        return Ok();
    }

    [HttpPost("/upload/background")]
    public async Task<IActionResult> UploadBackground()
    {
        if (Program.IsPreviewMode)
            return BadRequest("Preview mode is enabled.");

        if (User.Identity == null || !User.Identity.IsAuthenticated)
            return Unauthorized("Not logged in");

        if (!Request.Form.Files.Any())
            return BadRequest("No file");

        if (Request.Form.Files.Count != 1)
            return BadRequest("Too many files");

        IFormFile ImageFile = Request.Form.Files.First();

        if (ImageFile.Length > 5242880)
            return BadRequest("Background size is more than 5 MB");

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
            IdentityResult UpdateResult = await _userManager.UpdateAsync(AuthUser);
            if (!UpdateResult.Succeeded)
                return BadRequest("Failed to update user.");

            AuthUser.UpdatePartial();

            Directory.CreateDirectory(Program.Directory.Public.Files.Path + AuthUser.ResourceId.ToString());
        }

        Guid ImageId = Guid.NewGuid();

        using (Stream stream = ImageFile.OpenReadStream())
        {
            using (SKBitmap bitmap = SKBitmap.Decode(stream))
            {
                int Height = bitmap.Height;
                int Width = bitmap.Width;
                if (Width > 1024)
                {
                    double ratio = (double)1024 / Height;
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
                    Program.Directory.Public.Files.Path + AuthUser.ResourceId.ToString() + $"/Background_{ImageId.ToString()}.webp"
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
        IdentityResult UpdateResultBackground = await _userManager.UpdateAsync(AuthUser);
        if (!UpdateResultBackground.Succeeded)
            return BadRequest("Failed to update user.");

        AuthUser.UpdatePartial();
        _DB.TriggerSessionEvent(AuthUser.Id, SessionEventType.AccountUpdate);
        return Ok();
    }

    [HttpPost("/upload/icon")]
    public async Task<IActionResult> UploadIcon()
    {
        if (Program.IsPreviewMode)
            return BadRequest("Preview mode is enabled.");

        if (User.Identity == null || !User.Identity.IsAuthenticated)
            return Unauthorized("Not logged in");

        if (!Request.Form.Files.Any())
            return BadRequest("No file");

        if (Request.Form.Files.Count != 1)
            return BadRequest("Too many files");

        string? TeamId = Request.Query["teamId"];
        if (string.IsNullOrEmpty(TeamId))
            return BadRequest("Team id is missing");

        IFormFile ImageFile = Request.Form.Files.First();

        if (ImageFile.Length > 5242880)
            return BadRequest("Icon size is more than 5 MB");

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
        if (Team == null || !Team.Members.ContainsKey(AuthUser.Id))
            return BadRequest("Could not find team by id");

        TeamMemberData? Member = Team.GetMember(AuthUser);
        if (Member == null)
            return BadRequest("Failed to get member data");

        if (!Member.HasTeamPermission(Team, TeamPermission.ManageTeam))
            return BadRequest("You do not have Manage Team permission");

        if (Team.ResourceId == null)
        {
            Guid GeneratedId = CheckResourceId();
            UpdateResult UpdateResult = await Team.UpdateAsync(new UpdateDefinitionBuilder<TeamData>().Set(x => x.ResourceId, GeneratedId));
            if (!UpdateResult.IsAcknowledged)
                return BadRequest("Failed to update team resource.");

            Team.ResourceId = GeneratedId;

            Directory.CreateDirectory(Program.Directory.Public.Files.Path + GeneratedId.ToString());
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
                    Program.Directory.Public.Files.Path + Team.ResourceId.ToString() + $"/Icon_{ImageId.ToString()}.webp"
                    ))
                {
                    Save.CopyTo(Stream);
                }
                using (SKData Png = Image.Encode(SKEncodedImageFormat.Png, 100))
                using (Stream Save = Png.AsStream())
                using (FileStream Stream = System.IO.File.OpenWrite(
                    Program.Directory.Public.Files.Path + Team.ResourceId.ToString() + $"/Icon_{ImageId.ToString()}.png"
                    ))
                {
                    Save.CopyTo(Stream);
                }
            }
        }

        UpdateResult UpdateResultIcon = await Team.UpdateAsync(new UpdateDefinitionBuilder<TeamData>().Set(x => x.IconId, ImageId));
        if (!UpdateResultIcon.IsAcknowledged)
            return BadRequest("Failed to update team icon.");

        _ = _DB.AuditLogs.CreateAsync(new AuditLog(AuthUser.Id, Team.Id, AuditLogCategoryType.Setting, AuditLogEventType.IconChanged)
                .SetTarget(Team));

        if (Team.IconId != null)
        {
            try
            {
                if (System.IO.File.Exists(Team.Icon.Path("webp")))
                    System.IO.File.Delete(Team.Icon.Path("webp"));
            }
            catch { }

            try
            {
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

        if (Directory.Exists(Program.Directory.Public.Files.Path + ResourceId.ToString()))
            return CheckResourceId();

        return ResourceId;
    }
}
