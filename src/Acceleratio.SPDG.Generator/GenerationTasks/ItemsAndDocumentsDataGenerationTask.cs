using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Acceleratio.SPDG.Generator.SPModel;
using Acceleratio.SPDG.Generator.Structures;

namespace Acceleratio.SPDG.Generator.GenerationTasks
{
    public class ItemsAndDocumentsDataGenerationTask : DataGenerationTaskBase
    {
        private int _docsAdded = 0;
        private string _currentFileType = null;
        private int _totalItemsAdded = 0;
        private int _totalItemsDeleted = 0;


        public ItemsAndDocumentsDataGenerationTask(IDataGenerationTaskOwner owner) : base(owner)
        {
        }

        public override string Title
        {
            get { return "Items and Documents"; }
        }

        public override int CalculateTotalSteps()
        {
            var totalSteps  = WorkingDefinition.NumberOfSitesToCreate *
                              (WorkingDefinition.MaxNumberOfListsAndLibrariesPerSite *
                          (WorkingDefinition.MaxNumberofItemsToGenerate + WorkingDefinition.MaxNumberofDocumentLibraryItemsToGenerate
                          + WorkingDefinition.NumberofItemsToDelete + WorkingDefinition.NumberofDocumentLibraryItemsToDelete)
                          + WorkingDefinition.NumberOfBigListsPerSite * WorkingDefinition.MaxNumberofItemsBigListToGenerate);
            totalSteps = totalSteps*Owner.WorkingSiteCollections.Count;
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
                        using (var web = siteColl.OpenWeb(siteInfo.ID))
                        {
                            var available = siteInfo.Lists;
                            //shuffle because of big lists
                            available.Shuffle();
                            foreach (ListInfo listInfo in available)
                            {
                                _titleUsage.Clear();
                                if (!listInfo.isLib)
                                {
                                    var list = web.GetList(listInfo.Name);
                                    int itemCount = listInfo.isBigList ? WorkingDefinition.MaxNumberofItemsBigListToGenerate : WorkingDefinition.MaxNumberofItemsToGenerate;
                                    // delete
                                    if (WorkingDefinition.NumberofItemsToDelete > 0)
                                    {
                                        Owner.IncrementCurrentTaskProgress("Start Deleting items from list: " + listInfo.Name + " in site: " + web.Url, 0);
                                        _totalItemsDeleted += list.DeleteItems(WorkingDefinition.NumberofItemsToDelete);                                        
                                    }

                                    if (itemCount < 1) // No items to add
                                        continue;

                                    if( listInfo.TemplateType == SPDGListTemplateType.Tasks )
                                    {
                                        Owner.IncrementCurrentTaskProgress("Start adding items to tasks list: " + listInfo.Name + " in site: " + web.Url,0);
                                    }
                                    else if (listInfo.TemplateType == SPDGListTemplateType.Events)
                                    {
                                        Owner.IncrementCurrentTaskProgress("Start adding events to calendar: " + listInfo.Name + " in site: " + web.Url,0);
                                    }
                                    else
                                    {
                                        Owner.IncrementCurrentTaskProgress("Start adding items to list: " + listInfo.Name + " in site: " + web.Url,0);
                                    }
                                    
                                    List<ISPDGListItemInfo> batch=new List<ISPDGListItemInfo>();                                    
                                    //itemCount = SampleData.GetRandomNumber(itemCount*3/4, itemCount);
                                    for (int i = 0; i < itemCount; i++ )
                                    {
                                        var itemInfo=new SPDGListItemInfo();
                                        if (listInfo.TemplateType == SPDGListTemplateType.Tasks)
                                        {
                                            populateTask(itemInfo);
                                        }
                                        else if (listInfo.TemplateType == SPDGListTemplateType.Events)
                                        {
                                            populateEvent(itemInfo);
                                        }
                                        else
                                        {
                                            // TODO: Add the 10% attachments to the wizard as configurable
                                            // if itemCount % 10 = 0, add attachment to list item.
                                            if (i % 10 == 0)                                         
                                                populateItemInfo(list, itemInfo, false, true);
                                            else
                                                populateItemInfo(list, itemInfo, false);                               
                                            
                                        }
                                        batch.Add(itemInfo);

                                        if (batch.Count > 400)
                                        {
                                            list.AddItems(batch);
                                            Owner.IncrementCurrentTaskProgress(string.Format("Created {0}/{1} items for list {2}: ", i + 1, itemCount, list.RootFolder.Url), batch.Count);
                                            _totalItemsAdded += batch.Count;
                                            batch.Clear();                                            
                                        }
                                    }
                                    if (batch.Count > 0) // Pick up anything at the end that was less than 400
                                    {
                                        list.AddItems(batch);
                                        Owner.IncrementCurrentTaskProgress(string.Format("Created {0} items for list {1}: ", itemCount, list.RootFolder.Url), batch.Count);
                                        _totalItemsAdded += batch.Count;
                                        batch.Clear();
                                    }
                                    listInfo.ItemCount = itemCount;
                                }
                                else
                                {
                                    _docsAdded = 0;
                                    var list = web.GetList(listInfo.Name);
                                    if (WorkingDefinition.NumberofDocumentLibraryItemsToDelete > 0)
                                    {
                                        Owner.IncrementCurrentTaskProgress("Start Deleting documents from library: " + listInfo.Name + " in site: " + web.Url, 0);
                                        _totalItemsDeleted += list.DeleteItems(WorkingDefinition.NumberofDocumentLibraryItemsToDelete);
                                        _totalItemsDeleted += WorkingDefinition.NumberofDocumentLibraryItemsToDelete;
                                    }
                                    
                                    if (WorkingDefinition.MaxNumberofDocumentLibraryItemsToGenerate > 0)
                                        Owner.IncrementCurrentTaskProgress("Start adding documents to library: " + listInfo.Name + " in site: " + web.Url,0);
                                                                       
                                    while (_docsAdded < WorkingDefinition.MaxNumberofDocumentLibraryItemsToGenerate)
                                    {
                                        addDocumentToFolder(list,list.RootFolder);
                                    }
                                    listInfo.ItemCount = _docsAdded;
                                    _totalItemsAdded += _docsAdded;
                                }
                            }
                            //Log.Write("Total Items Deleted: " + _totalItemsDeleted);
                            //Log.Write("Total Items Added: " + _totalItemsAdded); // After each list (more verbose)                            
                        }
                        Log.Write("Total Items Deleted: " + _totalItemsDeleted);
                        Log.Write("Total Items: " + _totalItemsAdded); //After a Site (less verbose)
                    }
                }
            }
        }

        private void addDocumentToFolder(SPDGList docLib, SPDGFolder folder)
        {                        
            fileTypeRotator();
            byte[] fileContent = getFileContent();

            Random rnd = new Random();
            int randomValue = rnd.Next(7);
            string url = "";

            if ((randomValue % 6) == 0)
            {
                url = SampleData.GetSampleValueRandom(SampleData.Documents) + " " + SampleData.GetSampleValueRandom(SampleData.Departments.Select(d => d.Department).ToList()) + "." + _currentFileType;
            }
            else if ((randomValue % 5) == 0)
            {
                url = SampleData.GetSampleValueRandom(SampleData.Years) + " " + SampleData.GetSampleValueRandom(SampleData.Documents) + " " + SampleData.GetSampleValueRandom(SampleData.Departments.Select(d => d.Department).ToList()) + "." + _currentFileType;
            }
            else if ((randomValue % 4) == 0)
            {
                url = SampleData.GetSampleValueRandom(SampleData.Documents) + " " + SampleData.GetSampleValueRandom(SampleData.FirstNames) + "." + _currentFileType;
            }
            else if ((randomValue % 3) == 0)
            {
                url = SampleData.GetSampleValueRandom(SampleData.Documents) + " " + SampleData.GetSampleValueRandom(SampleData.Years) + "." + _currentFileType;
            }
            else
            {
                url = SampleData.GetSampleValueRandom(SampleData.Documents) + "." + _currentFileType;
            }

            try
            {   // Log an exception here, do not increment counter 
                var spFile = folder.AddFile(url, fileContent, true);
                var fileItem = spFile.Item;
            
                if (fileItem != null)
                {
                    populateItemInfo(docLib, fileItem, true);
                    fileItem.Update();
                }
                _docsAdded++;
            }
            catch (Exception ex)
            {
                Log.Write("Failed to add document: " + url + " Exception: " + ex.ToString());
            }
                       
            foreach(var childFolder in folder.SubFolders)
            {
                if( _docsAdded >= WorkingDefinition.MaxNumberofDocumentLibraryItemsToGenerate)
                {
                    break;
                }

                if( childFolder.Url.IndexOf("/Forms") == -1  )
                {
                    addDocumentToFolder(docLib, childFolder);
                }
            }
            Owner.IncrementCurrentTaskProgress("Adding document to folder: " + folder.Url);
        }

        private byte[] getFileContent()
        {
            byte[] content = null;
            if( _currentFileType == "test")
            {
                FileStream fstream = File.OpenRead("C:\\Temp\\test.docx");
                content = new byte[fstream.Length];
                fstream.Read(content, 0, (int)fstream.Length);
                fstream.Close();
            }
            else if (_currentFileType == "docx")
            {
                content = SampleData.CreateDocx();
            }
            else if (_currentFileType == "xlsx")
            {
                content = SampleData.CreateExcel();
            }
            else if (_currentFileType == "pdf")
            {
                content = SampleData.CreatePDF(WorkingDefinition.MinDocumentSizeKB, WorkingDefinition.MaxDocumentSizeMB * 1024);
            }
            else if (_currentFileType == "png")
            {
                content = SampleData.AddRandomPngFile();
            }

            return content;
        }


        private static Dictionary<string,int> _titleUsage=new Dictionary<string, int>();
        private void populateItemInfo(SPDGList list, ISPDGListItemInfo item, bool isDocLib, bool addAttachment)
        {
            if (addAttachment)
            {
                populateItemInfo(list, item, isDocLib);
                fileTypeRotator();
                var attachmentInfo = new SPDGListItemAttachmentInfo(Guid.NewGuid().ToString() + '.' + _currentFileType, getFileContent());
                item.Attachment = attachmentInfo;
            }
            else
                populateItemInfo(list, item, isDocLib);
        }

        private void populateItemInfo(SPDGList list, ISPDGListItemInfo item, bool isDocLib )
        {
            List<string> userFields = new List<string>();
            foreach(var field in list.Fields)
            {
                if(ColumnsAndViewsGenerationTask.AvailableFieldInfos.Any(x=>x.DisplayName==field.Title))
                {
                    userFields.Add(field.InternalName);
                }
            }

            string title = getFieldValue("First Name") + " "  + getFieldValue("Last Name");
            if (!_titleUsage.ContainsKey(title))
            {
                _titleUsage[title] = 0;
            }
            _titleUsage[title]++;
            if (_titleUsage[title] != 1)
            {
                title += " No. " + _titleUsage[title];
            }
            item["Title"] = title;

            foreach( string fieldName in userFields )
            {
                object value = getFieldValue(fieldName);
                if( value != null )
                {
                    item[fieldName] = value;
                }
            }    
        }

        private void populateTask(ISPDGListItemInfo item)
        {
            string title = SampleData.GetSampleValueRandom(SampleData.Accounts) + " Task";
            item["Title"] = title;
            item["Status"] = "In Progress";
            item["DueDate"] = SampleData.GetRandomDate(2013, 2015);
            item["Priority"] = "(2) Normal";
            item["PercentComplete"] = SampleData.GetRandomNumber(1, 100) / 100;                        
        }

        private void populateEvent(ISPDGListItemInfo item)
        {            
            string title = SampleData.GetSampleValueRandom(SampleData.Accounts) + " Event";
            item["Title"] = title;
            DateTime time = SampleData.GetRandomDateCurrentMonth();
            item["EventDate"] = time;
            item["EndDate"] = time;
            item["Location"] = SampleData.GetSampleValueRandom(SampleData.Cities);                 
        }

        private object getFieldValue( string fieldName)
        {
            if( fieldName == "First Name")
            {
                return SampleData.GetSampleValueRandom(SampleData.FirstNames);
            }
            else if (fieldName == "Last Name")
            {
                return SampleData.GetSampleValueRandom(SampleData.LastNames);
            }
            else if (fieldName.ToLower().Contains("phone"))
            {
                return SampleData.GetSampleValueRandom(SampleData.PhoneNumbers);
            }
            else if (fieldName.ToLower().StartsWith("web") )
            {
                return SampleData.GetSampleValueRandom(SampleData.WebSites);
            }
            else if (fieldName.ToLower().StartsWith("year"))
            {
                return SampleData.GetSampleValueRandom(SampleData.Years);
            }
            else if (fieldName.ToLower().StartsWith("email"))
            {
                return SampleData.GetSampleValueRandom(SampleData.EmailAddreses);
            }
            else if (fieldName == "Country")
            {
                return SampleData.GetSampleValueRandom(SampleData.Countries);
            }
            else if (fieldName == "Company")
            {
                return SampleData.GetSampleValueRandom(SampleData.Companies);
            }
            else if (fieldName == "City")
            {
                return SampleData.GetSampleValueRandom(SampleData.Cities);
            }
            else if (fieldName.ToLower().Contains("address"))
            {
                return SampleData.GetSampleValueRandom(SampleData.Addresses);
            }
            else if (fieldName == "Account")
            {
                return SampleData.GetSampleValueRandom(SampleData.Accounts);
            }
            else if (fieldName == "Salary")
            {
                return SampleData.GetRandomNumber(20000, 200000);
            }
            else if (fieldName == "Birthday")
            {
                return SampleData.GetRandomDate(1940, 2000);
            }

            return null;
        }

        private void fileTypeRotator()
        {
            bool changed = false;

            if (_currentFileType == null)
            {
                if (WorkingDefinition.IncludeDocTypeDOCX) { _currentFileType = "docx"; return; }
                if (WorkingDefinition.IncludeDocTypeXLSX) {_currentFileType = "xlsx"; return;}
                if (WorkingDefinition.IncludeDocTypePDF){ _currentFileType = "pdf"; return;}
                if (WorkingDefinition.IncludeDocTypeImages) { _currentFileType = "png"; return; }
                return;
            }

            if( _currentFileType == "docx" )
            {
                if (WorkingDefinition.IncludeDocTypeXLSX) {_currentFileType = "xlsx"; return;}
                if (WorkingDefinition.IncludeDocTypePDF) {_currentFileType = "pdf"; return;}
                if (WorkingDefinition.IncludeDocTypeImages) { _currentFileType = "png"; return; }
                return;
            }

            if (_currentFileType == "xlsx")
            {
                if (WorkingDefinition.IncludeDocTypePDF) {_currentFileType = "pdf"; return;}
                if (WorkingDefinition.IncludeDocTypeImages) { _currentFileType = "png"; return; }
                if (WorkingDefinition.IncludeDocTypeDOCX) {_currentFileType = "docx"; return;}
                return;
            }

            if (_currentFileType == "pdf")
            {
                if (WorkingDefinition.IncludeDocTypeImages) { _currentFileType = "png"; return; }
                if (WorkingDefinition.IncludeDocTypeDOCX) {_currentFileType = "docx"; return;}
                if (WorkingDefinition.IncludeDocTypeXLSX){ _currentFileType = "xlsx"; return;}
                return;
            }

            if( _currentFileType == "png" )
            {
                if (WorkingDefinition.IncludeDocTypeDOCX) {_currentFileType = "docx"; return;}
                if (WorkingDefinition.IncludeDocTypeXLSX){ _currentFileType = "xlsx"; return;}
                if (WorkingDefinition.IncludeDocTypePDF) { _currentFileType = "pdf"; return; }
                
            }

        }

       
    }
}
