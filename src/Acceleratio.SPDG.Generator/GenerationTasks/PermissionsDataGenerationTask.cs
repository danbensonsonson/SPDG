using System;
using System.Collections.Generic;
using System.Linq;
using Acceleratio.SPDG.Generator.SPModel;
using Acceleratio.SPDG.Generator.Structures;

namespace Acceleratio.SPDG.Generator.GenerationTasks
{
    public abstract class PermissionsDataGenerationTaskBase : DataGenerationTaskBase
    {
        int _permissionsPerObject = 1;
        int _permissionsPerObjectDelete = 0;
        List<SPDGUser> _siteSpUsers = null;
        List<SPDGGroup> _siteSpGroups = null;
        List<SPDGUser> _siteAdGroupSpUsers = null;
        SPDGSite _currentSiteCollection = null;

        // AD
        List<string> _adUsers = null;
        List<string> _adGroups = null;

        List<SiteInfo> _allSitesToHaveUniquePermissions = new List<SiteInfo>();
        List<ListInfo> _allListsToHavehUniquePermissions = new List<ListInfo>();
        List<FolderInfo> _allFoldersToHaveUniquePermissions = new List<FolderInfo>();

        private bool _doSitePermissions;
        private bool _doListPermissions;
        bool _doListItemPermissions;
        private bool _dofolderPermissions;
        private int _totalSteps;
        private int _stepCompleted = 0;
        public PermissionsDataGenerationTaskBase(IDataGenerationTaskOwner owner) : base(owner)
        {
        }

        public override string Title
        {
            get { return "Permissions"; }
        }


        private void Init()
        {
            var allSites = Owner.WorkingSiteCollections.SelectMany(x => x.Sites).ToList();
            var allLists = Owner.WorkingSiteCollections.SelectMany(x => x.Sites.SelectMany(y => y.Lists)).ToList();
            var allFolders = Owner.WorkingSiteCollections.SelectMany(x => x.Sites.SelectMany(y => y.Lists.SelectMany(z => z.Folders))).ToList();
            _permissionsPerObject = Owner.WorkingDefinition.PermissionsPerObject;
            _permissionsPerObjectDelete = Owner.WorkingDefinition.PermissionsPerObjectDelete;

            SetSitesToHaveUniquePermissions(allSites);
            SetListsToHaveUniquePermissions(allLists);
            SetFoldersToHaveUniquePermissions(allFolders);

            var allItemsCount = allLists.Sum(x => x.ItemCount); // This doesn't work in RESUME using Client APIs
            if (WorkingDefinition.Mode == DataGeneratorMode.Incremental || WorkingDefinition.Mode == DataGeneratorMode.Resume)
                allItemsCount += 1; // during an incremental run, we don't know how many items we have

            _doSitePermissions = WorkingDefinition.PermissionsPercentOfSites > 0 && allSites.Count > 0;
            _doListPermissions = WorkingDefinition.PermissionsPercentOfLists > 0 && allLists.Count > 0;
            _doListItemPermissions = WorkingDefinition.PermissionsPercentOfListItems > 0 && allItemsCount > 0;
            _dofolderPermissions = WorkingDefinition.PermissionsPercentOfFolders > 0 && allFolders.Count > 0;
            bool stuffTodo = _doSitePermissions
                             || _doListPermissions
                             || _doListItemPermissions
                             || _dofolderPermissions;
            if (!stuffTodo)
            {
                return;
            }

            if (Owner.WorkingUsers.Count > 0 && WorkingDefinition.Mode != DataGeneratorMode.Incremental) // This happens if users/groups OR mysites are created, but saves some time
                _adUsers = Owner.WorkingUsers;
            else
                _adUsers = GetAvailableUsersInDirectory();

            if (Owner.WorkingGroups.Count > 0 && WorkingDefinition.Mode != DataGeneratorMode.Incremental)
                _adGroups = Owner.WorkingGroups;
            else
                _adGroups = GetAvailableGroupsInDirectory();

            _totalSteps =
                //user ensure
                Owner.WorkingSiteCollections.Count + _allSitesToHaveUniquePermissions.Count;
            //enum + eventual permissions change
            //+ allSites.Count + _allSitesToHaveUniquePermissions.Count;
            if (_doListItemPermissions || _doListPermissions || _dofolderPermissions)
            {
                //enum + eventual permissions change
                //_totalSteps += allLists.Count + _allListsToHavehUniquePermissions.Count;
                _totalSteps += _allListsToHavehUniquePermissions.Count;
            }
            if (_dofolderPermissions)
            {
                //enum + eventual permissions change
                // _totalSteps += allFolders.Count + _allFoldersToHaveUniquePermissions.Count;
                _totalSteps += _allFoldersToHaveUniquePermissions.Count;
            }
            if (_doListItemPermissions)
            {
                var withUnique = (allItemsCount * WorkingDefinition.PermissionsPercentOfListItems) / 100;
                //enum + eventual permissions change
                if (WorkingDefinition.Mode == DataGeneratorMode.Incremental)
                    _totalSteps += allLists.Count; // In incremental mode, we don't know how many items, to the total work will be at the list level. It has to go through each list to determine which items to permission
                else
                    _totalSteps += allItemsCount + withUnique;
            }
        }

