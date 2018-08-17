using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitHubManagement
{
    public class RepositoryInfo
    {
        public string nameOfRepository;
        public string description;
        public string linkFromReposetory;
        public string repoID;
        public List<CommitInfo> commits;
        public List<string> dataLinks;
        public bool existsInDataBase = false;
    }
    public class RepositoryInfoForDataBase
    {
        public string nameOfRepository;
        public string description;
        public string linkFromReposetory;
        public string repoID;
        public List<CommitInfo> commits;
        public List<string> dataLinks;
        public bool existsInGitHub = false;
    }
}
