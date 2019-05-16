using System.Collections.Generic;
using System.Net.NetworkInformation;
using Acceleratio.SPDG.Generator.GenerationTasks;

namespace Acceleratio.SPDG.Generator.Server.GenerationTasks
{
    class PermissionsDataGenerationTask : PermissionsDataGenerationTaskBase
    {

        List<string> _allUsers;
        List<string> _allGroups;       

        public PermissionsDataGenerationTask(IDataGenerationTaskOwner owner) : base(owner)
        {
        }

        protected override List<string> GetAvailableUsersInDirectory()
        {
            if (_allUsers == null)
            {
                if (Owner.WorkingDomains.Count > 0)
                    _allUsers = AD.GetAllUsersFromAD(Owner.WorkingDomains[0]);
                else
                    _allUsers = AD.GetAllUsersFromAD(IPGlobalProperties.GetIPGlobalProperties().DomainName);
            }
            return _allUsers;
        }

        protected override List<string> GetAvailableGroupsInDirectory()
        {
            if (_allGroups == null)
            {
                if (Owner.WorkingDomains.Count > 0)
                    _allGroups = AD.GetAllGroupsFromAD(Owner.WorkingDomains[0]);
                else
                    _allGroups = AD.GetAllGroupsFromAD(IPGlobalProperties.GetIPGlobalProperties().DomainName);
            }
            return _allGroups;
        }
    }
}