        private void SetSitesToHaveUniquePermissions(List<SiteInfo> allSites)
        {
            // Pre-process sites for permissions
            int numSitesWithUniquePermissions = 0;
            foreach (var site in allSites)
            {
                if (site.HasUniqueRoleAssignments)
                {
                    _allSitesToHaveUniquePermissions.Add(site); // add to the list to work on in case there are more permissions per object or something
                    numSitesWithUniquePermissions++;
                }
            }
            // If we are not above the total percentage for sites, attempt to add more
            float percentageOfSitesWithPermissions = ((float)numSitesWithUniquePermissions / (float)allSites.Count) * 100;
            if (percentageOfSitesWithPermissions <= WorkingDefinition.PermissionsPercentOfSites)
            {
                foreach (var site in allSites)
                {
                    if (site.HasUniqueRoleAssignments) // only work on the sites without permissions already
                        continue;

                    if (SampleData.GetRandomNumber(1, 100) <= WorkingDefinition.PermissionsPercentOfSites)
                    {
                        _allSitesToHaveUniquePermissions.Add(site);
                        numSitesWithUniquePermissions++;
                        percentageOfSitesWithPermissions = ((float)numSitesWithUniquePermissions / (float)allSites.Count) * 100;
                        if (percentageOfSitesWithPermissions <= WorkingDefinition.PermissionsPercentOfSites)
                            break;
                    }
                }
            }
            else
                Log.Write("Sites already have " + percentageOfSitesWithPermissions + " percent unique permissions");
        }

        private void SetFoldersToHaveUniquePermissions(List<FolderInfo> allFolders)
        {
            // Pre-process Folder for permissions
            int numFoldersWithUniquePermissions = 0;
            foreach (var folder in allFolders)
            {
                if (folder.HasUniqueRoleAssignments)
                {
                    _allFoldersToHaveUniquePermissions.Add(folder); // add to the folder to work on in case there are more permissions per object or something
                    numFoldersWithUniquePermissions++;
                }
            }

            // If we are not above the total percentage for folders, attempt to add more
            float percentageOfFoldersWithPermissions = ((float)numFoldersWithUniquePermissions / (float)allFolders.Count) * 100;
            if (percentageOfFoldersWithPermissions <= WorkingDefinition.PermissionsPercentOfFolders)
            {
                foreach (var folder in allFolders)
                {
                    if (folder.HasUniqueRoleAssignments) // only work on the folder without permissions already
                        continue;
                    if (SampleData.GetRandomNumber(1, 100) <= WorkingDefinition.PermissionsPercentOfFolders)
                    {
                        _allFoldersToHaveUniquePermissions.Add(folder);
                        numFoldersWithUniquePermissions++;
                        percentageOfFoldersWithPermissions = ((float)numFoldersWithUniquePermissions / (float)allFolders.Count) * 100;
                        if (percentageOfFoldersWithPermissions <= WorkingDefinition.PermissionsPercentOfFolders)
                            break;
                    }
                }
            }
            else
                Log.Write("Folders already have " + percentageOfFoldersWithPermissions + " percent unique permissions");
        }

