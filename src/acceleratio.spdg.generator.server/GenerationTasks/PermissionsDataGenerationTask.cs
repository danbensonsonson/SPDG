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
                    _allUsers = AD.GetUsersFromAD(Owner.WorkingDomains[0]);
                else
                    _allUsers = AD.GetUsersFromAD(IPGlobalProperties.GetIPGlobalProperties().DomainName);
            }
            return _allUsers;
        }

        protected override List<string> GetAvailableGroupsInDirectory()
        {
            if (_allGroups == null)
            {
                if (Owner.WorkingDomains.Count > 0)
                    _allGroups = AD.GetGroupsFromAD(Owner.WorkingDomains[0]);
                else
                    _allGroups = AD.GetGroupsFromAD(IPGlobalProperties.GetIPGlobalProperties().DomainName);
            }
            return _allGroups;
        }
    }
}
