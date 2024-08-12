namespace DevSpaceWeb.Fido2;

//public class Fido2UserTwoFactorTokenProvider : IUserTwoFactorTokenProvider<AuthUser>
//{
//    public Task<bool> CanGenerateTwoFactorTokenAsync(UserManager<AuthUser> manager, AuthUser user)
//    {
//        return Task.FromResult(true);
//    }

//    public Task<string> GenerateAsync(string purpose, UserManager<AuthUser> manager, AuthUser user)
//    {
//        return Task.FromResult("fido2");
//    }

//    public Task<bool> ValidateAsync(string purpose, string token, UserManager<AuthUser> manager, AuthUser user)
//    {
//        return Task.FromResult(true);
//    }
//}