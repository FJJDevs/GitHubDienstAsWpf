using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace GitHubManagement
{
    class GitHubClass
    {
        public void GetHtmlCodeForClubReposis()
        {
            using (WebClient client = new WebClient())
            {
                string htmlCode = client.DownloadString("https://github.com/FJJDevs");          //Später noch erweiterbar, welchen Account/Team man auf repos durchsuchen möchte
            }
        }
    }
}
