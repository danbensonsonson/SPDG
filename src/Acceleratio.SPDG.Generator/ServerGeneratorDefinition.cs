using System.Collections.Generic;
namespace Acceleratio.SPDG.Generator
{
    public class ServerGeneratorDefinition : GeneratorDefinitionBase
    {  
        public override bool IsClientObjectModel { get { return false; } }
        public string SharePointURL { get; set; }              
        public string WebAppOwnerLogin { get; set; }
        public string WebAppOwnerPassword { get; set; }
        public string WebAppOwnerEmail { get; set; }
        public string DatabaseServer { get; set; }
        // TODO: Removed the following two variables
        public string ADDomainName { get; set; }
        public string ADOrganizationalUnit { get; set; }
        public List<ServerUsersGroupsDefinition> ServerUGDefinition { get; set; }
        public int CreateNewWebApplications { get; set; }
        public string UseExistingWebApplication { get; set; }
        public string UseExistingWebApplicationName { get; set; }
        public bool CreateOutOfTheBoxWorkflowsToList { get; set; }
        public bool AttachCustomWorkflowToList { get; set; }
        public string Fqdn { get; set; }

        public ServerGeneratorDefinition()
        {
            ServerUGDefinition = new List<ServerUsersGroupsDefinition>();
        }
    }

    public class ServerUsersGroupsDefinition
    {
        private int _numberOfUsersToCreate;
        private int _numberOfSecurityGroupsToCreate;
        private int _maxNumberOfUsersInCreatedSecurityGroups;

        public string ADDomainName { get; set; }
        public string ADOrganizationalUnit { get; set; }
        public string FQDN { get; set; }
        public int NumberOfUsersToCreate
        {
            get
            {
                return _numberOfUsersToCreate;

            }
            set
            {
                _numberOfUsersToCreate = value;
            }
        }

        public int NumberOfSecurityGroupsToCreate
        {
            get
            {
                return _numberOfSecurityGroupsToCreate;
            }
            set { _numberOfSecurityGroupsToCreate = value; }
        }

        public int MaxNumberOfUsersInCreatedSecurityGroups
        {
            get
            {
                return _maxNumberOfUsersInCreatedSecurityGroups;
            }
            set { _maxNumberOfUsersInCreatedSecurityGroups = value; }
        }
    }
}