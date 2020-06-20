using System;
using System.Collections.Generic;
using BlitzkriegSoftware.Tenant.Libary;
using BlitzkriegSoftware.Tenant.MongoProvider;
using BlitzkriegSoftware.Tenant.MongoProvider.Models;


namespace BlitzkriegSoftware.Tenant.Demo.Seeder
{
    class Program
    {
        static void Main(string[] args)
        {
            BuildDemoData();
        }

        #region "Demo Data"

        public static string[] Usernames = new string[] { "spookdejur@gmail.com", "spookdejur@hotmail.com" };

        public static Guid[] Tenants = new Guid[] {
            new Guid("{00000000-0000-0000-0000-000000000001}"),
            new Guid("{00000000-0000-0000-0000-000000000002}")
        };

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

        public static void BuildDemoData()
        {
            foreach (var t in Tenants)
            {
                var model = new TenantBase()
                {
                    _id = t,
                    Contact = new ContactBase()
                    {
                        DisplayName = t.ToString(),
                        TenantId = t,
                        ContactName = t.ToString()

                    },
                    Configuration = new List<KeyValuePair<string, string>>()
                    {
                      new KeyValuePair<string, string>("TenantDb", t.ToString())
                    }
                };
                Console.WriteLine(model.ToString());
                TenantProvider.TenantAddUpdate(model);
            }

            int ct = 0;
            foreach (var u in Usernames)
            {
                var model = new TenantUserProfileBase()
                {
                    UniqueUserId = u,
                    _id = Guid.NewGuid(),
                };
                if (ct % 2 == 0)
                {
                    model.Tenants.Add(Tenants[0]);
                }
                else
                {
                    model.Tenants.Add(Tenants[1]);
                }

                model.SettingsPut("ct", ct.ToString());

                Console.WriteLine(model.ToString());

                UserProvider.Write(model);
                ct++;
            }

        }

        #endregion

    }
}
