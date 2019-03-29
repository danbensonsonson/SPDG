using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices.ActiveDirectory;

namespace AD
{
    class Program
    {
        private static List<string> shuffled = new List<string>();
        private static List<string> prinicpals;

        static void Main(string[] args)
        {
            int numberOfUsers = 2;
            int numberOfGroups = 2;
            int usersPerGroup = 2;
            string domain = "subdomain";
            string fqdn = "subdomain.ad.lab.lucidworks.com";
            string ou = "OU=Domain Controllers,CN=subdomain,CN=ad,CN=lab,CN=lucidworks,CN=com";

            SampleData.PrepareSampleCollections();
            List<string> users = createUsers(domain, ou, fqdn, numberOfUsers);
            createGroups(domain, ou, numberOfGroups, usersPerGroup, users);
            Console.ReadLine();
        }

        public static List<String> createUsers(string domain, string ou, string fqdn, int numOfUsers)
        {
            ContextType contextType = ContextType.Domain;
            //must pass null parameter to principalcontext if no ou selected
            if (ou == "")
            {
                ou = null;
            }
            var createdPrincipals = new List<string>();
            //using (PrincipalContext ctx = new PrincipalContext(contextType, domain, ou, "Administrator", "4SGNgNjhSk4XmubEAuvz9"))
            using (PrincipalContext ctx = new PrincipalContext(contextType, domain, ou))
            {
                for (int i = 0; i < numOfUsers; i++)
                {
                    try
                    {
                        UserPrincipal userPrincipal = new UserPrincipal(ctx);
                        Console.WriteLine(SampleData.GetSampleValueRandom(SampleData.LastNames));
                        userPrincipal.Surname = SampleData.GetSampleValueRandom(SampleData.LastNames);
                        userPrincipal.GivenName = SampleData.GetSampleValueRandom(SampleData.FirstNames);
                        userPrincipal.SamAccountName = getSamAccountName(userPrincipal.GivenName.ToLower(), userPrincipal.Surname.ToLower());
                        userPrincipal.Name = userPrincipal.GivenName + " " + userPrincipal.Surname;
                        userPrincipal.DisplayName = userPrincipal.GivenName + " " + userPrincipal.Surname;
                        userPrincipal.UserPrincipalName = userPrincipal.SamAccountName + "@" + fqdn;
                        
                        string pwdOfNewlyCreatedUser = "Acce1234!";

                        userPrincipal.SetPassword(pwdOfNewlyCreatedUser);
                        userPrincipal.Enabled = true;
                        userPrincipal.PasswordNeverExpires = true;
                        Console.WriteLine(string.Format("Creating {0}/{1} users: {2}", i, numOfUsers, domain + "\\" + userPrincipal.SamAccountName));
                        userPrincipal.Save();
                        Console.WriteLine(string.Format("Created {0}/{1} users: {2}", i, numOfUsers, userPrincipal.SamAccountName));
                        createdPrincipals.Add(userPrincipal.Sid.Value);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }
            }

            return createdPrincipals;
        }

        public static void createGroups(string domain, string ou, int numOfGroups, int maxNumberOfUsersPerGroup, List<string> principalList)
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
                        Console.WriteLine(string.Format("Created {0}/{1} groups.", i, numOfGroups));
                        principalList.Add(groupPrincipal.Sid.Value);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }
            }
        }

        private static void addPrincipalsToGroup(GroupPrincipal group, int maxNumberOfUsersPerGroup)
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

        private static string getSamAccountName(string fn, string ln)
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
