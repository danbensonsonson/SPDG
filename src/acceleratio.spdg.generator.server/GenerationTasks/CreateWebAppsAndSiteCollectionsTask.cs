using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using Acceleratio.SPDG.Generator.GenerationTasks;
using Acceleratio.SPDG.Generator.Structures;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.Office.Server.UserProfiles;

namespace Acceleratio.SPDG.Generator.Server.GenerationTasks
{
    class CreateWebAppsAndSiteCollectionsTask : DataGenerationTaskBase
    {
        public override string Title { get { return "Creating Web Applications / Site Collections"; } }

        public CreateWebAppsAndSiteCollectionsTask(IDataGenerationTaskOwner owner) : base(owner)
        {
        }


        new ServerGeneratorDefinition WorkingDefinition
        {
            get { return (ServerGeneratorDefinition)base.WorkingDefinition; }
        }

        public override int CalculateTotalSteps()
        {
            var totalSteps = WorkingDefinition.CreateNewSiteCollections + WorkingDefinition.CreateMySites;
            if (WorkingDefinition.CreateNewWebApplications > 0)
            {
                totalSteps = (WorkingDefinition.CreateNewSiteCollections * WorkingDefinition.CreateNewWebApplications) + WorkingDefinition.CreateNewWebApplications + WorkingDefinition.CreateMySites;
            }
            return totalSteps;
        }
        
        public override void Execute()
        {
            if (WorkingDefinition.CreateNewWebApplications > 0)
            {
                createNewWebApplications();
                GeneratorDefinitionBase.SerializeDefinition(DataGenerator.SessionID + ".xml", WorkingDefinition);
            }
            // use existing web app
            else if (WorkingDefinition.CreateNewSiteCollections > 0 || WorkingDefinition.CreateMySites > 0) //ok?
            {
                createNewSiteCollections();
                GeneratorDefinitionBase.SerializeDefinition(DataGenerator.SessionID + ".xml", WorkingDefinition);
            }

        }

        private void CreateMySites(SPWebApplication webApp)
        {
            if (WorkingDefinition.CreateMySites <= 0)
                return;

            try {
                //get current service context
                SPSite site = webApp.Sites[0];
                SPServiceContext serviceContext = SPServiceContext.GetContext(site);

                //initialize user profile config manager object
                UserProfileManager upm = new UserProfileManager(serviceContext);
                // loop through users based on number of CreateMySites
                List<string> users = Owner.WorkingUsers;

                if (users.Count < WorkingDefinition.CreateMySites)
                {   // TODO - how to get a working domain without configuration?
                    users = AD.GetUsersFromAD(IPGlobalProperties.GetIPGlobalProperties().DomainName); // Just grab a big chunk of users to work from. This call can take time. Use sparingly
                    // Working users is no longer necessary. Save the list for farther down the line to avoid the above call
                    Owner.WorkingUsers.AddRange(users);
                }

                bool createdMySite = false;
                int createdMySites = 0;
                foreach (string user in users)
                //for (int s = 0; s < users.Count; s++)
                {
                    // Get the account name
                    createdMySite = CreateMySite(upm, user); 
                    if (createdMySite)
                    {
                        Owner.IncrementCurrentTaskProgress(string.Format("Created {0}/{1} my sites.", createdMySites + 1, WorkingDefinition.CreateMySites));
                        createdMySites++;
                    }
                    if (createdMySites >= WorkingDefinition.CreateMySites)
                        break;
                }
            }
            catch (Exception ex)
            {
                Errors.Log(ex);
            }            
        }

        private bool CreateMySite(UserProfileManager upm, string sAccount)
        {
            try { 
                //create user profile is one doesn't already exist
                if (!upm.UserExists(sAccount))
                    upm.CreateUserProfile(sAccount);

                //to set prop values on user profile
                UserProfile u = upm.GetUserProfile(sAccount);
                if (u.PersonalSite == null)
                {
                    u.CreatePersonalSite();
                    Owner.IncrementCurrentTaskProgress("Created my site '" + u.PersonalSite.Url + "'");
                    return true;
                }
                else
                    return false; // My Site already existed
            }
            catch (Exception ex)
            {
                Log.Write("Could not create my site for '" + sAccount + "'");
                Errors.Log(ex);
                return false;
            }
        }

        private void createNewSiteCollections()
        {
            try
            {
                SPWebService spWebService = SPWebService.ContentService;
                SPWebApplication webApp = spWebService.WebApplications.First(a => a.Id == new Guid(WorkingDefinition.UseExistingWebApplication));

                // Reusme: number
                int numSiteCollections = WorkingDefinition.CreateNewSiteCollections - Owner.WorkingSiteCollections.Count;
                for (int s = 0; s < numSiteCollections; s++)
                {
                    CreateSiteCollection(webApp);
                }

                CreateMySites(webApp);
            }
            catch (Exception ex)
            {
                Errors.Log(ex);
            }
        }

