using System;
using System.Collections.Generic;
using System.Linq;
using Acceleratio.SPDG.Generator.SPModel;
using Acceleratio.SPDG.Generator.Structures;


namespace Acceleratio.SPDG.Generator.GenerationTasks
{
    public class ListsDataGenerationTask : DataGenerationTaskBase
    {
        SPDGListTemplateType _lastTemplateType = SPDGListTemplateType.NoListTemplate;
        string _lastListPrefix = "List";
        const string OOB_Lists_Libraries = "Documents,MicroFeed,Form Templates,Composed Looks,Master Page Gallery,Site Pages,Site Assets,Style Library";

        public ListsDataGenerationTask(IDataGenerationTaskOwner owner) : base(owner)
        {
        }

        public override string Title
        {
            get { return "Lists and Libraries"; }
        }      

        public override int  CalculateTotalSteps()
        {
            int totalSteps = (WorkingDefinition.MaxNumberOfListsAndLibrariesPerSite + WorkingDefinition.NumberOfBigListsPerSite) * WorkingDefinition.NumberOfSitesToCreate;
            totalSteps = totalSteps * Owner.WorkingSiteCollections.Count;
            if (WorkingDefinition.Mode == DataGeneratorMode.Incremental)
            {
                totalSteps += 1; // TODO hack to make sure it is active
            }
            return totalSteps;
        }

    
        public override void Execute()
        {            
            foreach (SiteCollInfo siteCollInfo in Owner.WorkingSiteCollections)
            {
                using (var siteColl = Owner.ObjectsFactory.GetSite(siteCollInfo.URL))
                {
                    foreach (SiteInfo siteInfo in siteCollInfo.Sites)
                    {
                        IEnumerable<SPDGList> lists = null;
                        using (var web = siteColl.OpenWeb(siteInfo.ID)) 
                        {
                            Random rnd = new Random();
                            //int listsToCreate = rnd.Next(WorkingDefinition.MaxNumberOfListsAndLibrariesPerSite+1);
                            int listsToCreate = WorkingDefinition.MaxNumberOfListsAndLibrariesPerSite;
                            int bigListsToCreate = WorkingDefinition.NumberOfBigListsPerSite;
                            
                            listsToCreate += bigListsToCreate;
                            int bigListsCreated = 0;
                            Log.Write("Getting existing lists in site  '" + web.Url + "'");
                            lists = web.Lists;
                            // Add existing list for this site for adding items
                            int totalLists = 0;
                            foreach (var existing in lists)
                            {
                                totalLists++;
                                if (!SampleData.BusinessDocsTypes.Contains(existing.Title))
                                    continue;
                                if (existing.BaseTemplate == SPDGListTemplateType.GenericList || existing.BaseTemplate == SPDGListTemplateType.DocumentLibrary)
                                {
                                    ListInfo listInfo = new ListInfo();
                                    listInfo.Name = existing.Title;
                                    listInfo.TemplateType = existing.BaseTemplate;
                                    listInfo.isLib = existing.IsDocumentLibrary;
                                    listInfo.HasUniqueRoleAssignments = existing.HasUniqueRoleAssignments;
                                
                                    // filter out OOB lists OOB_Lists_Libraries
                                    siteInfo.Lists.Add(listInfo);
                                    Owner.IncrementCurrentTaskProgress("Getting list '" + listInfo.Name + "' in site '" + web.Url + "'" + " Type: " + listInfo.TemplateType);
                                }
                                
                            }
                            // delete                            
                            int listsToDelete = siteInfo.Lists.Count > WorkingDefinition.NumberOfListsAndLibrariesToDelete ? WorkingDefinition.NumberOfListsAndLibrariesToDelete : siteInfo.Lists.Count;
                            if (listsToDelete > 0)
                            {
                                int listsDeleted = 0;
                                Owner.IncrementCurrentTaskProgress("Deleting lists in site: " + web.Url);
                                for (int i = 0; i < totalLists; i++)
                                {
                                    if (listsDeleted >= listsToDelete)
                                        break;
                                    var list = lists.ElementAt(i);
                                    if (!SampleData.BusinessDocsTypes.Contains(list.Title))
                                        continue;
                                    string listName = list.Title;
                                    try
                                    {
                                        list.Delete();
                                        Owner.IncrementCurrentTaskProgress("Deleted list: " + listName);
                                        var remove = siteInfo.Lists.FirstOrDefault(x => x.Name == listName);
                                        siteInfo.Lists.Remove(remove);
                                        i--; // if a list was deleted, we have fewer in the lists
                                        listsDeleted++;
                                    }
                                    catch (Exception ex)
                                    {
                                        Log.Write("Failed to delete list: " + listName + " in site '" + web.Url + "' Reason: " + ex.Message);
                                        break;
                                    }
                                    
                                }
                            }

                            // Resume: if it's not incremental, only create the amount of lists that haven't already been created
                            // Incremental: create the amount of lists asked for, regardless of what's there
                            if (WorkingDefinition.Mode == DataGeneratorMode.Resume)
                                listsToCreate = listsToCreate - siteInfo.Lists.Count;

                            if (listsToCreate > 0)
                                Log.Write("Creating lists in site '" + web.Url + "'");

                            for ( int s = 0; s < listsToCreate; s++ )
                            {
                                try
                                {
                                    SPDGListTemplateType listTemplate;
                                    if (bigListsCreated >= bigListsToCreate)
                                    {
                                        getNextTemplateType();
                                        
                                        listTemplate = _lastTemplateType;
                                    }
                                    else
                                    {
                                        _lastListPrefix = "List";
                                        listTemplate = SPDGListTemplateType.GenericList;                                        
                                    }
                                    
                                    string listName = findAvailableListName(web);
                                    Guid listGuid = web.AddList(listName, string.Empty, (int)listTemplate);
                                    Owner.IncrementCurrentTaskProgress("Created List '" + listName + "' in site '" + web.Url + "'" + " Type: " + listTemplate);
                                    var list = web.GetList(listGuid);                                    
                                    web.AddNavigationNode(list.Title, list.DefaultViewUrl, NavigationNodeLocation.QuickLaunchLists);
                                    ListInfo listInfo = new ListInfo();
                                    listInfo.Name = listName;
                                    listInfo.TemplateType = listTemplate;
                                    listInfo.isLib = (listTemplate == SPDGListTemplateType.DocumentLibrary ? true : false);
                                    if (!listInfo.isLib && bigListsCreated < bigListsToCreate)
                                    {
                                        listInfo.isBigList = true;
                                        bigListsCreated++;
                                    }
                                    siteInfo.Lists.Add(listInfo);

                                    
                                }
                                catch(Exception ex )
                                {
                                    Errors.Log(ex);
                                }                                
                            }
                        }
                    }
                }
            }
        }