        private void SetListsToHaveUniquePermissions(List<ListInfo> allLists)
        {
            // Pre-process Lists for permissions
            int numListsWithUniquePermissions = 0;
            foreach (var list in allLists)
            {
                if (list.HasUniqueRoleAssignments)
                {
                    _allListsToHavehUniquePermissions.Add(list); // add to the list to work on in case there are more permissions per object or something
                    numListsWithUniquePermissions++;
                }
            }

            // If we are not above the total percentage for lists, attempt to add more
            float percentageOfListsWithPermissions = ((float)numListsWithUniquePermissions / (float)allLists.Count) * 100;
            if (percentageOfListsWithPermissions <= WorkingDefinition.PermissionsPercentOfLists)
            {
                foreach (var list in allLists)
                {
                    if (list.HasUniqueRoleAssignments) // only work on the lists without permissions already
                        continue;
                    if (SampleData.GetRandomNumber(1, 100) <= WorkingDefinition.PermissionsPercentOfLists)
                    {
                        _allListsToHavehUniquePermissions.Add(list);
                        numListsWithUniquePermissions++;
                        // TODO this logic on the first pass might not work...
                        percentageOfListsWithPermissions = ((float)numListsWithUniquePermissions / (float)allLists.Count) * 100;
                        if (percentageOfListsWithPermissions <= WorkingDefinition.PermissionsPercentOfLists)
                            break;
                    }
                }
            }
            else
                Log.Write("Lists already have " + percentageOfListsWithPermissions + " percent unique permissions");
        }

        public override int CalculateTotalSteps()
        {
            if (_totalSteps == 0)
            {
                Init();
            }
            return _totalSteps;
        }


        public override void Execute()
        {
            Log.Write("Total Permissions Steps: " + _totalSteps);
            foreach (SiteCollInfo siteCollInfo in Owner.WorkingSiteCollections)
            {
                using (var siteColl = Owner.ObjectsFactory.GetSite(siteCollInfo.URL))
                {
                    _siteSpUsers = new List<SPDGUser>();
                    _siteAdGroupSpUsers = new List<SPDGUser>();
                    _siteSpGroups = new List<SPDGGroup>();
                    _currentSiteCollection = siteColl;

                    Owner.IncrementCurrentTaskProgress("Ensuring users and groups on '" + siteCollInfo.URL + "'");
                    if (WorkingDefinition.PermissionsPercentForSPGroups > 0)
                        EnsureUsersAndGroups(siteColl.RootWeb); //Need some of this for the creation of SP groups. Random AD user/group selection/ensure will occur on demand. 

                    //setSitePermissions(siteColl.RootWeb); // Don't think adding permissions to the root site is necessary
                    _stepCompleted++;
                    Log.Write("Permissions Steps Completed: " + _stepCompleted + " of " + _totalSteps);
                    foreach (SiteInfo siteInfo in siteCollInfo.Sites)
                    {
                        using (var web = siteColl.OpenWeb(siteInfo.ID))
                        {
                            Owner.IncrementCurrentTaskProgress("Crawling site", 0);
                            if (_allSitesToHaveUniquePermissions.Contains(siteInfo))
                            {
                                setSitePermissions(web);
                                _stepCompleted++;
                                Log.Write("Permissions Steps Completed: " + _stepCompleted + " of " + _totalSteps);
                            }

                            if (_doListPermissions || _doListItemPermissions || _dofolderPermissions)
                            {
                                foreach (ListInfo listInfo in siteInfo.Lists)
                                {
                                    Owner.IncrementCurrentTaskProgress("Crawling list " + web.Url + "/" + listInfo.Name, 0);
                                    if (_allListsToHavehUniquePermissions.Contains(listInfo))
                                    {
                                        setListPermissions(web, listInfo.Name);
                                        _stepCompleted++;
                                        Log.Write("Permissions Steps Completed: " + _stepCompleted + " of " + _totalSteps);
                                    }

                                    if (_dofolderPermissions)
                                    {
                                        foreach (FolderInfo folderInfo in listInfo.Folders)
                                        {
                                            if (_allFoldersToHaveUniquePermissions.Contains(folderInfo))
                                            {
                                                setFolderPermissions(web, folderInfo.URL);
                                                _stepCompleted++;
                                                Log.Write("Permissions Steps Completed: " + _stepCompleted + " of " + _totalSteps);
                                            }
                                            Owner.IncrementCurrentTaskProgress("Crawled folder " + web.Url + "/" + listInfo.Name);
                                        }
                                    }

                                    if (_doListItemPermissions)
                                    {
                                        setItemPermissions(web, listInfo.Name);
                                        Log.Write("Unique permissions changed in list: " + listInfo.Name + " in site: " + web.Url);
                                        _stepCompleted++;
                                        Log.Write("Permissions Steps Completed: " + _stepCompleted + " of " + _totalSteps);
                                    }
                                    Owner.IncrementCurrentTaskProgress("Crawled list " + web.Url + "/" + listInfo.Name);
                                }
                            }

                            Owner.IncrementCurrentTaskProgress("Crawled site " + web.Url);
                        }
                    }
                }
            }

        }

