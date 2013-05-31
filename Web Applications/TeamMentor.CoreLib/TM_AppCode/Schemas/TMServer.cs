using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TeamMentor.CoreLib
{
    public class TMServer
    {
        public List<TMServer_UserDataRepo> UserDataRepos { get; set; }

        public TMServer()
        {
            UserDataRepos = new List<TMServer_UserDataRepo>();
        }

        public class TMServer_UserDataRepo
        {
            public string Name  { get; set; }
            public string Path  { get; set; }
            public bool Enabled { get; set; }
        }
    }

    
}
