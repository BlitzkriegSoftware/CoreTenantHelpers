using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Globalization;


namespace BlitzkriegSoftware.Tenant.Demo.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        /// <summary>
        /// Cultures
        /// </summary>
        public string[] Cultures
        {
            get
            {
                var setting = this.Configuration["AllowedLanguages"];
                if(string.IsNullOrWhiteSpace(setting))
                {
                    setting = "en-US";
                }
                var cultures = setting.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                return cultures;
            }
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            _ = services.AddAuthentication(AzureADDefaults.AuthenticationScheme)
                .AddAzureAD(options => this.Configuration.Bind("AzureAd", options));

            _ = services.AddLocalization(options => options.ResourcesPath = "Resources");

            _ = services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                    policy.AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowAnyOrigin());
            });

            _ = services.AddHsts(options =>
            {
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromMilliseconds(31536000);
            });

            _ = services.AddAntiforgery(options =>
            {
                options.FormFieldName = "AntiforgeryFieldname";
                options.HeaderName = "X-CSRF-TOKEN-HEADERNAME";
                options.SuppressXFrameOptionsHeader = false;
            });

            _ = services.AddMvc(
                config =>
                {
                    config.Filters.Add(typeof(Libs.GlobalExceptionFilter));
                })
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                .AddDataAnnotationsLocalization();

            _ = services.Configure<RequestLocalizationOptions>(opts =>
            {
                var supportedCultures = new List<CultureInfo>();

                foreach(var c in this.Cultures)
                {
                    supportedCultures.Add(new CultureInfo(c));
                }
                
                opts.DefaultRequestCulture = new RequestCulture(this.Cultures[0]);

                // Formatting numbers, dates, etc.
                opts.SupportedCultures = supportedCultures;

                // UI strings that we have localized.
                opts.SupportedUICultures = supportedCultures;
            });

            _ = services.AddControllersWithViews(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            });

            _ = services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");

                app.UseHsts();
            }

            app.Use(async (context, next) =>
            {
                // Prohibited Headers
                _ = context.Response.Headers.Remove("splitsdkversion");
                _ = context.Response.Headers.Remove("x-aspnet-version");
                _ = context.Response.Headers.Remove("x-powered-by");
                _ = context.Response.Headers.Remove("server");

                // Required Headers
                context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
                context.Response.Headers.Add("X-Frame-Options", "DENY");
                context.Response.Headers.Add("X-Xss-Protection", "1;mode=block");

                await next().ConfigureAwait(false);
            });

            app.UseHttpsRedirection();

            var localizationOptions = new RequestLocalizationOptions().SetDefaultCulture(this.Cultures[0])
                .AddSupportedCultures(this.Cultures)
                .AddSupportedUICultures(this.Cultures);

            app.UseRequestLocalization(localizationOptions);

            // var options = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            // app.UseRequestLocalization(options.Value);

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
