using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using BrockAllen.MembershipReboot;
using BrockAllen.MembershipReboot.Hierarchical;
using IdentityManager;
using IdentityManager.Configuration;
using IdentityManager.MembershipReboot;

namespace WorkshopManager.Admin
{
    public class MembershipRebootIdentityManagerFactory
    {
        string connString;
        public MembershipRebootIdentityManagerFactory(string connString)
        {
            this.connString = connString;
        }

        public IdentityManagerServiceFactory Create()
        {
            var userRepo = new RavenUserAccountRepository("MembershipReboot");
            var userSvc = new UserAccountService<HierarchicalUserAccount>(new MembershipRebootConfiguration<HierarchicalUserAccount>(), userRepo);

            var groupRepo = new RavenGroupRepository("MembershipReboot");
            var groupSvc = new GroupService<HierarchicalGroup>(groupRepo);

            var idMgr = new MembershipRebootRavenDbIdentityManagerService(userSvc, userRepo, groupSvc, groupRepo);
            var factory = new IdentityManagerServiceFactory {IdentityManagerService = new Registration<IIdentityManagerService>(idMgr) };
            return factory;
        }
    }

    public class MembershipRebootRavenDbIdentityManagerService :
        MembershipRebootIdentityManagerService<HierarchicalUserAccount, HierarchicalGroup>
    {
        public MembershipRebootRavenDbIdentityManagerService(UserAccountService<HierarchicalUserAccount> userAccountService, GroupService<HierarchicalGroup> groupService, bool includeAccountProperties = true) : base(userAccountService, groupService, includeAccountProperties)
        {
        }

        public MembershipRebootRavenDbIdentityManagerService(UserAccountService<HierarchicalUserAccount> userAccountService, IUserAccountQuery<HierarchicalUserAccount> userQuery, GroupService<HierarchicalGroup> groupService, IGroupQuery groupQuery, bool includeAccountProperties = true) : base(userAccountService, userQuery, groupService, groupQuery, includeAccountProperties)
        {
        }

        public MembershipRebootRavenDbIdentityManagerService(UserAccountService<HierarchicalUserAccount> userAccountService, IUserAccountQuery<HierarchicalUserAccount> userQuery, GroupService<HierarchicalGroup> groupService, IGroupQuery groupQuery, IdentityManagerMetadata metadata) : base(userAccountService, userQuery, groupService, groupQuery, metadata)
        {
        }

        public MembershipRebootRavenDbIdentityManagerService(UserAccountService<HierarchicalUserAccount> userAccountService, IUserAccountQuery<HierarchicalUserAccount> userQuery, GroupService<HierarchicalGroup> groupService, IGroupQuery groupQuery, Func<Task<IdentityManagerMetadata>> metadataFunc) : base(userAccountService, userQuery, groupService, groupQuery, metadataFunc)
        {
        }

        protected override IQueryable<HierarchicalUserAccount> DefaultFilter(IQueryable<HierarchicalUserAccount> query,
            string filter)
        {
            return query.Where(x => x.Username.StartsWith(filter));
            //return base.DefaultFilter(query, filter);

        }

        protected override IQueryable<HierarchicalUserAccount> DefaultSort(IQueryable<HierarchicalUserAccount> query)
        {
            return base.DefaultSort(query);
        }
    }
}
