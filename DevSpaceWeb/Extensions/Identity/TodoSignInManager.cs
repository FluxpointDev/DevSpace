namespace DevSpaceWeb.Extensions.Identity
{
    //public class MySignInManager : SignInManager<IdentityUser>
    //{
    //    private readonly UserManager<IdentityUser> _userManager;
    //    private readonly ApplicationDbContext _db;
    //    private readonly IHttpContextAccessor _contextAccessor;

    //    public MySignInManager(
    //        UserManager<IdentityUser> userManager,
    //        IHttpContextAccessor contextAccessor,
    //        IUserClaimsPrincipalFactory<IdentityUser> claimsFactory,
    //        IOptions<IdentityOptions> optionsAccessor,
    //        ILogger<SignInManager<IdentityUser>> logger,
    //        ApplicationDbContext dbContext,
    //        IAuthenticationSchemeProvider schemeProvider,
    //        IUserConfirmation<IdentityUser> userConfirmation
    //        )
    //        : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemeProvider, userConfirmation)
    //    {
    //        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
    //        _contextAccessor = contextAccessor ?? throw new ArgumentNullException(nameof(contextAccessor));
    //        _db = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    //    }

    //    public override async Task<SignInResult> PasswordSignInAsync(string userName, string password, bool rememberMe, bool shouldLockout)
    //    {
    //        SignInResult result;
    //        if (userName == "pippo" && password == "pippo")
    //        {
    //            return SignInResult.Success;
    //        }
    //        else
    //        {
    //            return SignInResult.Failed;
    //        }
    //    }
    //}
}
