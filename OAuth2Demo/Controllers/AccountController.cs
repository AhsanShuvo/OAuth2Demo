using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using OAuth2Demo.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OAuth2Demo.Controllers
{
    public class AccountController : Controller
    {
        
        [HttpGet]
        public IActionResult Index(string returnUrl)
        {
            LoginViewModel model = new LoginViewModel()
            {
                ReturnUrl = returnUrl,
                ExternalLogins = new List<string>() {"Google", "Facebook"}
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult ExternalLogin(string provider)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account");

            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };

            return Challenge(properties, provider);
        }

        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            LoginViewModel loginViewModel = new LoginViewModel
            {
                ReturnUrl = returnUrl,
                ExternalLogins = new List<string>() { "Google", "Facebook" }
            };
            var response = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            var claims = response.Principal.Identities.FirstOrDefault();
            return RedirectToAction("Index", loginViewModel);
        }
    }
}
