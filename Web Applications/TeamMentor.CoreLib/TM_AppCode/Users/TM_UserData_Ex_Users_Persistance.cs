using System.Collections.Generic;
using O2.DotNetWrappers.ExtensionMethods;
using O2.FluentSharp;

namespace TeamMentor.CoreLib
{
    public static class TM_UserData_Ex_Users_Persistance
    {	
        public static TM_UserData   loadTmUserData(this TM_UserData userData)
        {
            userData.TMUsers = new List<TMUser>();	        
            if (userData.Path_UserData.dirExists().isFalse())
            {
                "[TM_UserData_Ex_Users_Persistance] in loadTmUserObjects, provided userDataPath didn't exist: {0}"
                    .error(userData.Path_UserData);
            }
            else
            {	            
                foreach (var file in userData.Path_UserData.files("*.userData.xml"))
                {
                    var tmUser = file.load<TMUser>();
                    if (tmUser.notNull() && tmUser.UserID > 0)
                        userData.TMUsers.Add(tmUser);
                }
            }
            return userData;
        }                       
        public static string        getTmUserXmlFile(this TMUser tmUser)
        {
            return TM_UserData.Current.Path_UserData.pathCombine("{0}.userData.xml".format(tmUser.ID));
        }
        public static TMUser        saveTmUser(this TMUser tmUser)
        {
            if (TM_UserData.Current.UsingFileStorage)
            {                
                lock (tmUser)
                {                    
                    tmUser.saveAs(tmUser.getTmUserXmlFile());
                    tmUser.triggerGitCommit();
                }
            }
            return tmUser;
        }

        public static bool          deleteTmUser(this TM_UserData userData, TMUser tmUser)
        {    		
            if (tmUser.notNull())
            {
                userData.TMUsers.remove(tmUser);
                if (userData.UsingFileStorage)
                {
                    lock (tmUser)
                    {
                        tmUser.getTmUserXmlFile().file_Delete();
                        userData.triggerGitCommit();
                    }
                }
                return true;
            }
            return false;
        }

        public static TM_UserData   setupGitSupport(this TM_UserData userData)
        {
            if (userData.AutoGitCommit)
            {
                if (userData.Path_UserData.isGitRepository())
                {
                    //"[TM_UserData][setupGitSupport] open repository: {0}".info(userData.Path_UserData);
                    userData.NGit = userData.Path_UserData.git_Open();
                }
                else
                {
                    //"[TM_UserData][setupGitSupport] initializing repository at: {0}".info(userData.Path_UserData);
                    userData.NGit = userData.Path_UserData.git_Init();
                }
            }
            return userData;
        }


        public static TMUser        triggerGitCommit(this TMUser tmUser)
        {
            TM_UserData.Current.triggerGitCommit();
            return tmUser;
        }
        public static TM_UserData   triggerGitCommit(this TM_UserData userData)
        {
            if (userData.AutoGitCommit)
                userData.NGit.add_and_Commit_using_Status();
            return userData;
        }
    }
}