        private SPDGListTemplateType getNextTemplateType()
        {
            if (_lastTemplateType == SPDGListTemplateType.NoListTemplate)
            {
                if( WorkingDefinition.LibTypeList)
                {
                    _lastTemplateType = SPDGListTemplateType.GenericList;
                    _lastListPrefix = "List";
                    return _lastTemplateType;
                }
                else if (WorkingDefinition.LibTypeDocument)
                {
                    _lastTemplateType = SPDGListTemplateType.DocumentLibrary;
                    _lastListPrefix = "Library";
                    return _lastTemplateType;
                }
                else if (WorkingDefinition.LibTypeTasks)
                {
                    _lastTemplateType = SPDGListTemplateType.Tasks;
                    _lastListPrefix = "Tasks";
                    return _lastTemplateType;
                }
                else if (WorkingDefinition.LibTypeCalendar)
                {
                    _lastTemplateType = SPDGListTemplateType.Events;
                    _lastListPrefix = "Events";
                    return _lastTemplateType ;
                }
            }

            if (_lastTemplateType == SPDGListTemplateType.GenericList)
            {
                if (WorkingDefinition.LibTypeDocument)
                {
                    _lastTemplateType = SPDGListTemplateType.DocumentLibrary;
                    _lastListPrefix = "Library";
                    return _lastTemplateType;
                }
                else if (WorkingDefinition.LibTypeTasks)
                {
                    _lastTemplateType = SPDGListTemplateType.Tasks;
                    _lastListPrefix = "Tasks";
                    return _lastTemplateType;
                }
                else if (WorkingDefinition.LibTypeCalendar)
                {
                    _lastTemplateType = SPDGListTemplateType.Events;
                    _lastListPrefix = "Events";
                    return _lastTemplateType;
                }
            }

            if (_lastTemplateType == SPDGListTemplateType.DocumentLibrary)
            {
                if (WorkingDefinition.LibTypeTasks)
                {
                    _lastTemplateType = SPDGListTemplateType.Tasks;
                    _lastListPrefix = "Tasks";
                    return _lastTemplateType;
                }
                else if (WorkingDefinition.LibTypeCalendar)
                {
                    _lastTemplateType = SPDGListTemplateType.Events;
                    _lastListPrefix = "Events";
                    return _lastTemplateType;
                }
                else if (WorkingDefinition.LibTypeList)
                {
                    _lastTemplateType = SPDGListTemplateType.GenericList;
                    _lastListPrefix = "List";
                    return _lastTemplateType;
                }
            }

            if (_lastTemplateType == SPDGListTemplateType.Tasks)
            {
                if (WorkingDefinition.LibTypeCalendar)
                {
                    _lastTemplateType = SPDGListTemplateType.Events;
                    _lastListPrefix = "Events";
                    return _lastTemplateType;
                }
                else if (WorkingDefinition.LibTypeList)
                {
                    _lastTemplateType = SPDGListTemplateType.GenericList;
                    _lastListPrefix = "List";
                    return _lastTemplateType;
                }
                else if (WorkingDefinition.LibTypeDocument)
                {
                    _lastTemplateType = SPDGListTemplateType.DocumentLibrary;
                    _lastListPrefix = "Library";
                    return _lastTemplateType;
                }
            }

            if (_lastTemplateType == SPDGListTemplateType.Events)
            {
                if (WorkingDefinition.LibTypeList)
                {
                    _lastTemplateType = SPDGListTemplateType.GenericList;
                    _lastListPrefix = "List";
                    return _lastTemplateType;
                }
                else if (WorkingDefinition.LibTypeDocument)
                {
                    _lastTemplateType = SPDGListTemplateType.DocumentLibrary;
                    _lastListPrefix = "Library";
                    return _lastTemplateType;
                }
                else if (WorkingDefinition.LibTypeTasks)
                {
                    _lastTemplateType = SPDGListTemplateType.Tasks;
                    _lastListPrefix = "Tasks";
                    return _lastTemplateType;
                }
            }

            return _lastTemplateType;
        }

        private string findAvailableListName(SPDGWeb web)
        {
            string candidate = SampleData.GetSampleValueRandom(SampleData.BusinessDocsTypes);

            if (_lastTemplateType == SPDGListTemplateType.Tasks)
            {
                candidate += " Tasks";
            }
            else if (_lastTemplateType == SPDGListTemplateType.Events)
            {
                candidate += " Calendar";
            }

            int i = 0;
            while (web.TryGetList(candidate) != null)
            {
                candidate = SampleData.GetSampleValueRandom(SampleData.BusinessDocsTypes); //TODO I think a site with more than 35 lists is getting stuck here.
                i++;
                if (i > SampleData.BusinessDocsTypes.Count) // TODO add a number or something to keep moving
                {
                    candidate = candidate + " " + i;
                    break;
                }
            }

            return candidate;
        }
    }
}