        protected abstract List<string> GetAvailableUsersInDirectory();
        protected abstract List<string> GetAvailableGroupsInDirectory();

        private void EnsureUsersAndGroups(SPDGWeb web)
        {
            /// ENSURE AD USERS TO SHAREPOINT
            /// INCREMENTAL: If the correct number of users/groups have already been ensured and we have SP groups create, shuffle which users are in the groups
            // need some users here to fill created SP Groups
            _siteSpUsers = web.SiteUsers.Where(x => !x.IsDomainGroup).ToList();
            _siteAdGroupSpUsers = web.SiteUsers.Where(x => x.IsDomainGroup).ToList();
            int numOfUsers = _adUsers.Count > 20 ? 20 : _adUsers.Count;
            numOfUsers = numOfUsers - _siteSpUsers.Count; // SPDG Users already created in a previous run
            //int numOfUsers = _adUsers.Count;
            for (int i = 0; i < numOfUsers; i++)
            {
                string userName = _adUsers[SampleData.GetRandomNumber(0, _adUsers.Count)];

                if (userName == string.Empty) continue;

                try
                {
                    var user = web.EnsureUser(userName);
                    Log.Write("Ensured user:" + userName + " for site: " + web.Url);
                }
                catch (Exception ex)
                {
                    Errors.Log(ex);
                    Log.Write("Error adding user:" + userName);
                }
            }

            /// ENSURE AD GROUPS TO SHAREPOINT
            int numOfGroups = _adUsers.Count > 20 ? 20 : _adUsers.Count;
            numOfGroups = numOfGroups - _siteAdGroupSpUsers.Count; // SPDG Users/Groups already created in a previous run
            //numOfUsers = _adGroups.Count;
            for (int i = 0; i < numOfGroups; i++)
            {
                string groupName = _adGroups[SampleData.GetRandomNumber(0, _adGroups.Count)];

                if (groupName == string.Empty) continue;

                try
                {
                    var user = web.EnsureUser(groupName);
                    Log.Write("Ensured group:" + groupName + " for site: " + web.Url);
                }
                catch (Exception ex)
                {
                    Errors.Log(ex);
                    Log.Write("Error adding group:" + groupName);
                }
            }

            HashSet<string> createdGroups = new HashSet<string>();
            /// Modify existing SharePoint Groups
            int numOfSPGroups = 10;
            numOfSPGroups = numOfSPGroups - (web.SiteGroups.Count() -1); // -1 for the built in excel services group
            /// CREATE SHAREPOINT GROUPS            
            if (WorkingDefinition.PermissionsPercentForSPGroups > 0)
            {
                for (int i = 0; i < numOfSPGroups; i++)
                {
                    string nameCandidate = SampleData.GetSampleValueRandom(SampleData.Accounts) + " Group";
                    try
                    {
                        web.AddSiteGroup(nameCandidate,
                        web.CurrentUser,
                        web.CurrentUser,
                        "SPDG generated group");
                        createdGroups.Add(nameCandidate);
                        Log.Write("Created SP group:" + nameCandidate);
                    }
                    catch (Exception ex)
                    {
                        Errors.Log(ex);
                    }
                }
            }
            _siteSpGroups = web.SiteGroups.ToList();
            _siteSpUsers = web.SiteUsers.Where(x => !x.IsDomainGroup).ToList();
            _siteAdGroupSpUsers = web.SiteUsers.Where(x => x.IsDomainGroup).ToList();

            // Add/Modify users/groups to SP Groups 
            // Incremental: Remove 1 member and add 1
            int membersPerSPGroup = 10;
            int membersToDelete = WorkingDefinition.IncrementalUpdateSPGroupMembership;
            //if (_siteSpGroups.Count > 0 && membersToDelete > 0)
            if (_siteSpGroups.Count > 0)
            {
                var candidates = _siteSpUsers.Union(_siteAdGroupSpUsers).ToList();
                foreach (var grp in _siteSpGroups)
                {
                    try
                    {
                        int membersToAdd = membersPerSPGroup;
                        int usersInGroup = grp.Users.Count();
                        if (usersInGroup >= membersPerSPGroup)
                        {
                            grp.RemoveUsers(membersToDelete);
                            membersToAdd = membersToDelete; // Removing, then adding the same amount during incremental updates
                            Log.Write(string.Format("Removing {0} memeber(s) from SharePoint group {1}:", membersToDelete, grp.Name));
                        }
                        candidates.Shuffle();
                        var elementsToTake = Math.Min(candidates.Count, membersToAdd);
                        var users = candidates.Take(elementsToTake).ToList();
                        //var grp = _siteSpGroups.First(x => x.Name == @group);
                        grp.AddUsers(users);
                        Log.Write(string.Format("Added {0} members to SharePoint group {1}:", users.Count, grp.Name));
                    }
                    catch (Exception ex)
                    {
                        Errors.Log(ex);
                    }
                }
            }

        }

