using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitHubManagement
{
    public class RepositoryInfo
    {
        public string NameOfRepository { get; set; }
        public string Description { get; set; }
        public string LinkFromReposetory { get; set; }
        public string RepoID { get; set; }
        public List<CommitInfo> Commits { get; set; }
        public List<string> DataLinks { get; set; }
        public bool ExistsInDataBase { get; set; } = false;
    }

    public class RepositoryInfoForDataBase
    {
        public string NameOfRepository { get; set; }
        public string Description { get; set; }
        public string LinkFromReposetory { get; set; }
        public string RepoID { get; set; }
        public List<CommitInfo> Commits { get; set; }
        public List<string> DataLinks { get; set; }
        public bool ExistsInGitHub { get; set; } = false;
    }
}
