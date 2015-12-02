// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultRegistry.cs" company="Web Advanced">
// Copyright 2012 Web Advanced (www.webadvanced.com)
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using BrockAllen.MembershipReboot;
using BrockAllen.MembershipReboot.Hierarchical;
using BrockAllen.MembershipReboot.WebHost;
using Raven.Client;
using Raven.Client.Document;
using StructureMap.Pipeline;
using StructureMap.Web;
using Thinktecture.IdentityModel.Web;

namespace WorkshopManager.Web.DependencyResolution {
    using StructureMap.Configuration.DSL;
    using StructureMap.Graph;
	
    public class DefaultRegistry : Registry {
        #region Constructors and Destructors

        public DefaultRegistry() {
            Scan(
                scan => {
                    scan.TheCallingAssembly();
                    scan.WithDefaultConventions();
					scan.With(new ControllerConvention());
                });

            var docStore = GetRavenDbDocumentStore();

            PassiveSessionConfiguration.ConfigureSessionCache(new RavenDbTokenCacheRepository(docStore));

            var config = MembershipRebootConfig.Create();
            config.RequireAccountVerification = false;
            For<MembershipRebootConfiguration<HierarchicalUserAccount>>().Use(config);

            For<IDocumentStore>().Use(docStore);
            For<UserAccountService<HierarchicalUserAccount>>().Use<UserAccountService<HierarchicalUserAccount>>();
            For<AuthenticationService<HierarchicalUserAccount>>().Use<SamAuthenticationService<HierarchicalUserAccount>>();
            For<IUserAccountQuery>().HybridHttpOrThreadLocalScoped().Use<RavenUserAccountRepository>();
            For<IUserAccountRepository<HierarchicalUserAccount>>().HybridHttpOrThreadLocalScoped().Use<RavenUserAccountRepository>();
        }

        private IDocumentStore GetRavenDbDocumentStore()
        {
            var docStore = new DocumentStore();
            var ravenDbUrl = Environment.GetEnvironmentVariable("MembershipReboot.RavenDb.Url") ?? System.Configuration.ConfigurationManager.AppSettings.Get("MembershipReboot.RavenDb.Url");
            var ravenApiKey = Environment.GetEnvironmentVariable("MembershipReboot.RavenDb.ApiKey") ?? System.Configuration.ConfigurationManager.AppSettings.Get("MembershipReboot.RavenDb.ApiKey");

            if (!String.IsNullOrEmpty(ravenDbUrl))
            {
                docStore.Url = ravenDbUrl;
                if (!string.IsNullOrEmpty(ravenApiKey))
                    docStore.ApiKey = ravenApiKey;
            }
            else
            {
                docStore.ConnectionStringName = "MembershipReboot";
            }
            
            docStore.Initialize();
            return docStore;
        }

        #endregion
    }
}