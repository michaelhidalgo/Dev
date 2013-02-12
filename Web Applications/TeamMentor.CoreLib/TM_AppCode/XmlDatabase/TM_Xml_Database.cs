using System;
using System.Collections.Generic;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.ExtensionMethods;
using urn.microsoft.guidanceexplorer;

namespace TeamMentor.CoreLib
{	
    public partial class TM_Xml_Database 
    {		    
        public static TM_Xml_Database Current                   { get; set; }
        public static bool            SkipServerOnlineCheck     { get; set;  }

        //config
        public bool			UsingFileStorage				{ get; set; }   
        public bool         ServerOnline                    { get; set; }
        
        //users
        public TM_UserData  UserData                        { get; set; }        

        //articles        
        public Dictionary<Guid, guidanceExplorer>	    GuidanceExplorers_XmlFormat { get; set; }	
        public Dictionary<Guid, string>				    GuidanceItems_FileMappings	{ get; set; }			
        public Dictionary<Guid, TeamMentor_Article>	    Cached_GuidanceItems		{ get; set; }
        public Dictionary<Guid, VirtualArticleAction>   VirtualArticles			{ get; set; }
                                                            
        public string 	Path_XmlDatabase 		        { get; set; }					
        public string 	Path_XmlLibraries 		        { get; set; }        
        public List<TM_Library> Libraries  				{  get { 	return this.tmLibraries(); } }
        public List<Folder_V3> 	Folders  				{  get { 	return this.tmFolders(); } } 		
        public List<View_V3> 	Views  					{  get { 	return this.tmViews(); } } 
        public List<TeamMentor_Article> GuidanceItems	{  get {	return this.tmGuidanceItems(); } }
      
        [Log("TM_Xml_Database Setup")]        
        public TM_Xml_Database          () : this(false)          // defaults to creating a TM_Instance in memory    
        {
        }
        public TM_Xml_Database          (bool useFileStorage)
        {
            O2Thread.mtaThread(CheckIfServerIsOnline);
            UsingFileStorage = useFileStorage;
            Current = this;
            Setup();
        }

        [Admin] public TM_Xml_Database ResetDatabase()
        {
            Cached_GuidanceItems        = new Dictionary<Guid, TeamMentor_Article>();
            GuidanceItems_FileMappings  = new Dictionary<Guid, string>();
            GuidanceExplorers_XmlFormat = new Dictionary<Guid, guidanceExplorer>();
            UserData                    = new TM_UserData(UsingFileStorage);
            return this;
        }

        [Admin] public TM_Xml_Database  Setup()
        {                       
            try
            {
                ResetDatabase();
                if (UsingFileStorage)
                {
                    SetPathsAndloadData();
                    this.handleDefaultInstallActions();
                    this.xmlDB_Load_GuidanceItems();					
                }

                this.createDefaultAdminUser(); // make sure this user exists						
            }
            catch(Exception ex)
            {
                "[TM_Xml_Database] .ctor: {0} \n\n".error(ex.Message, ex.StackTrace);
            }
            return this;
        } 
        [Admin] public void             CheckIfServerIsOnline()
        {
            if (SkipServerOnlineCheck)
                return;
            ServerOnline = new O2.Kernel.CodeUtils.O2Kernel_Web().online();     // only check this once
        }
        [Admin] public void             SetPathsAndloadData()
        {            
            try
            {
                var tmComfig            = TMConfig.Current;
                var xmlDatabasePath     = tmComfig.xmlDatabasePath();
                var xmlLibraryPath      = tmComfig.XmlLibrariesPath;
                var userDataPath        = tmComfig.UserDataPath;                
                
                setLibraryPath_and_LoadDataIntoMemory(xmlDatabasePath, xmlLibraryPath, userDataPath);

                "[TM_Xml_Database][setDataFromCurrentScript] TM_Xml_Database.Path_XmlDatabase: {0}" .debug(xmlDatabasePath);
                "[TM_Xml_Database][setDataFromCurrentScript] TMConfig.Current.XmlLibrariesPath: {0}".debug(xmlLibraryPath);
                "[TM_Xml_Database][setDataFromCurrentScript] TMConfig.Current.UserDataPath: {0}"    .debug(userDataPath);
                TM_Xml_Database_Load_and_FileCache_Utils.populateGuidanceItemsFileMappings();	//only do this once
            }
            catch(Exception ex)
            {
                "[TM_Xml_Database] static .ctor: {0} \n\n".error(ex.Message, ex.StackTrace);
            }
        }        
        [Admin] public string           ReloadData()
        {
            return ReloadData(null);
        }				    
        [Admin] public string           ReloadData(string newLibraryPath)
        {	
            "In Reload data".info();
            if (newLibraryPath.notNull())
            {
                var tmConfig = TMConfig.Current;
                tmConfig.XmlLibrariesPath = newLibraryPath;
                tmConfig.SaveTMConfig();			
            }
            
            GuidanceItems_FileMappings.Clear();
            GuidanceExplorers_XmlFormat.Clear();								
            Libraries.Clear();
            Folders.Clear();
            Views.Clear();
            GuidanceItems.Clear();
            
            /*if(newLibraryPath.valid())
                setLibraryPath_and_LoadDataIntoMemory(newLibraryPath);
            else
                TM_Xml_Database.Current.loadDataIntoMemory();			*/

            Setup();

            /*this.reCreate_GuidanceItemsCache();	
            this.xmlDB_Load_GuidanceItems();
            
            this.createDefaultAdminUser();	// make sure this user exists		
            */
            return "In the library '{0}' there are {1} library(ies), {2} views and {3} GuidanceItems".
                        format(Current.Path_XmlLibraries.directoryName(), Libraries.size(), Views.size(), GuidanceItems.size());
        }
        
    }		
}
