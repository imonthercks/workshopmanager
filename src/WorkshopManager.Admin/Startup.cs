using System;
using System.Threading.Tasks;
using IdentityManager.Configuration;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(WorkshopManager.Admin.Startup))]

namespace WorkshopManager.Admin
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var factory = new MembershipRebootIdentityManagerFactory("MembershipReboot");
            // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888
            var options = new IdentityManagerOptions
            {
                SecurityConfiguration = new LocalhostSecurityConfiguration() {RequireSsl = false},
                
                Factory = factory.Create()
                

                // configuration values in here
            };
            app.UseIdentityManager(options);

        }
    }
}
