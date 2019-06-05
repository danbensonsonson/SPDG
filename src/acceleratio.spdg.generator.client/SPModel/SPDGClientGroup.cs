using System.Collections.Generic;
using Acceleratio.SPDG.Generator.SPModel;
using Microsoft.SharePoint.Client;

namespace Acceleratio.SPDG.Generator.Client.SPModel
{
    class SPDGClientGroup : SPDGGroup
    {
        private readonly Group _group;
        private List<SPDGUser> _groupUsers;
        private readonly ClientContext _context;

        public SPDGClientGroup(Group group, ClientContext context) : base(group.Id, group.LoginName, group.LoginName)
        {
            _group = @group;
            _context = context;
        }

        public Group Group
        {
            get { return _group; }
        }

        public override void AddUsers(IEnumerable<SPDGUser> users)
        {
            LoadUsers();
            foreach (SPDGClientUser spdgUser in users)
            {
                _group.Users.AddUser(spdgUser.User);
            }
            _context.ExecuteQuery();
         
        }

        public override void RemoveUsers(int count)
        {
            LoadUsers();
            int numUsersInGroup = _group.Users.Count;
            int numberToDelete = count < numUsersInGroup ? count : numUsersInGroup;
            for (int i = 0; i < numberToDelete; i++)
            {
                _group.Users.Remove(_group.Users[i]);
            }
        }

        public override IEnumerable<SPDGUser> Users
        {
            get
            {
                LoadUsers();
                foreach (User spuser in _group.Users)
                {
                    yield return new SPDGClientUser(spuser);
                }
            }
        }

        private void LoadUsers()
        {
            if (_groupUsers == null)
            {   // TODO: Might need a dirty flag as well in case the group changes, refetch from the server
                _groupUsers = new List<SPDGUser>();
                _context.Load(_group.Users);
                _context.ExecuteQuery();
                foreach (var user in _group.Users)
                {
                    _groupUsers.Add(new SPDGClientUser(user));
                }
            }
        }
    }
}
