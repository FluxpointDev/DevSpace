using DevSpaceWeb.Data.Users;
using Fido2NetLib;
using Fido2NetLib.Objects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Text;
using System.Text.Json;

namespace DevSpaceWeb.Fido2;

public class CustomAuthenticatorAttestationRawResponse
{
    public string name { get; set; }
    public AuthenticatorAttestationRawResponse data { get; set; }
}

public class PasskeyRegisterController : AuthControllerContext
{
    public PasskeyRegisterController(
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

    [HttpPost("/auth/passkey/register/makeCredentialOptions")]
    public async Task<JsonResult> MakeCredentialOptions()
    {
        string RequestId = Request.Headers["RequestVerificationToken"];
        if (string.IsNullOrEmpty(RequestId))
            return Json(new Fido2Error("Failed to verify request id."));

        try
        {
            AuthUser? AuthUser = await GetCurrentUserAsync();
            if (AuthUser == null)
                return Json(new Fido2Error("Failed to get user."));

            string Data = Cache.GetString("passkey-" + RequestId);
            if (string.IsNullOrEmpty(Data))
                return Json(new Fido2Error("Failed user validation."));

            AuthRequest? RequestData = JsonSerializer.Deserialize<AuthRequest?>(Data);
            if (RequestData == null && RequestData.UserId != AuthUser.Id)
                return Json(new Fido2Error("Failed user validation."));

            Fido2User user = new Fido2User
            {
                DisplayName = AuthUser.UserName,
                Name = AuthUser.Email,
                Id = Fido2Service.GetPasskeyIdInBytes(AuthUser.Mfa.PasskeyId.Value.ToString()) // byte representation of userID is required
            };


            // 2. Get user existing keys by username
            List<PublicKeyCredentialDescriptor> existingKeys = new List<PublicKeyCredentialDescriptor>();
            foreach (FidoStoredCredential credential in AuthUser.Mfa.Passkeys)
            {
                if (credential.Descriptor != null)
                    existingKeys.Add(credential.Descriptor);
            }

            // 3. Create options
            AuthenticatorSelection authenticatorSelection = new AuthenticatorSelection
            {
                UserVerification = UserVerificationRequirement.Required
            };

            AuthenticationExtensionsClientInputs exts = new AuthenticationExtensionsClientInputs
            {
                Extensions = true,
                UserVerificationMethod = true,
                CredProps = true,
                DevicePubKey = new AuthenticationExtensionsDevicePublicKeyInputs { }
            };
            CredentialCreateOptions options = _fido2Service._lib.RequestNewCredential(user, existingKeys, authenticatorSelection, AttestationConveyancePreference.Direct, exts);


            // 4. Temporarily store options, session/in-memory cache/redis/db
            HttpContext.Session.SetString("fido2.attestationOptions", options.ToJson());


            // 5. return options to client
            return Json(options);
        }
        catch (Exception e)
        {
            Logger.LogMessage(e.ToString(), LogSeverity.Debug);
            return Json(new Fido2Error(FormatException(e)));
        }
    }

    [HttpPost("/auth/passkey/register/makeCredential")]
    public async Task<JsonResult> MakeCredential([FromBody] CustomAuthenticatorAttestationRawResponse attestationResponse)
    {
        string RequestId = Request.Headers["RequestVerificationToken"];
        if (string.IsNullOrEmpty(RequestId))
            return Json(new Fido2Error("Failed to verify request id."));

        try
        {
            AuthUser? user = await GetCurrentUserAsync();
            if (user == null)
                return Json(new Fido2Error("Failed to get user."));

            string Data = Cache.GetString("passkey-" + RequestId);
            if (string.IsNullOrEmpty(Data))
                return Json(new Fido2Error("Failed user validation."));

            AuthRequest? RequestData = JsonSerializer.Deserialize<AuthRequest?>(Data);
            if (RequestData == null && RequestData.UserId != user.Id)
                return Json(new Fido2Error("Failed user validation."));

            // 1. get the options we sent the client
            string? jsonOptions = HttpContext.Session.GetString("fido2.attestationOptions");
            CredentialCreateOptions options = CredentialCreateOptions.FromJson(jsonOptions);
            // 2. Create callback so that lib can verify credential id is unique to this user
            IsCredentialIdUniqueToUserAsyncDelegate callback = async (args, cancellationToken) =>
            {
                FidoStoredCredential? cred = await user.Mfa.GetPasskeyByIdAsync(args.CredentialId);
                if (cred != null)
                    return false;

                return true;
            };

            // 2. Verify and make the credentials
            MakeNewCredentialResult success = await _fido2Service._lib.MakeNewCredentialAsync(attestationResponse.data, options, callback);
            if (success.Result != null)
            {

                if (_fido2Service._metadata != null)
                {
                    MetadataBLOBPayloadEntry? Metadata = await _fido2Service._metadata.GetEntryAsync(success.Result.AaGuid);
                    if (Metadata != null)
                    {
                        Logger.LogMessage("Metadata: " + Metadata.MetadataStatement.Description, LogSeverity.Debug);
                    }
                }
                // 3. Store the credentials in db
                user.Mfa.Passkeys.Add(new FidoStoredCredential
                {
                    Name = attestationResponse.name,
                    Descriptor = new PublicKeyCredentialDescriptor(success.Result.Id),
                    PublicKey = success.Result.PublicKey,
                    UserHandle = success.Result.User.Id,
                    CreatedAt = DateTime.UtcNow,
                    AaGuid = success.Result.AaGuid
                });

                user.Mfa.PasskeyLastRegisteredAt = DateTime.UtcNow;
                user.Mfa.IsTwoFactorEnabled = true;
                await _userManager.UpdateAsync(user);

                RequestData.IsSuccess = true;
                Cache.SetString("passkey-" + RequestId, JsonSerializer.Serialize(RequestData));
            }

            return Json(success);
        }
        catch (Exception e)
        {
            Logger.LogMessage(e.ToString(), LogSeverity.Debug);
            return Json(new Fido2Error(FormatException(e)));
        }
    }
}