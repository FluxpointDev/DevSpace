using DevSpaceWeb.Database;
using Microsoft.AspNetCore.Identity;

namespace DevSpaceWeb.Services;
public interface IAuthService
{
    //    Task<RegisterResult> Register(RegisterModel registerModel);
    //    Task<LoginResult> Login(LoginModel loginModel);
    //    Task Logout();
}
public class AuthService : IAuthService
{
    private readonly SignInManager<AuthUser> _signInManager;

    public AuthService(
                SignInManager<AuthUser> signInManager)
    {
        _signInManager = signInManager;
    }


    //public async Task<LoginResult> Login(LoginModel login)
    //{
    //    var user = await _signInManager.UserManager.FindByEmailAsync(login.Email);

    //    if (user == null)
    //        return new LoginResult { Successful = false, Error = "Username or password was invalid." };

    //    var result = await _signInManager.PasswordSignInAsync(user.UserName, login.Password, login.RememberMe, false);

    //    if (!result.Succeeded)
    //        return new LoginResult { Successful = false, Error = "Username or password was invalid." };

    //    var roles = await _signInManager.UserManager.GetRolesAsync(user);
    //    var claims = new List<Claim>
    //    {
    //        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
    //        new Claim(ClaimTypes.Email, login.Email),
    //        new Claim(ClaimTypes.Name, user.UserName)
    //    };

    //    foreach (var role in roles)
    //        claims.Add(new Claim(ClaimTypes.Role, role));

    //    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("BlahSomeKeyBlahFlibbertyGibbertNonsenseBananarama"));
    //    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    //    var expiry = DateTime.Now.AddDays(Convert.ToInt32(1));

    //    var token = new JwtSecurityToken(
    //        "https://localhost",
    //        "https://localhost",
    //        claims,
    //        expires: expiry,
    //        signingCredentials: creds
    //    );

    //    return new LoginResult { Successful = true, Token = new JwtSecurityTokenHandler().WriteToken(token) };
    //}

    //public Task Logout()
    //{
    //    throw new NotImplementedException();
    //}

    //public async Task<RegisterResult> Register(RegisterModel model)
    //{
    //    var newUser = new AppIdentityUser { UserName = model.Email, Email = model.Email };

    //    var result = await _userManager.CreateAsync(newUser, model.Password);

    //    if (!result.Succeeded)
    //    {
    //        var errors = result.Errors.Select(x => x.Description);

    //        return new RegisterResult { Successful = false, Errors = errors };
    //    }

    //    return new RegisterResult { Successful = true };
    //}
}




//public interface IAuthService
//{
//    Task<RegisterResult> Register(RegisterModel registerModel);
//    Task<LoginResult> Login(LoginModel loginModel);
//    Task Logout();
//}
//public class AuthService : IAuthService
//{
//    private readonly HttpClient _httpClient;
//    private readonly AuthenticationStateProvider _authenticationStateProvider;

//    public AuthService(HttpClient httpClient,
//                       AuthenticationStateProvider authenticationStateProvider)
//    {
//        _httpClient = httpClient;
//        _authenticationStateProvider = authenticationStateProvider;
//    }

//    public async Task<RegisterResult> Register(RegisterModel registerModel)
//    {
//        var result = await _httpClient.PostAsJsonAsync("api/accounts/CreateAccount", registerModel);
//        if (!result.IsSuccessStatusCode)
//        {
//            return new RegisterResult { Successful = false, Errors = new List<string> { "Error occured" } };
//        }
//        return new RegisterResult { Successful = true };
//    }


//    public async Task<LoginResult> Login(LoginModel loginModel)
//    {
//        //var loginAsJson = JsonSerializer.Serialize(loginModel);
//        //var response = await _httpClient.PostAsync("api/Login",
//        //    new StringContent(loginAsJson, Encoding.UTF8, "application/json"));

//        //var loginResult = JsonSerializer.Deserialize<LoginResult>(await response.Content.ReadAsStringAsync(),
//        //new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
//        var response = await _httpClient.PostAsJsonAsync("api/Login", loginModel);
//        var ResponseContent = await response.Content.ReadFromJsonAsync<LoginResult>();

//        if (ResponseContent!.Successful)
//        {
//            ((CustomAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsAuthenticated(ResponseContent.Token!);
//            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", ResponseContent.Token);

//            return ResponseContent;
//        }
//        return ResponseContent;
//    }

//    public async Task Logout()
//    {
//        ((CustomAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsLoggedOut();
//        _httpClient.DefaultRequestHeaders.Authorization = null;
//    }
//}
