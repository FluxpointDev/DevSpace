using DevSpaceWeb.Data.Users;
using Fido2NetLib;
using Fido2NetLib.Objects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using System.Text;

namespace DevSpaceWeb.Fido2;


public class PasskeyAuthController : AuthControllerContext
{
    public PasskeyAuthController(
        UserManager<AuthUser> userManager,
        SignInManager<AuthUser> signInManager,
        Fido2Service fido2Service, IMemoryCache cache) : base(userManager, signInManager, fido2Service)
    {
        Cache = cache;
    }

    private readonly IMemoryCache Cache;

    private static string FormatException(Exception e)
    {
        return string.Format("{0}{1}", e.Message, e.InnerException != null ? " (" + e.InnerException.Message + ")" : "");
    }

    [HttpPost("/auth/passkey/confirm/assertionOptions")]
    public async Task<ActionResult> AssertionOptionsPost()
    {
        string RequestId = Request.Headers["RequestId"];
        if (string.IsNullOrEmpty(RequestId))
            return Json(new Fido2Error("Failed to verify request id."));

        try
        {
            AuthUser? identityUser = await GetCurrentUserAsync();
            if (identityUser == null)
                throw new ArgumentException("User not found");

            AuthRequest? Data = Cache.Get<AuthRequest>("passkey-" + RequestId);
            if (Data == null || Data.UserId != identityUser.Id)
                return Json(new Fido2Error("Failed user validation."));

            Fido2User user = new Fido2User
            {
                DisplayName = identityUser!.UserName,
                Name = identityUser.Email,
                Id = Fido2Service.GetPasskeyIdInBytes(identityUser.Mfa.PasskeyId.Value.ToString()) // byte representation of userID is required
            };
            List<PublicKeyCredentialDescriptor> existingCredentials = identityUser.Mfa.Passkeys.Where(x => x.Descriptor != null).Select(c => c.Descriptor).ToList();


            AuthenticatorSelection authenticatorSelection = new AuthenticatorSelection
            {
                UserVerification = UserVerificationRequirement.Required
            };

            AuthenticationExtensionsClientInputs exts = new AuthenticationExtensionsClientInputs
            {
                Extensions = true,
                UserVerificationMethod = true,
                DevicePubKey = new AuthenticationExtensionsDevicePublicKeyInputs()
            };

            // 3. Create options

            AssertionOptions options = _fido2Service._lib.GetAssertionOptions(
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
        string RequestId = Request.Headers["RequestId"];
        if (string.IsNullOrEmpty(RequestId))
            return Json(new Fido2Error("Failed to verify request id."));

        try
        {
            // 1. Get the assertion options we sent the client
            string? jsonOptions = HttpContext.Session.GetString("fido2.assertionOptions");
            AssertionOptions options = AssertionOptions.FromJson(jsonOptions);
            HttpContext.Session.Remove("fido2.assertionOptions");

            AuthUser? identityUser = await GetCurrentUserAsync();
            if (identityUser == null)
                throw new ArgumentException("User not found");

            AuthRequest? Data = Cache.Get<AuthRequest>("passkey-" + RequestId);
            if (Data == null || Data.UserId != identityUser.Id)
                return Json(new Fido2Error("Failed user validation."));

            // 2. Get registered credential from database
            FidoStoredCredential? creds = await identityUser.Mfa.GetPasskeyByIdAsync(clientResponse.Id);
            if (creds == null)
                throw new Exception("Unknown credentials");


            // 4. Create callback to check if userhandle owns the credentialId
            IsUserHandleOwnerOfCredentialIdAsync callback = async (args, cancellationToken) =>
            {
                List<FidoStoredCredential> storedCreds = await identityUser.Mfa.GetPasskeysByUserHandleAsync(args.UserHandle);
                return storedCreds.Any(c => c.Descriptor != null && c.Descriptor.Id.SequenceEqual(args.CredentialId));
            };

            if (creds.PublicKey == null)
            {
                throw new InvalidOperationException($"No public key");
            }

            // 5. Make the assertion
            VerifyAssertionResult res = await _fido2Service._lib.MakeAssertionAsync(
                clientResponse, options, creds.PublicKey, null, 0, callback);

            if (res.Status == "ok")
            {
                // 6. Store the updated counter
                if (Data.LogRequest)
                {
                    identityUser = await GetCurrentUserAsync();
                    FidoStoredCredential? passkeyUsed = await identityUser.Mfa.GetPasskeyByIdAsync(res.CredentialId);
                    if (passkeyUsed != null)
                    {
                        passkeyUsed.LastUsedAt = DateTime.UtcNow;
                        identityUser.Mfa.PasskeyLastUsedDevice = passkeyUsed.Name;
                    }
                    else
                        identityUser.Mfa.PasskeyLastUsedDevice = "Unknown";
                    identityUser.Mfa.PasskeyLastUsedAt = DateTime.UtcNow;
                    await _userManager.UpdateAsync(identityUser);
                }

                Logger.LogMessage("Passkey SUCCESS! - " + RequestId, LogSeverity.Debug);

                Data.IsSuccess = true;
                Cache.Set("passkey-" + RequestId, Data, new TimeSpan(0, 5, 0));
            }
            return Json(res);

        }
        catch (Exception e)
        {
            return Json(new MakeNewCredentialResult("error", FormatException(e), null));
        }
    }
}
