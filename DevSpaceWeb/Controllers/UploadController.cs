﻿using DevSpaceWeb.Data;
using DevSpaceWeb.Data.Teams;
using DevSpaceWeb.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
            try
            {
                System.IO.File.Delete(Program.Directory.Public.Resources.Path + AuthUser.ResourceId.ToString() + $"/Avatar_{AuthUser.AvatarId.ToString()}.webp");
                System.IO.File.Delete(Program.Directory.Public.Resources.Path + AuthUser.ResourceId.ToString() + $"/Avatar_{AuthUser.AvatarId.ToString()}.png");
            }
            catch { }
        }

        AuthUser.AvatarId = ImageId;
        AuthUser.UpdatePartial();
        await _userManager.UpdateAsync(AuthUser);
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

        _DB.Teams.Cache.TryGetValue(TeamId, out var Team);
        if (Team == null)
            return BadRequest("Could not find team by id");

        if (!Team.Members.ContainsKey(AuthUser.Id))
            return BadRequest("You do not belong to this team");

        var Member = Team.GetMember(AuthUser);
        if (Member == null)
            return BadRequest("Failed to get member data");

        if (!Team.HasTeamPermission(Member, Data.Permissions.TeamPermission.ManageTeam))
            return BadRequest("You do not have Manage Team permission");

        if (Team.ResourceId == null)
        {
            var GeneratedId = CheckResourceId();
            Team.Update(new UpdateDefinitionBuilder<TeamData>().Set(x => x.ResourceId, GeneratedId));
            Team.ResourceId = GeneratedId;
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
                    Program.Directory.Public.Resources.Path + Team.ResourceId.ToString() + $"/Icon_{ImageId.ToString()}.webp"
                    ))
                {
                    Save.CopyTo(Stream);
                }
                using (var Png = Image.Encode(SKEncodedImageFormat.Png, 100))
                using (var Save = Png.AsStream())
                using (var Stream = System.IO.File.OpenWrite(
                    Program.Directory.Public.Resources.Path + Team.ResourceId.ToString() + $"/Icon_{ImageId.ToString()}.png"
                    ))
                {
                    Save.CopyTo(Stream);
                }
            }
        }

        Team.Update(new UpdateDefinitionBuilder<TeamData>().Set(x => x.IconId, ImageId));

        if (Team.IconId != null)
        {
            try
            {
                System.IO.File.Delete(Program.Directory.Public.Resources.Path + Team.ResourceId.ToString() + $"/Icon_{Team.IconId.ToString()}.webp");
                System.IO.File.Delete(Program.Directory.Public.Resources.Path + Team.ResourceId.ToString() + $"/Icon_{Team.IconId.ToString()}.png");
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
