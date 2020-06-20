using BlitzkriegSoftware.Tenant.Libary;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace BlitzkriegSoftware.Tenant.Demo.Web.Controllers
{
    public class _CoreTenantControllerBase : Controller
    {
        protected readonly ILogger<_CoreTenantControllerBase> _logger;
        private TenantUserProfileBase _user;
        private Guid _tenantid;

        protected Guid TenantId
        {
            get
            {
                if(this._tenantid == Guid.Empty)
                {
                    this._tenantid = this.TenantUser.Tenants.FirstOrDefault();
                }
                return this._tenantid;
            }
        }

        protected TenantUserProfileBase TenantUser
        {
            get
            {
                if(this._user == null)
                {
                    var username = Libs.ParseName.RemoveParts(this.User.Identity.Name, Libs.ParseParts.PoundPart);
                    this._user = Program.UserProvider.Read(username);
                    this.Enrich(ref this._user);
                }
                return this._user;
            }
        }

        public const int ClientCookie_Expires_Milliseconds = 1000 * 60 * 60;
        public const string ClientCookie_Name = "TenantId";

        /// <summary>
        /// Set Client Cookie for Styling
        /// </summary>
        protected void ClientCookieSet()
        {
#pragma warning disable SCS0008 // The cookie is missing security flag Secure
#pragma warning disable SCS0009 // The cookie is missing security flag HttpOnly
            var option = new CookieOptions
            {
                Expires = DateTime.Now.AddMilliseconds(ClientCookie_Expires_Milliseconds),
                HttpOnly = false,
                Secure = false,
                IsEssential = true,
                SameSite = SameSiteMode.Lax,
                Path = "/"
            };
#pragma warning restore SCS0009 // The cookie is missing security flag HttpOnly
#pragma warning restore SCS0008 // The cookie is missing security flag Secure

            this.Response?.Cookies.Append(ClientCookie_Name, this.TenantId.ToString(), option);
        }

        public _CoreTenantControllerBase(ILogger<_CoreTenantControllerBase> logger)
        {
            this._logger = logger;
        }

        protected static string[] okKeys = new string[] { "emailaddress", "surname", "givenname" };

        protected void Enrich(ref TenantUserProfileBase model)
        {
            if (model == null) return;

            foreach (var c in this.User.Claims)
            {
                var key = c.Type;
                var index = key.LastIndexOf('/');
                if (index >= 0) key = key.Substring(index + 1);
                var value = c.Value;
                if (okKeys.Contains(key))
                {
                    model.SettingsPut(key, value);
                }
            }
        }

    }
}
