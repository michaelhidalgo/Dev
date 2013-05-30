using System;
using System.Collections.Generic;
using O2.DotNetWrappers.ExtensionMethods;
using O2.DotNetWrappers.Windows;

namespace TeamMentor.CoreLib
{
    public static class TM_Xml_Database_Ex_Config
    {        
        public static bool copy_FilesIntoWebRoot(this TM_Xml_Database tmDatabase)
        {            
            var sourceFolder = tmDatabase.UserData.webRootFiles();
            if (sourceFolder.notValid())
                return false;
            var targetFolder = tmDatabase.webRoot();
            if (targetFolder.pathCombine("web.config").fileExists().isFalse())
            {
                "[copy_FilesIntoWebRoot] failed because web.config was not found on targetFolder: {0}".error(targetFolder);
                return false;
            }
            if (sourceFolder.dirExists().isFalse())
            {
                "[copy_FilesIntoWebRoot] skipped because targetFolder was not found: {0}".error(targetFolder);
                return false;
            }            
            Files.copyFolder(sourceFolder, targetFolder,true,true,"");            
            return true;
        }

        public static string getGitUserConfigFile(this TM_Xml_Database tmDatabase)         
        {
            return (tmDatabase.notNull() && tmDatabase.UsingFileStorage)
                        ? tmDatabase.Path_XmlDatabase.pathCombine(TMConsts.GIT_USERDATA_FILENAME) 
                        : "";
            //return (TMConfig.Location.valid())
//                        ? TMConfig.Location.parentFolder().pathCombine(TMConsts.GIT_USERDATA_FILENAME)
  //                      : "";
        }

        public static bool setGitUserConfigFile(this TM_Xml_Database tmDatabase, string gitUserConfig_Data)
        {
            try
            {
                var gitUserConfigFile = tmDatabase.getGitUserConfigFile();
                if (gitUserConfig_Data.notValid() && gitUserConfigFile.fileExists())
                {
                    "[setGitUserConfigFile] Deleting current gitUserconfigFile: {0}".info(gitUserConfigFile);
                    gitUserConfigFile.file_Delete();
                }
                else
                    gitUserConfig_Data.saveAs(gitUserConfigFile);
                return true;
            }
            catch (Exception ex)
            {
                ex.log("[setGitUserConfigFile]");
                return false;
            }
        }
    }
}