        private void setSitePermissions(SPDGWeb web)
        {

            int roleAssignments = web.NumUniqueRoleAssignments;
            int permissions = SampleData.GetRandomNumber(1, _permissionsPerObject);
            // delete
            if (roleAssignments >= _permissionsPerObjectDelete && web.HasUniqueRoleAssignments)
            {
                int permissionsToDelete = _permissionsPerObjectDelete;
                if (roleAssignments > permissions)
                    permissionsToDelete = roleAssignments - permissions; //bring the total permissions back in line with the configuration

                for (int i = 0; i < permissionsToDelete; i++)
                {
                    // delete permissions (removes a random permission
                    Owner.IncrementCurrentTaskProgress("Removing permission from site " + web.Url, 0);
                    web.RemoveRoleAssignment();
                    roleAssignments--;
                }
            }
            // add
            if (roleAssignments >= permissions && web.HasUniqueRoleAssignments)
            {
                Log.Write("Existing Site '" + web.Url + " already has " + roleAssignments + " permissions of " + permissions);
                return;
            }
            Owner.IncrementCurrentTaskProgress("Adding permissions to site " + web.Url + "'", 0);
            try
            {
                web.BreakRoleInheritance(false);

                for (int i = 0; i < permissions; i++)
                {                
                    setupNextRoleAssignment(web, web);
                    roleAssignments++;
                    if (roleAssignments >= permissions)
                        break;
                }
            }
            catch (Exception ex)
            {
                Errors.Log(ex);
                return;
            }
            Log.Write(permissions + "  unique permissions added to site: " + web.Url);
        }

