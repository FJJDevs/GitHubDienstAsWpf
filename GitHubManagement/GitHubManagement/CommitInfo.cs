using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitHubManagement
{
    public class CommitInfo
    {
        public string Title { get; set; }
        public string Date { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
        public string CommitId { get; set; }
        public string RepoId { get; set; }
        public bool IsInDatabase { get; set; }
    }
}
