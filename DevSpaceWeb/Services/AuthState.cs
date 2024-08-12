namespace DevSpaceWeb.Services;

//public class CustomAuthenticationStateProvider : AuthenticationStateProvider
//{
//    private readonly HttpClient _httpClient;

//    public CustomAuthenticationStateProvider(HttpClient httpClient)
//    {
//        _httpClient = httpClient;
//    }

//    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
//    {
//        var savedToken = await _localStorage.GetItemAsync<string>("authToken");

//        if (string.IsNullOrWhiteSpace(savedToken))
//        {
//            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
//        }

//        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", savedToken);

//        return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(savedToken), "jwt")));
//    }

//    public void MarkUserAsAuthenticated(string token)
//    {
//        var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt"));
//        var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
//        NotifyAuthenticationStateChanged(authState);
//    }

//    public void MarkUserAsLoggedOut()
//    {
//        var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
//        var authState = Task.FromResult(new AuthenticationState(anonymousUser));
//        NotifyAuthenticationStateChanged(authState);
//    }

//    private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
//    {
//        var claims = new List<Claim>();
//        var payload = jwt.Split('.')[1];
//        var jsonBytes = ParseBase64WithoutPadding(payload);
//        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

//        keyValuePairs!.TryGetValue(ClaimTypes.Role, out object roles);

//        if (roles != null)
//        {
//            if (roles.ToString()!.Trim().StartsWith("["))
//            {
//                var parsedRoles = JsonSerializer.Deserialize<string[]>(roles.ToString()!);

//                foreach (var parsedRole in parsedRoles!)
//                {
//                    claims.Add(new Claim(ClaimTypes.Role, parsedRole));
//                }
//            }
//            else
//            {
//                claims.Add(new Claim(ClaimTypes.Role, roles.ToString()!));
//            }

//            keyValuePairs.Remove(ClaimTypes.Role);
//        }

//        claims.AddRange(keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()!)));

//        return claims;
//    }

//    private byte[] ParseBase64WithoutPadding(string base64)
//    {
//        switch (base64.Length % 4)
//        {
//            case 2: base64 += "=="; break;
//            case 3: base64 += "="; break;
//        }
//        return Convert.FromBase64String(base64);
//    }
//}