        private void setListPermissions(SPDGWeb web, string listName)
        {
            var list = web.GetList(listName);
            int permissions = SampleData.GetRandomNumber(1, _permissionsPerObject);

            int roleAssignments = list.NumUniqueRoleAssignments;
            // delete
            if (roleAssignments >= _permissionsPerObjectDelete && list.HasUniqueRoleAssignments)
            {
                int permissionsToDelete = _permissionsPerObjectDelete;
                if (roleAssignments > permissions)
                    permissionsToDelete = roleAssignments - permissions; //bring the total permissions back in line with the configuration

                for (int i = 0; i < permissionsToDelete; i++)
                {
                    // delete permissions
                    Owner.IncrementCurrentTaskProgress("Removing permission from list " + web.Url + "/" + listName, 0);
                    list.RemoveRoleAssignment();                    
                    roleAssignments--;
                }
            }
            // add
            if (roleAssignments >= permissions && list.HasUniqueRoleAssignments)
            {
                Log.Write("Existing List '" + web.Url + "/" + listName + " already has " + roleAssignments + " permissions of " + permissions);
                return;
            }

            Owner.IncrementCurrentTaskProgress("Adding permissions to list " + web.Url + "/" + listName, 0);
            list.BreakRoleInheritance(false);            

            for (int i = 0; i < permissions; i++)
            {
                setupNextRoleAssignment(web, list);
                roleAssignments++;
                if (roleAssignments >= permissions)
                    break;
            }

            Owner.IncrementCurrentTaskProgress(permissions + " + unique permissions added to list: " + listName + ", site: " + web.Url);
        }

        private void setFolderPermissions(SPDGWeb web, string folderURL)
        {
            SPDGFolder folder = web.GetFolder(folderURL);
            if (folder == null)
                return;

            int permissions = SampleData.GetRandomNumber(1, _permissionsPerObject);
            int roleAssignments = folder.Item.NumUniqueRoleAssignments;
            // delete
            if (roleAssignments >= _permissionsPerObjectDelete && folder.Item.HasUniqueRoleAssignments)
            {
                int permissionsToDelete = _permissionsPerObjectDelete;
                if (roleAssignments > permissions)
                    permissionsToDelete = roleAssignments - permissions; //bring the total permissions back in line with the configuration

                for (int i = 0; i < permissionsToDelete; i++)
                {
                    // delete permissions
                    folder.Item.RemoveRoleAssignment();
                    Owner.IncrementCurrentTaskProgress("Removing permission from folder: " + folderURL + " in site: " + web.Url, 0);
                    roleAssignments--;
                }
            }
            // add
            if (roleAssignments >= permissions && folder.Item.HasUniqueRoleAssignments)
            {
                Log.Write("Existing Folder '" + folderURL + " in site: " + web.Url + " already has " + roleAssignments + " permissions of " + permissions);
                return;
            }

            Owner.IncrementCurrentTaskProgress("Adding permissions to folder: " + folderURL + " in site: " + web.Url, 0);
            folder.Item.BreakRoleInheritance(false);

            for (int i = 0; i < permissions; i++)
            {
                setupNextRoleAssignment(web, folder.Item);
                roleAssignments++;
                if (roleAssignments >= permissions)
                    break;
            }

            Owner.IncrementCurrentTaskProgress(permissions + " unique permissions added to folder: " + folderURL + " in site: " + web.Url);
        }

