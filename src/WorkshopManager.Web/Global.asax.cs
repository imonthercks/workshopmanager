using System;
using System.Collections.Generic;
using System.IdentityModel.Services;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Raven.Client;
using Thinktecture.IdentityModel.Web;

namespace WorkshopManager.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

        }

        public override void Init()
        {
            base.Init();

            var sam = FederatedAuthentication.SessionAuthenticationModule;
            sam.IsReferenceMode = true;

            PassiveModuleConfiguration.CacheSessionsOnServer();
        }
    }

    public class RavenDbTokenCacheRepository : ITokenCacheRepository
    {
        private readonly IDocumentStore _docStore;

        public RavenDbTokenCacheRepository(IDocumentStore docStore)
        {
            _docStore = docStore;
        }

        public void AddOrUpdate(TokenCacheItem item)
        {
            var session = _docStore.OpenSession();
            var dbItem = session.Load<TokenCacheItem>(item.Key);

            if (dbItem == null)
            {
                dbItem = new TokenCacheItem {Key = item.Key};
                session.Store(dbItem);
            }
            dbItem.Token = item.Token;
            dbItem.Expires = item.Expires;
            session.SaveChanges();
        }

        public TokenCacheItem Get(string key)
        {
            var session = _docStore.OpenSession();
            var dbItem = session.Load<TokenCacheItem>(key);
            return dbItem;
        }

        public void Remove(string key)
        {
            var session = _docStore.OpenSession();
            var dbItem = session.Load<TokenCacheItem>(key);
            session.Delete(dbItem);
        }
    }
}
