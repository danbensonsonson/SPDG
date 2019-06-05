using System;
using System.IO;
using System.Xml.Serialization;
using Acceleratio.SPDG.Generator;

namespace Acceleratio.SPDG.UI
{
    
    public class Common
    {
        public const string APP_TITLE = "SharePoint Data Generator";

        public static bool PreventAppClosing { get; set; }

        public static GeneratorDefinitionBase WorkingDefinition { get; set; }


        public static void InitClientDefinition()
        {
            var clientDefinition = new ClientGeneratorDefinition();
            SetCommonDefaults(clientDefinition);
            WorkingDefinition = clientDefinition;
        }

        public static void InitServerDefinition()
        {
            var serverDefinition = new ServerGeneratorDefinition();
            SetCommonDefaults(serverDefinition);

            serverDefinition.CredentialsOfCurrentUser = true;
            serverDefinition.CreateNewWebApplications = 0;

            WorkingDefinition = serverDefinition;
        }

        public class SerializeWrapper
        {
            public GeneratorDefinitionBase Definition { get; set; }
        }

        public static void SerializeDefinition(string path)
        {
            if (!Directory.Exists(Common.ConfigDir))
                Directory.CreateDirectory(Common.ConfigDir);
            XmlSerializer serializer = new XmlSerializer(typeof(SerializeWrapper), new Type[] { typeof(ClientGeneratorDefinition), typeof(ServerGeneratorDefinition) });
            using (TextWriter writer = new StreamWriter(path))
            {
                serializer.Serialize(writer, new SerializeWrapper() { Definition = WorkingDefinition });
            }            
        }

        public static string ConfigDir
        {
            get { return "config\\";  }
        }

        public static void DeserializeDefinition(string path)
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(SerializeWrapper), new Type[] { typeof(ClientGeneratorDefinition), typeof(ServerGeneratorDefinition) });
            TextReader reader = new StreamReader(path);
            object obj = deserializer.Deserialize(reader);
            WorkingDefinition = ((SerializeWrapper)obj).Definition;
            reader.Close();            
        }

        private static void SetCommonDefaults(GeneratorDefinitionBase definition)
        {          
            definition.NumberOfSecurityGroupsToCreate = 0;
            definition.NumberOfUsersToCreate = 0;
            definition.NumberOfSitesToCreate = 10;
            definition.MaxNumberOfColumnsPerList = 0;
            definition.MaxNumberOfContentTypesPerSiteCollection = 0;
            definition.MaxNumberOfFoldersToGenerate = 0;
            definition.MaxNumberofItemsToGenerate = 1000;
            definition.NumberofItemsToDelete = 0;
            definition.MaxNumberofDocumentLibraryItemsToGenerate = 1000;
            definition.NumberofDocumentLibraryItemsToDelete = 0;
            definition.MaxNumberOfLevelsForSites = 1;
            definition.MaxNumberOfListsAndLibrariesPerSite = 90;
            definition.PercentListItemsWithAttachments = 10;
            definition.MaxNumberOfViewsPerList = 0;         
            definition.CreateNewSiteCollections = 1;
            definition.SiteTemplate = "Team Site";
            definition.LibTypeList = true;
            definition.LibTypeDocument = true;
            definition.LibTypeCalendar = false;
            definition.LibTypeTasks = false;
            definition.CreateSomeFoldersInDocumentLibraries = true;
            definition.MaxNumberOfFoldersToGenerate = 1;
            definition.MaxNumberOfNestedFolderLevelPerLibrary = 1;
            definition.CreateColumns = true;
            definition.MaxNumberOfColumnsPerList = 20;
            definition.PrefilListAndLibrariesWithItems = true;
            definition.IncludeDocTypeDOCX = true;
            definition.IncludeDocTypePDF = true;
            definition.IncludeDocTypeImages = true;
            definition.IncludeDocTypeXLSX = true;
            definition.MinDocumentSizeKB = 100;
            definition.MaxDocumentSizeMB = 1;
            definition.ContentTypesCanInheritFromOtherContentType = true;
            definition.CreateContentTypes = true;
            definition.MaxNumberOfContentTypesPerSiteCollection = 1;
            definition.PermissionsPercentOfSites = 50;
            definition.PermissionsPercentOfLists = 3;
            definition.PermissionsPerObject = 3;
            definition.PermissionsPerObjectDelete = 0;
            definition.PermissionsPercentForUsers = 50;
            definition.PermissionsPercentForSPGroups = 3;
            definition.PermissionsPercentOfListItems = 3;
            definition.PermissionsPercentOfFolders = 3;
            definition.IncrementalUpdateSPGroupMembership = 1;

        }
    }

    public class ComboboxItem
    {
        public string Text { get; set; }
        public object Value { get; set; }

        public override string ToString()
        {
            return Text;
        }
    }
}