        private void setItemPermissions(SPDGWeb web, string listName)
        {
            var list = web.GetList(listName);
            int permissions = SampleData.GetRandomNumber(1, _permissionsPerObject);

            List<SPDGListItem> itemsToHavehUniquePermissions = new List<SPDGListItem>();
            // Pre-process Items for permissions
            int numItemsWithUniquePermissions = 0;
            int totalItems = 0;
            foreach (var item in list.Items)
            {
                totalItems++;
                if (item.HasUniqueRoleAssignments)
                {
                    itemsToHavehUniquePermissions.Add(item); 
                    numItemsWithUniquePermissions++;
                }
            }
            float percentageOfItemssWithPermissions = ((float)numItemsWithUniquePermissions / totalItems) * 100;

            if (percentageOfItemssWithPermissions >= WorkingDefinition.PermissionsPercentOfListItems)
            {
                Log.Write("Items in List: " + list.Title + " already have " + percentageOfItemssWithPermissions + " percent unique permissions");

                if (_permissionsPerObject < 1) // double check
                    return;
                foreach (var item in itemsToHavehUniquePermissions)
                {
                    permissions = SampleData.GetRandomNumber(1, _permissionsPerObject);
                    int roleAssignments = item.NumUniqueRoleAssignments;
                    Owner.IncrementCurrentTaskProgress("Working item/document '" + item.DisplayName + "' in list '" + list.Title + " has " + roleAssignments + " permissions", 0);
                    // delete
                    if (roleAssignments >= _permissionsPerObjectDelete)
                    {
                        for (int i = 0; i < _permissionsPerObjectDelete; i++)
                        {
                            // delete permissions
                            Owner.IncrementCurrentTaskProgress("Removing permission from item/document '" + item.DisplayName + "' in list '" + list.Title, 0);
                            item.RemoveRoleAssignment();
                            roleAssignments--;
                        }
                    }
                    // add      
                    if (roleAssignments >= permissions)
                    {
                        Log.Write("Existing item/document '" + item.DisplayName + "' in list '" + list.Title + " already has " + roleAssignments + " permissions of " + permissions);
                        continue;
                    }

                    Owner.IncrementCurrentTaskProgress("Adding permissions for existing item/document '" + item.DisplayName + "' in list '" + list.Title, 0);
                    for (int i = 0; i < permissions; i++)
                    {
                        setupNextRoleAssignment(web, item);
                        roleAssignments++;
                        if (roleAssignments >= permissions)
                            break;                        
                    }
                    Owner.IncrementCurrentTaskProgress(permissions + " unique permissions added for existing item/document '" + item.DisplayName + "' in list '" + list.Title);
                    shareItem(list, item);
                }
                return;
            }

            // add new permissions
            foreach (var item in list.Items)
            {
                permissions = SampleData.GetRandomNumber(1, _permissionsPerObject);
                if (SampleData.GetRandomNumber(1, 100) < WorkingDefinition.PermissionsPercentOfListItems)
                {
                    Owner.IncrementCurrentTaskProgress("Adding permissions for item/document '" + item.DisplayName + "' in list '" + list.Title, 0);
                    item.BreakRoleInheritance(false);

                    for (int i = 0; i < permissions; i++)
                    {
                        setupNextRoleAssignment(web, item);
                    }
                    Owner.IncrementCurrentTaskProgress(permissions + " unique permissions added for item/document '" + item.DisplayName + "' in list '" + list.Title);
                    shareItem(list, item);
                    numItemsWithUniquePermissions++;
                    percentageOfItemssWithPermissions = ((float)numItemsWithUniquePermissions / totalItems) * 100;
                    if (percentageOfItemssWithPermissions >= WorkingDefinition.PermissionsPercentOfListItems)
                        break;
                }
                else
                {
                    Owner.IncrementCurrentTaskProgress("");
                }
            }
        }

        private void shareItem(SPDGList list, SPDGListItem item)
        {
           
            if (list.IsDocumentLibrary && item.SupportsSharing)
            {
                // this is cached for SPO and thus fast
                var users = GetAvailableUsersInDirectory();
                Owner.IncrementCurrentTaskProgress("Sharing document '" + item.DisplayName + "' in library '" + list.Title, 0);

                var emailsForView = new HashSet<string>();
                var emailsForEdit = new HashSet<string>();
                for (int i = 0; i < _permissionsPerObject; i++)
                {
                    var emailCollection = emailsForView;
                    if (SampleData.GetRandomNumber(0, 100) < 30)
                    {
                        emailCollection = emailsForEdit;
                    }

                    if (SampleData.GetRandomNumber(0, 100) < 20)
                    {
                        // anonymous
                        emailCollection.Add("");
                    }
                    else if (SampleData.GetRandomNumber(1, 100) < 50)
                    {
                        emailCollection.Add(users[SampleData.GetRandomNumber(0, users.Count - 1)]);
                        //emailCollection.Add(getRandomSPUser().Email);
                    }
                    else
                    {
                        emailCollection.Add(SampleData.GetRandomEmail());
                    }
                }

                item.ShareWithPeople(emailsForView, false);
                item.ShareWithPeople(emailsForEdit, true);
            }
        }


