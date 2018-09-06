using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace GitHubManagement
{
    public class GitHub
    {
        private int commitCounter = 0;
        private List<RepositoryInfo> repoInfoList;

        public GitHub()
        {
            repoInfoList = new List<RepositoryInfo>();
        }

        #region Commit

        public List<CommitInfo> SetUpCommitListWithInfo(string repoUrl, string repoName, string repoId)
        {
            string commitUrl = repoUrl + "/commits/master";
            // Wird jedes mal nach einem Commit neu gesetzt
            string webString = GetWebStringFromWebSite(commitUrl);          
            List<CommitInfo> commits = new List<CommitInfo>();
            while (true)
            {
                // + 19 jedes mal wenn er ein repo findet
                var index = webString.IndexOf("class=\"commit-title");
                // Findet kein Repo mehr im string
                if (index == -1)
                {
                    return commits;
                }
                string subString = webString.Substring(index);
                webString = webString.Substring(index + 19);
                CommitInfo commitInfo = new CommitInfo();
                string[] nameAndDescription = GetNameAndDescriptionOfCommit(subString);
                commitInfo.Title = nameAndDescription[0];
                commitInfo.Description = nameAndDescription[1];
                commitInfo.Date = GetDateTimeOfCommit(subString);
                commitInfo.Author = GetAutorOfCommit(subString);
                commitInfo.CommitId = commitCounter.ToString();
                commitInfo.RepoId = repoId;
                commits.Add(commitInfo);
                commitCounter++;
            }
        }

        private string GetIDOfCommit(string subString)
        {
            int indexOfIDInSubString = subString.IndexOf("clipboard-copy value=");
            string subStringID = subString.Substring(indexOfIDInSubString + 24);
            return subStringID.Split('"')[0];
        }

        private string GetAutorOfCommit(string subString)
        {
            string subStringAutor = subString.Substring(subString.IndexOf("alt") + 6);
            Console.WriteLine(subStringAutor.Split('"')[0]);
            return subStringAutor.Split('"')[0];
        }

        private string GetDateTimeOfCommit(string subString)
        {
            string subStringDateTime = subString.Substring(subString.IndexOf("datetime") + 10,
                                                                             20);
            Console.WriteLine(subStringDateTime);
            return subStringDateTime.Substring(0, 10);
        }
        private string[] GetNameAndDescriptionOfCommit(string subString)     
        {
            string subNameAndDescWithHTMLInfo = subString.Substring(subString.IndexOf("title=") + 7);
            string subWith = subNameAndDescWithHTMLInfo.Substring(0, subNameAndDescWithHTMLInfo.IndexOf("class=\"message\"") - 2);
            // class="message"
            var splitSubStringDescriptionAndName = subWith.Split(new[] { "\r\n","\r","\n" }, StringSplitOptions.None);
            string nameOfCommit = splitSubStringDescriptionAndName[0];
            string descriptionOfCommit = null;
            for (int i = 1; i < splitSubStringDescriptionAndName.Length; i++)
            {
                if (i == splitSubStringDescriptionAndName.Length - 1)
                {
                    descriptionOfCommit += splitSubStringDescriptionAndName[i];
                }
                else
                {
                    descriptionOfCommit += splitSubStringDescriptionAndName[i] + "\n";
                }
            }
            Console.WriteLine("Commitname: " + nameOfCommit);
            Console.WriteLine("description: " + descriptionOfCommit);
            return new string[] { nameOfCommit,
                                descriptionOfCommit };
        }

        #endregion

        private string GetWebStringFromWebSite(string url)
        {
            using (WebClient client = new WebClient())
            {
                ServicePointManager.SecurityProtocol = (SecurityProtocolType.Tls12);
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                try
                {
                    HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(url);
                    myRequest.Method = "GET";
                    WebResponse myResponse = myRequest.GetResponse();
                    StreamReader sr = new StreamReader(myResponse.GetResponseStream(), Encoding.UTF8);
                    // Wird jedes mal nach einem Repo neu gesetzt
                    string result = sr.ReadToEnd();

                    myResponse.Close();
                    sr.Close();
                    return result;
                }
                catch (Exception e)
                {
                    Logger.LogMessage(e.ToString(), "GetWebStringFromWebSite", "Fehler: Hat versucht die Url zu ereichen(fehlgeschlagen): " + url);
                    Environment.Exit(0);
                    return null;
                }
            }
        }
        #region Repository

        public List<RepositoryInfo> SetUpRepoListWithInfo()
        {
            //Repo URL
            string webString = GetWebStringFromWebSite("https://github.com/FJJDevs");
            while (true)
            {
                var index = webString.IndexOf("d-inline-block mb-1");
                //Findet kein Repo mehr im string
                if (index == -1)
                    return repoInfoList;
                string subString = webString.Substring(index);
                webString = webString.Substring(index + 19);

                RepositoryInfo repoInfoObj = new RepositoryInfo();
                repoInfoObj.NameOfRepository = GetNameOfSubStringOfReposetory(subString);
                repoInfoObj.Description = GetDescriptionOfReposetory(subString);
                repoInfoObj.LinkFromReposetory = "https://github.com/FJJDevs/" + repoInfoObj.NameOfRepository;
                repoInfoObj.RepoID = GetIDFromRepository(repoInfoObj.LinkFromReposetory);
                repoInfoObj.Commits = SetUpCommitListWithInfo(repoInfoObj.LinkFromReposetory,
                                                              repoInfoObj.NameOfRepository,
                                                              repoInfoObj.RepoID);
                Console.WriteLine(repoInfoObj.RepoID);
                Console.WriteLine("\n");
                repoInfoList.Add(repoInfoObj);
            }
        }

        public List<RepositoryInfo> GetRepoInfoList()
        {
            return this.repoInfoList;
        }

        private string GetIDFromRepository(string url)
        {
            string webString = GetWebStringFromWebSite(url);
            int indexOfIDString = webString.IndexOf("data-scope-id=");
            string subStringID = webString.Substring(indexOfIDString + 15);
            return subStringID.Split('"')[0];
        }

        private string GetNameOfSubStringOfReposetory(string subString)
        {
            var indexOfSubString = subString.IndexOf("codeRepository");
            string subStringName = subString.Substring(indexOfSubString + 17);
            var splitsubStringName = subStringName.Split('<');
            Console.WriteLine(splitsubStringName[0].Replace("  ",
                                                            ""));
            return splitsubStringName[0].Replace("  ", "");
        }

        private string GetDescriptionOfReposetory(string subString)
        {
            var indexOfSubString = subString.IndexOf("description");
            string subStringDescription = subString.Substring(indexOfSubString + 14);
            var splitSubStringDescription = subStringDescription.Split('<');
            string descriptionsubString = splitSubStringDescription[0].Replace("  ",
                                                                                "");
            Console.WriteLine(descriptionsubString.Substring(0, descriptionsubString.Length - 1));
            return descriptionsubString.Substring(0, descriptionsubString.Length - 1);
        }
        #endregion
    }
}
