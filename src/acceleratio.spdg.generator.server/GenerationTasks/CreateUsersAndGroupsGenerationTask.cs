using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Text;
using Acceleratio.SPDG.Generator.GenerationTasks;

namespace Acceleratio.SPDG.Generator.Server.GenerationTasks
{
    class CreateUsersAndGroupsGenerationTask : DataGenerationTaskBase
    {
        private List<string> prinicpals;
        private List<string> shuffled = new List<string>();

        public override string Title
        {
            get { return "Creating Active Directory users and groups."; }
        }

        new ServerGeneratorDefinition WorkingDefinition
        {
            get { return (ServerGeneratorDefinition)base.WorkingDefinition; }
        }
       
        public CreateUsersAndGroupsGenerationTask(IDataGenerationTaskOwner owner) : base(owner)
        {

        }

        public override int CalculateTotalSteps()
        {
            // TODO need to re-work the math here to account for multiple domains
            var totalSteps = WorkingDefinition.NumberOfUsersToCreate + WorkingDefinition.NumberOfSecurityGroupsToCreate;
            if (WorkingDefinition.MaxNumberOfUsersInCreatedSecurityGroups > 0)
            {
                totalSteps += WorkingDefinition.NumberOfSecurityGroupsToCreate;
            }
            return totalSteps;
        }

        public override void Execute()
        {
            List<ServerUsersGroupsDefinition> ugDefinitions = WorkingDefinition.ServerUGDefinition;
            // Test if we have one or more user/group definitions
            if (ugDefinitions == null || ugDefinitions.Count < 1)
                return;

            // Support for multiple domains (of users/groups)
            foreach (ServerUsersGroupsDefinition sugd in ugDefinitions)
            {
                var users = new List<string>();
                if (sugd.NumberOfUsersToCreate > 0)
                {
                    try
                    {
                        Log.Write("Creating Active Directory users.");
                        users = createUsers(sugd.ADDomainName, sugd.ADOrganizationalUnit, sugd.NumberOfUsersToCreate);
                    }
                    catch (Exception ex)
                    {
                        Errors.Log(ex);
                    }
                }

                if (sugd.NumberOfSecurityGroupsToCreate > 0)
                {
                    try
                    {
                        Log.Write("Creating Active Directory groups.");
                        createGroups(sugd.ADDomainName, sugd.ADOrganizationalUnit, sugd.NumberOfSecurityGroupsToCreate, sugd.MaxNumberOfUsersInCreatedSecurityGroups, users);
                    }
                    catch (Exception ex)
                    {
                        Errors.Log(ex);
                    }

                }
            }
        }

        public List<String> createUsers(string domain, string ou, int numOfUsers)
        {
            ContextType contextType = ContextType.Domain;
            //must pass null parameter to principalcontext if no ou selected
            if (ou == "")
            {
                ou = null;
            }
            var createdPrincipals = new List<string>();
            using (PrincipalContext ctx = new PrincipalContext(contextType, domain, ou))
            {
                for (int i = 0; i < numOfUsers; i++)
                {
                    try
                    {
                        UserPrincipal userPrincipal = new UserPrincipal(ctx);
                        userPrincipal.Surname = SampleData.GetSampleValueRandom(SampleData.LastNames);
                        userPrincipal.GivenName = SampleData.GetSampleValueRandom(SampleData.FirstNames);
                        userPrincipal.SamAccountName = getSamAccountName(userPrincipal.GivenName.ToLower(), userPrincipal.Surname.ToLower());
                        userPrincipal.Name = userPrincipal.GivenName + " " + userPrincipal.Surname;
                        userPrincipal.DisplayName = userPrincipal.GivenName + " " + userPrincipal.Surname;
                        userPrincipal.UserPrincipalName = userPrincipal.SamAccountName + "@" + WorkingDefinition.Fqdn;

                        string pwdOfNewlyCreatedUser = "Acce1234!";

                        userPrincipal.SetPassword(pwdOfNewlyCreatedUser);
                        userPrincipal.Enabled = true;
                        userPrincipal.PasswordNeverExpires = true;
                        userPrincipal.Save();
                        Owner.IncrementCurrentTaskProgress(string.Format("Created {0}/{1} users.", i, numOfUsers));
                        createdPrincipals.Add(userPrincipal.Sid.Value);
                    }
                    catch (Exception ex)
                    {
                        Errors.Log(ex);
                    }
                }
            }

            return createdPrincipals;
        }

        public void createGroups(string domain, string ou, int numOfGroups, int maxNumberOfUsersPerGroup, List<string> principalList)
        {
            ContextType contextType = ContextType.Domain;
            //must pass null parameter to principalcontext if no ou selected
            if (ou == "")
            {
                ou = null;
            }

            prinicpals = principalList;

            using (PrincipalContext ctx = new PrincipalContext(contextType, domain, ou))
            {
                for (int i = 0; i < numOfGroups; i++)
                {
                    try
                    {
                        GroupPrincipal groupPrincipal = new GroupPrincipal(ctx);
                        groupPrincipal.Name = SampleData.GetSampleValueRandom(SampleData.Accounts);
                        groupPrincipal.DisplayName = groupPrincipal.Name;
                        groupPrincipal.SamAccountName = getSamAccountName(groupPrincipal.Name, null);
                        addPrincipalsToGroup(groupPrincipal, maxNumberOfUsersPerGroup);

                        groupPrincipal.Save();
                        Owner.IncrementCurrentTaskProgress(string.Format("Created {0}/{1} groups.", i, numOfGroups));
                        principalList.Add(groupPrincipal.Sid.Value);
                    }
                    catch (Exception ex)
                    {
                        Errors.Log(ex);
                    }
                }
            }
        }

        private void addPrincipalsToGroup(GroupPrincipal group, int maxNumberOfUsersPerGroup)
        {
            if (shuffled.Count == 0)
            {
                shuffled.AddRange(prinicpals);
                shuffled.Shuffle();
            }
            int takeCt = Math.Min(maxNumberOfUsersPerGroup, shuffled.Count);
            if (takeCt > 0)
            {
                var randomPrincipals = shuffled.Take(takeCt);
                shuffled.RemoveRange(0, takeCt);
                foreach (var randomPrincipal in randomPrincipals)
                {
                    group.Members.Add(group.Context, IdentityType.Sid, randomPrincipal);
                }
            }
        }

        private string getSamAccountName(string fn, string ln)
        {
            StringBuilder sb = new StringBuilder();
            if (fn != null && fn.Length > 0)
            {
                sb.Append(fn.Substring(0, 1).ToLower());
            }
            if (ln != null && ln.Length > 0)
            {
                sb.Append(ln.Substring(0, 1).ToLower());
            }
            sb.Append("" + new Random().Next());
            return sb.ToString();
        }
    }
}