        private void setupNextRoleAssignment(SPDGWeb web, SPDGSecurableObject securableObject)
        {
            SPDGPrincipal principal = null;
            int maxAttempts = 20;
            int i = 0;
            var rnd = SampleData.GetRandomNumber(0, 100);
            if (rnd < WorkingDefinition.PermissionsPercentForUsers)
            {
                while (principal == null)
                {
                    principal = getRandomADUser();
                    i++;
                    if (i >= maxAttempts)
                        break;
                }
            }
            else if (rnd < WorkingDefinition.PermissionsPercentForUsers + WorkingDefinition.PermissionsPercentForSPGroups)
            {
                principal = _siteSpGroups[SampleData.GetRandomNumber(0, _siteSpGroups.Count - 1)];
                Log.Write("Adding SharePoint Group: " + principal.Name);
            }
            else
            {
                while (principal == null)
                {
                    principal = getRandomADGroup();
                    i++;
                    if (i >= maxAttempts)
                        break;
                }
            }
            // TODO get rid of the excess logging
            //Log.Write("Start GetRoleAssignmentByPrincipal");
            var roleAssignment = securableObject.GetRoleAssignmentByPrincipal(principal);
            //Log.Write("End GetRoleAssignmentByPrincipal");
            if (roleAssignment == null || roleAssignment.RoleDefinitionBindings.All(x => x.IsGuestRole))
            {
                var availableRoledefinitions = web.RoleDefinitions.Where(x => !x.IsGuestRole).ToList();

                var selected = availableRoledefinitions[SampleData.GetRandomNumber(0, availableRoledefinitions.Count - 1)];
                //Log.Write("Start AddRoleAssignment");
                // TODO This line is taking 3s
                securableObject.AddRoleAssignment(principal, new List<SPDGRoleDefinition> { selected });
                Log.Write("End AddRoleAssignment");
            }
        }

        private SPDGUser getRandomADGroup()
        {
            SPDGUser group = null;
            string groupName = null;
            try
            {
                // TODO need to get the SPDGGroup from Ensure user
                groupName = _adGroups[SampleData.GetRandomNumber(0, _adGroups.Count)];
                //Log.Write("Ensuring group:" + groupName + " for site: " + _currentSiteCollection.RootWeb.Url);
                group = _currentSiteCollection.RootWeb.EnsureUser(groupName); // Ensuring groups on demand rather than the whole list to a site colleciton
                //Log.Write("Ensured group:" + group.Name + " for site: " + _currentSiteCollection.RootWeb.Url);
            }
            catch (Exception ex)
            {
                Errors.Log(ex);
                Log.Write("Error adding group:" + groupName);
            }

            return group;
        }

        private SPDGUser getRandomADUser()
        {          
            SPDGUser user = null;
            string userName = null;
            try
            {                
                do
                {
                    Log.Write("Getting random user:" + userName + " for site: " + _currentSiteCollection.RootWeb.Url);
                    userName = _adUsers[SampleData.GetRandomNumber(0, _adUsers.Count)];
                    //Log.Write("Ensuring user:" + userName + " for site: " + _currentSiteCollection.RootWeb.Url);
                    user = _currentSiteCollection.RootWeb.EnsureUser(userName); // Ensuring users on demand rather than the whole list to a site colleciton
                    //Log.Write("Ensured user:" + userName + " for site: " + _currentSiteCollection.RootWeb.Url);
                } while (user.IsGuestUser);

                
            }
            catch (Exception ex)
            {
                Errors.Log(ex);
                Log.Write("Error adding user:" + userName);
            }          

            return user;
        }
    }
}