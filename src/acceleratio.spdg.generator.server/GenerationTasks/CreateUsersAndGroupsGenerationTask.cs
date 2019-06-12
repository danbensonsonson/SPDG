using System;
using System.Collections.Generic;
using System.Configuration;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Text;
using Acceleratio.SPDG.Generator.GenerationTasks;

namespace Acceleratio.SPDG.Generator.Server.GenerationTasks
{
    class CreateUsersAndGroupsGenerationTask : DataGenerationTaskBase
    {
        private List<string> prinicpals = new List<string>();
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

            if (WorkingDefinition.Mode == DataGeneratorMode.Resume)
            {
                // get users/groups to create the difference
            }

            // Support for multiple domains (of users/groups)
            // TODO, you don't always get the exact number you ask for due to silent expection handling. change eventually
            foreach (ServerUsersGroupsDefinition sugd in ugDefinitions)
            {
                var users = new List<string>();
                Owner.WorkingDomains.Add(sugd.ADDomainName);
                int numUsersToCreate = sugd.NumberOfUsersToCreate;
                int numGroupsToCreate = sugd.NumberOfSecurityGroupsToCreate;
                if (WorkingDefinition.Mode == DataGeneratorMode.Resume && numUsersToCreate > 0)
                {
                    Owner.WorkingUsers.AddRange(AD.GetUsersFromAD(sugd.ADDomainName));
                    numUsersToCreate = numUsersToCreate - Owner.WorkingUsers.Count;
                }
                
                if (numUsersToCreate > 0)
                {
                    try
                    {
                        Log.Write("Creating Active Directory users.");
                        if (WorkingDefinition.Mode != DataGeneratorMode.Incremental)
                            WorkingDefinition.Mode = DataGeneratorMode.Resume;
                        users = createUsers(sugd.ADDomainName, sugd.ADOrganizationalUnit, numUsersToCreate);
                    }
                    catch (Exception ex)
                    {
                        Errors.Log(ex);
                        // TODO: catastrophic user error
                    }
                }

                // Write config file to resume?
                GeneratorDefinitionBase.SerializeDefinition(DataGenerator.SessionID + ".xml", WorkingDefinition);

                if (WorkingDefinition.Mode == DataGeneratorMode.Resume && numGroupsToCreate > 0)
                {
                    Owner.WorkingGroups.AddRange(AD.GetGroupsFromAD(sugd.ADDomainName));
                    numGroupsToCreate = numGroupsToCreate - Owner.WorkingGroups.Count;
                }

                try
                {
                    Log.Write("Creating Active Directory groups.");
                    if (WorkingDefinition.Mode != DataGeneratorMode.Incremental)
                        WorkingDefinition.Mode = DataGeneratorMode.Resume;
                    createGroups(sugd.ADDomainName, sugd.ADOrganizationalUnit, numGroupsToCreate, sugd.MaxNumberOfUsersInCreatedSecurityGroups, users);
                }
                catch (Exception ex)
                {
                    Errors.Log(ex);
                }

                // Write config file to resume?
                GeneratorDefinitionBase.SerializeDefinition(DataGenerator.SessionID + ".xml", WorkingDefinition);
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
            //using (PrincipalContext ctx = new PrincipalContext(contextType, domain, ou, ConfigurationManager.AppSettings["adUser"], ConfigurationManager.AppSettings["adPassword"]))
            using (PrincipalContext ctx = new PrincipalContext(contextType, domain, ou))
            {
                // TODO: test if there is something wrong with the context, then throw exception
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
                        Owner.WorkingUsers.Add(userPrincipal.UserPrincipalName);

                        string pwdOfNewlyCreatedUser = "Acce1234!";

                        userPrincipal.SetPassword(pwdOfNewlyCreatedUser);
                        userPrincipal.Enabled = true;
                        userPrincipal.PasswordNeverExpires = true;
                        Owner.IncrementCurrentTaskProgress(string.Format("Creating {0}/{1} users: {2}", i, numOfUsers, domain + "\\" + userPrincipal.SamAccountName));
                        userPrincipal.Save();
                        Owner.IncrementCurrentTaskProgress(string.Format("Created {0}/{1} users: {2}", i, numOfUsers, userPrincipal.SamAccountName));
                        createdPrincipals.Add(userPrincipal.Sid.Value);  // this was SID, wonder if I need to assign user to groups
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

            // Resume - Look through the groups to see if the correct number of users have been added
            if (WorkingDefinition.Mode == DataGeneratorMode.Resume)
            {
                using (PrincipalContext ctx = new PrincipalContext(contextType, domain, ou))
                {
                    foreach (string groupSid in Owner.WorkingGroups)
                    {
                        GroupPrincipal gp = GroupPrincipal.FindByIdentity(ctx, groupSid);
                        if (gp == null)
                            continue;
                        PrincipalCollection pc = gp.Members;
                        int membersToAdd = maxNumberOfUsersPerGroup - pc.Count;
                        addPrincipalsToGroup(gp, membersToAdd);
                        gp.Save();
                    }
                }
            }

            prinicpals = principalList;

            // Create
            // using (PrincipalContext ctx = new PrincipalContext(contextType, domain, ou, ConfigurationManager.AppSettings["adUser"], ConfigurationManager.AppSettings["adPassword"]))
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
                        Owner.WorkingGroups.Add(groupPrincipal.Sid.Value);
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
            if (maxNumberOfUsersPerGroup < 1)
                return;

            if (shuffled.Count == 0)
            {
                if (prinicpals.Count > Owner.WorkingUsers.Count)
                    shuffled.AddRange(prinicpals);
                else
                    shuffled.AddRange(Owner.WorkingUsers);
                shuffled.Shuffle();
            }
            int takeCt = Math.Min(maxNumberOfUsersPerGroup, shuffled.Count);
            if (takeCt > 0)
            {
                var randomPrincipals = shuffled.Take(takeCt);
                shuffled.RemoveRange(0, takeCt);
                foreach (var randomPrincipal in randomPrincipals)
                {
                    try
                    {
                        if (prinicpals.Count > Owner.WorkingUsers.Count)
                            group.Members.Add(group.Context, IdentityType.Sid, randomPrincipal); // this was Sid
                        else
                            group.Members.Add(group.Context, IdentityType.UserPrincipalName, randomPrincipal);
                    }
                    catch (Exception ex)
                    { }
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
