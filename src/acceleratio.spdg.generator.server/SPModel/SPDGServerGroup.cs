using System.Collections.Generic;
using Acceleratio.SPDG.Generator.SPModel;
using Microsoft.SharePoint;

namespace Acceleratio.SPDG.Generator.Server.SPModel
{
    class SPDGServerGroup : SPDGGroup
    {
        private readonly SPGroup _group;

        public SPDGServerGroup(SPGroup group) : base(group.ID, group.LoginName, group.Name)
        {
            _group = @group;            
        }

        public SPGroup Group
        {
            get { return _group; }
        }

        public override void AddUsers(IEnumerable<SPDGUser> users)
        {
            foreach (var spdgUser in users)
            {
                _group.AddUser(((SPDGServerUser)spdgUser).SPUser);
            }
        }

        public override void RemoveUsers(int count)
        {            
            int numUsersInGroup = _group.Users.Count;
            int numberToDelete = count < numUsersInGroup ? count : numUsersInGroup;
            for (int i = 0; i < numberToDelete; i++)
            {
                _group.RemoveUser(_group.Users[i]);
            }
        }

        public override IEnumerable<SPDGUser> Users
        {
            get
            {
                foreach (SPUser spuser in _group.Users)
                {
                    yield return new SPDGServerUser(spuser);
                }
            }
        }
    }
}
