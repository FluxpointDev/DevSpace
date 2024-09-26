using DevSpaceWeb.Data;
using Fido2NetLib;
using Fido2NetLib.Objects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Text;
using System.Text.Json;

namespace DevSpaceWeb.Fido2;


public class PasskeyAuthController : AuthControllerContext
{
    public PasskeyAuthController(
        UserManager<AuthUser> userManager,
        SignInManager<AuthUser> signInManager,
        Fido2Service fido2Service, IDistributedCache cache) : base(userManager, signInManager, fido2Service)
    {
        Cache = cache;
    }

    private readonly IDistributedCache Cache;

    private static string FormatException(Exception e)
    {
        return string.Format("{0}{1}", e.Message, e.InnerException != null ? " (" + e.InnerException.Message + ")" : "");
    }

    [HttpPost("/auth/passkey/confirm/assertionOptions")]
    public async Task<ActionResult> AssertionOptionsPost()
    {
        string RequestId = Request.Headers["RequestVerificationToken"];
        if (string.IsNullOrEmpty(RequestId))
            return Json(new Fido2Error("Failed to verify request id."));

        try
        {
            var identityUser = await GetCurrentUserAsync();
            if (identityUser == null)
                throw new ArgumentException("User not found");

            string Data = Cache.GetString("passkey-" + RequestId);
            if (string.IsNullOrEmpty(Data))
                return Json(new Fido2Error("Failed user validation."));

            AuthRequest? RequestData = JsonSerializer.Deserialize<AuthRequest?>(Data);
            if (RequestData == null && RequestData.UserId != identityUser.Id)
                return Json(new Fido2Error("Failed user validation."));

            var user = new Fido2User
            {
                DisplayName = identityUser!.UserName,
                Name = identityUser.Email,
                Id = Fido2Service.GetPasskeyIdInBytes(identityUser.Auth.PasskeyId.Value.ToString()) // byte representation of userID is required
            };
            List<PublicKeyCredentialDescriptor> existingCredentials = identityUser.Auth.Passkeys.Where(x => x.Descriptor != null).Select(c => c.Descriptor).ToList();


            var authenticatorSelection = new AuthenticatorSelection
            {
                UserVerification = UserVerificationRequirement.Required
            };

            var exts = new AuthenticationExtensionsClientInputs
            {
                Extensions = true,
                UserVerificationMethod = true,
                DevicePubKey = new AuthenticationExtensionsDevicePublicKeyInputs()
            };

            // 3. Create options

            var options = _fido2Service._lib.GetAssertionOptions(
                existingCredentials,
                authenticatorSelection.UserVerification,
                exts
            );

            // 4. Temporarily store options, session/in-memory cache/redis/db
            HttpContext.Session.SetString("fido2.assertionOptions", options.ToJson());

            // 5. Return options to client
            return Json(options);
        }

        catch (Exception e)
        {
            return Json(new AssertionOptions { Status = "error", ErrorMessage = FormatException(e) });
        }
    }

    [HttpPost("/auth/passkey/confirm/makeAssertion")]
    public async Task<JsonResult> MakeAssertion([FromBody] AuthenticatorAssertionRawResponse clientResponse)
    {
        string RequestId = Request.Headers["RequestVerificationToken"];
        if (string.IsNullOrEmpty(RequestId))
            return Json(new Fido2Error("Failed to verify request id."));

        try
        {
            // 1. Get the assertion options we sent the client
            var jsonOptions = HttpContext.Session.GetString("fido2.assertionOptions");
            var options = AssertionOptions.FromJson(jsonOptions);
            HttpContext.Session.Remove("fido2.assertionOptions");

            var identityUser = await GetCurrentUserAsync();
            if (identityUser == null)
                throw new ArgumentException("User not found");

            string Data = Cache.GetString("passkey-" + RequestId);
            if (string.IsNullOrEmpty(Data))
                return Json(new Fido2Error("Failed user validation."));

            AuthRequest? RequestData = JsonSerializer.Deserialize<AuthRequest?>(Data);

            if (RequestData == null && RequestData.UserId != identityUser.Id)
                return Json(new Fido2Error("Failed user validation."));

            // 2. Get registered credential from database
            var creds = await identityUser.GetPasskeyByIdAsync(clientResponse.Id);
            if (creds == null)
                throw new Exception("Unknown credentials");


            // 4. Create callback to check if userhandle owns the credentialId
            IsUserHandleOwnerOfCredentialIdAsync callback = async (args, cancellationToken) =>
            {
                var storedCreds = await identityUser.GetPasskeysByUserHandleAsync(args.UserHandle);
                return storedCreds.Any(c => c.Descriptor != null && c.Descriptor.Id.SequenceEqual(args.CredentialId));
            };

            if (creds.PublicKey == null)
            {
                throw new InvalidOperationException($"No public key");
            }

            // 5. Make the assertion
            var res = await _fido2Service._lib.MakeAssertionAsync(
                clientResponse, options, creds.PublicKey, null, 0, callback);

            if (res.Status == "ok")
            {
                // 6. Store the updated counter
                if (RequestData.LogRequest)
                {
                    identityUser = await GetCurrentUserAsync();
                    var passkeyUsed = await identityUser.GetPasskeyByIdAsync(res.CredentialId);
                    if (passkeyUsed != null)
                    {
                        passkeyUsed.LastUsedAt = DateTimeOffset.UtcNow;
                        identityUser.Auth.PasskeyLastUsedDevice = passkeyUsed.Name;
                    }
                    else
                        identityUser.Auth.PasskeyLastUsedDevice = "Unknown";
                    identityUser.Auth.PasskeyLastUsedAt = DateTimeOffset.UtcNow;
                    await _userManager.UpdateAsync(identityUser);
                }

                Console.WriteLine("Passkey SUCCESS! - " + RequestId);

                RequestData.IsSuccess = true;
                Cache.SetString("passkey-" + RequestId, JsonSerializer.Serialize(RequestData));
            }
            return Json(res);

        }
        catch (Exception e)
        {
            return Json(new MakeNewCredentialResult("error", FormatException(e), null));
        }
    }
}
