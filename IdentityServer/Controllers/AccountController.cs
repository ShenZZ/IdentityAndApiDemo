using Duende.IdentityServer.Services;
using Duende.IdentityServer.Stores;
using Duende.IdentityServer.Events;
using Duende.IdentityServer.Test;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Duende.IdentityServer;

namespace IdentityServer.Controllers;

public class AccountController : Controller
{
    private readonly ILogger<AccountController> _logger;
    private readonly TestUserStore _users;
    private readonly IIdentityServerInteractionService _interaction;
    private readonly IClientStore _clientStore;
    private readonly IEventService _events;
    private readonly IAuthenticationSchemeProvider _schemeProvider;
    private readonly IIdentityProviderStore _identityProviderStore;

    public AccountController(
        ILogger<AccountController> logger,
        IIdentityServerInteractionService interaction,
        IClientStore clientStore,
        IAuthenticationSchemeProvider schemeProvider,
        IIdentityProviderStore identityProviderStore,
        IEventService events,
        TestUserStore users = null)
    {
        _logger = logger;
        // this is where you would plug in your own custom identity management library (e.g. ASP.NET Identity)
        _users = users ?? throw new Exception("Please call 'AddTestUsers(TestUsers.Users)' on the IIdentityServerBuilder in Startup or remove the TestUserStore from the AccountController.");

        _interaction = interaction;
        _clientStore = clientStore;
        _schemeProvider = schemeProvider;
        _identityProviderStore = identityProviderStore;
        _events = events;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Login(string returnUrl = null)
    {
        var model = new LoginModel
        {
            ReturnUrl = returnUrl,
            EnableLocalLogin = true
        };

        //var context = await _interaction.GetAuthorizationContextAsync(returnUrl);
        //if (context?.Client.ClientId != null)
        //{
        //    var client = await _clientStore.FindEnabledClientByIdAsync(context.Client.ClientId);
        //    if (client != null)
        //    {
        //        model.EnableLocalLogin = client.EnableLocalLogin;
        //    }
        //}

        return View(model);
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginModel model)
    {
        // check if we are in the context of an authorization request
        var context = await _interaction.GetAuthorizationContextAsync(model.ReturnUrl);

        if (ModelState.IsValid)
        {
            // validate username/password against in-memory store
            if (_users.ValidateCredentials(model.Username, model.Password))
            {
                var user = _users.FindByUsername(model.Username);
                await _events.RaiseAsync(new UserLoginSuccessEvent(user.Username, user.SubjectId, user.Username, clientId: context?.Client.ClientId));

                // only set explicit expiration here if user chooses "remember me". 
                // otherwise we rely upon expiration configured in cookie middleware.
                AuthenticationProperties props = null;
                if (model.RememberLogin)
                {
                    props = new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.Add(TimeSpan.FromDays(30))
                    };
                };

                // issue authentication cookie with subject ID and username
                var isuser = new IdentityServerUser(user.SubjectId)
                {
                    DisplayName = user.Username
                };

                await HttpContext.SignInAsync(isuser, props);

                if (context != null)
                {
                    //if (context.IsNativeClient())
                    //{
                    //    // The client is native, so this change in how to
                    //    // return the response is for better UX for the end user.
                    //    return this.LoadingPage(model.ReturnUrl);
                    //}

                    // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
                    return Redirect(model.ReturnUrl);
                }

                // request for a local page
                if (Url.IsLocalUrl(model.ReturnUrl))
                {
                    return Redirect(model.ReturnUrl);
                }
                else if (string.IsNullOrEmpty(model.ReturnUrl))
                {
                    return Redirect("~/");
                }
                else
                {
                    // user might have clicked on a malicious link - should be logged
                    throw new Exception("invalid return URL");
                }
            }

            await _events.RaiseAsync(new UserLoginFailureEvent(model.Username, "invalid credentials", clientId: context?.Client.ClientId));
            ModelState.AddModelError(string.Empty, "Invalid username or password");
        }

        model.EnableLocalLogin = true;
        //if (context?.Client.ClientId != null)
        //{
        //    var client = await _clientStore.FindEnabledClientByIdAsync(context.Client.ClientId);
        //    if (client != null)
        //    {
        //        model.EnableLocalLogin = client.EnableLocalLogin;
        //    }
        //}

        return View(model);
    }
}
