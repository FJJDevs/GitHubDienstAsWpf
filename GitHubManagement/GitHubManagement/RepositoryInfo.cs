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
        public List<CommitInfo> commits;
        public List<string> dataLinks;

        public RepositoryInfo(string _name, string _description, string _link)      //Erstellungsdatum (Projekt besitzt Datein die man vlt noch darstellen möchte ...)
        {

        }
    }
}
