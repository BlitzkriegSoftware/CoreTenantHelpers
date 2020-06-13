using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using BlitzkriegSoftware.Tenant.Libary;
using BlitzkriegSoftware.Tenant.MongoProvider;
using BlitzkriegSoftware.Tenant.MongoProvider.Models;

namespace BlitzkriegSoftware.Tenant.Demo.Web
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        #region "Providers"

        private static TenantProvider<TenantBase> _tenantprovider;

        public static TenantProvider<TenantBase> TenantProvider
        {
            get
            {
                if (_tenantprovider == null)
                {
                    var config = new MongoConfiguration()
                    {
                        Database = "TenantProvider",
                        Collection = "Tenants"
                    };

                    var tdp = new MongoTenantDataProvider<TenantBase>(config);
                    _tenantprovider = new TenantProvider<TenantBase>(tdp);
                }
                return _tenantprovider;
            }
        }

        private static MongoTenantUserProfileProvider<TenantUserProfileBase> _userprovider;

        public static MongoTenantUserProfileProvider<TenantUserProfileBase> UserProvider
        {
            get
            {
                if (_userprovider == null)
                {

                    var config = new MongoConfiguration()
                    {
                        Database = "TenantProvider",
                        Collection = "Users"
                    };

                    _userprovider = new MongoTenantUserProfileProvider<TenantUserProfileBase>(config);
                }
                return _userprovider;
            }
        }

        #endregion

    }
}
