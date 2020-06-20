using BlitzkriegSoftware.Tenant.Demo.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Localization;
using System;


// See: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/localization?view=aspnetcore-3.1#make-the-apps-content-localizable

namespace BlitzkriegSoftware.Tenant.Demo.Web.Controllers
{
    [Authorize]
    public class HomeController : _CoreTenantControllerBase
    {
        public HomeController(ILogger<_CoreTenantControllerBase> logger) : base(logger) { }
        
        public IActionResult Index()
        {
            this.ClientCookieSet();
            return this.RedirectToAction("Home");
        }

        public IActionResult Home()
        {
            return this.View(this.TenantUser);
        }

        public IActionResult AadInfo()
        {
            return this.View(this.User);
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return this.View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
        }

        /// <summary>
        /// Set Langauge Post Back From Partial
        /// </summary>
        /// <param name="culture"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            this.Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return this.LocalRedirect(returnUrl);
        }

    }
}