        private void CreateSiteCollection(SPWebApplication webApp)
        {
            string sitCollName = "";
            string url = "";
            string baseName = "";

            findAvailableSiteCollectionName(webApp, out sitCollName, out url, out baseName);

            Owner.IncrementCurrentTaskProgress("Creating site collection '" + url + "'");

            SPSiteCollection siteCollections = webApp.Sites;
            SPSite site = siteCollections.Add("/sites/" + url, WorkingDefinition.SiteCollOwnerLogin,
                WorkingDefinition.SiteCollOwnerEmail);

            SPWeb web = site.RootWeb;
            web.Title = sitCollName;
            web.Update();

            SiteCollInfo siteCollInfo = new SiteCollInfo();
            siteCollInfo.URL = site.Url;

            Owner.WorkingSiteCollections.Add(siteCollInfo);
            WorkingDefinition.SiteCollections.Add(siteCollInfo.URL); // Add to configuration for serialization
            WorkingDefinition.UseExistingSiteCollection = true;
        }

        private void findAvailableSiteCollectionName(SPWebApplication webApp, out string siteName, out string url, out string baseName)
        {
            baseName = "";
            siteName = SampleData.GetSampleValueRandom(SampleData.Companies);
            string siteUrl = Utils.GenerateSlug(siteName, 25);

            int i = 0;
            while (webApp.Sites.Any(s => s.Url.Contains(siteUrl)))
            {
                siteName = SampleData.GetRandomName(SampleData.Companies, SampleData.Offices, null, ref i, out baseName);
                siteUrl = Utils.GenerateSlug(siteName, 25);
            }

            url = siteUrl;
        }

        private void createNewWebApplications()
        {
            try
            {
                int currentPort = 80;
                var webAppNames = SampleData.GetRandomNonRepeatingValues(SampleData.WebApplications, WorkingDefinition.CreateNewWebApplications);
                for (int a = 0; a < WorkingDefinition.CreateNewWebApplications; a++)
                {                 
                    SPWebApplication newApplication;
                    SPWebApplicationBuilder webAppBuilder = new SPWebApplicationBuilder(SPFarm.Local);

                    currentPort = Utils.GetNextAvailablePort(currentPort + 1);

                    var webApplicationName = string.Format("SharePoint {0} - {1}", webAppNames[a], currentPort.ToString("00"));
                    Owner.IncrementCurrentTaskProgress("Creating Web application '" + webApplicationName);
                    webAppBuilder.Port = currentPort;
                    webAppBuilder.RootDirectory = new DirectoryInfo("C:\\inetpub\\wwwroot\\wss\\" + currentPort.ToString());
                    webAppBuilder.ApplicationPoolId = string.Format("SharePoint {0} - {1} Pool", webAppNames[a], currentPort.ToString("00"));
                    webAppBuilder.IdentityType = IdentityType.SpecificUser;
                    webAppBuilder.ApplicationPoolUsername = WorkingDefinition.WebAppOwnerLogin;
                    webAppBuilder.ApplicationPoolPassword = Utils.StringToSecureString(WorkingDefinition.WebAppOwnerPassword);


                    webAppBuilder.ServerComment = webApplicationName;
                    webAppBuilder.CreateNewDatabase = true;
                    webAppBuilder.DatabaseServer = WorkingDefinition.DatabaseServer; // DB server name
                    webAppBuilder.DatabaseName = string.Format("WSS_Content_{0}_{1}", webAppNames[a], currentPort.ToString("00"));// DB Name
                    //webAppBuilder.HostHeader = "SPDG" + appNumer.ToString("00") + ".com"; //if any 

                    webAppBuilder.UseNTLMExclusively = true;  // authentication provider for NTLM
                    webAppBuilder.AllowAnonymousAccess = true; // anonymous access permission

                    // Finally create web application
                    newApplication = webAppBuilder.Create();

                    //Enable Claims
                    newApplication.UseClaimsAuthentication = true;
                    newApplication.Update();
                    newApplication.Provision();
                    WorkingDefinition.UseExistingWebApplication = newApplication.Id.ToString();
                    WorkingDefinition.UseExistingWebApplicationName = webApplicationName;

                    // Resume: calculate the number of site collections left
                    int numSiteCollections = WorkingDefinition.CreateNewSiteCollections - Owner.WorkingSiteCollections.Count;
                    for (int s = 0; s < numSiteCollections; s++)
                    {
                        CreateSiteCollection(newApplication);
                    }

                    CreateMySites(newApplication);
                }
            }
            catch (Exception ex)
            {
                Errors.Log(ex);
            }
        }
    }
}
