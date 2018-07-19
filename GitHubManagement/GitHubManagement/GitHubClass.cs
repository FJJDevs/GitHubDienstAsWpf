using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace GitHubManagement
{
    class GitHubClass
    {
        private List<RepositoryInfo> repoInfoList = new List<RepositoryInfo>();

        public GitHubClass()
        {
            repoInfoList = new List<RepositoryInfo>();
        }
        public List<RepositoryInfo> GetRepoInfoList()
        {
            return this.repoInfoList;
        }
        public List<CommitInfo> SetUpCommitListWithInfo(string repoUrl, string repoName)
        {
            string commitUrl = repoUrl + "/commits/master";
            string webString = GetWebStringFromWebSite(commitUrl);           //Wird jedes mal nach einem Commit neu gesetzt
            List<CommitInfo> commits = new List<CommitInfo>();
            while (true)
            {
                var index = webString.IndexOf("class=\"commit-title");     //+ 19 jedes mal wenn er ein repo findet
                if (index == -1)                //Findet kein Repo mehr im string
                    return commits;
                string subString = webString.Substring(index);
                webString = webString.Substring(index + 19);
                CommitInfo commitInfo = new CommitInfo();
                string[] nameAndDescription = GetNameAndDescriptionOfCommit(subString);
                commitInfo.titel = nameAndDescription[0];
                commitInfo.description = nameAndDescription[1];
                commitInfo.date = GetDateTimeOfCommit(subString);
                commitInfo.autor = GetAutorOfCommit(subString);
                commitInfo.commitID = GetIDOfCommit(subString);
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
            string subStringDateTime = subString.Substring(subString.IndexOf("datetime") + 10, 20);
            Console.WriteLine(subStringDateTime);
            return null;
        }
        private string[] GetNameAndDescriptionOfCommit(string subString)        //0 Im Array ist der Tittel und 1 die Description
        {
            string subStringNameAndDescriptionWithHtmlInformation = subString.Substring(subString.IndexOf("title=") + 7);
            string subStringNameAndDescriptionWithoutHtmlInformation = subStringNameAndDescriptionWithHtmlInformation.Substring(0, subStringNameAndDescriptionWithHtmlInformation.IndexOf("class=\"message\"") - 2);
            //class="message"
            var splitSubStringDescriptionAndName = subStringNameAndDescriptionWithoutHtmlInformation.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            string nameOfCommit = splitSubStringDescriptionAndName[0];
            string descriptionOfCommit = null;
            for (int i = 1; i < splitSubStringDescriptionAndName.Length; i++)
            {
                if (i == splitSubStringDescriptionAndName.Length - 1)
                    descriptionOfCommit += splitSubStringDescriptionAndName[i];
                else
                    descriptionOfCommit += splitSubStringDescriptionAndName[i] + "\n";
            }
            Console.WriteLine("Commitname: " + nameOfCommit);
            Console.WriteLine("description: " + descriptionOfCommit);
            return new string[] { nameOfCommit, descriptionOfCommit };
        }
        private string GetWebStringFromWebSite(string url)
        {
            using (WebClient client = new WebClient())
            {
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;          //Damit ein Tunnel aufgebaut werden kann
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                try
                {
                    HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(url);
                    myRequest.Method = "GET";
                    WebResponse myResponse = myRequest.GetResponse();
                    StreamReader sr = new StreamReader(myResponse.GetResponseStream(), System.Text.Encoding.UTF8);
                    string result = sr.ReadToEnd();                //Wird jedes mal nach einem Repo neu gesetzt

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
        public List<RepositoryInfo> SetUpRepoListWithInfo()
        {
            string webString = GetWebStringFromWebSite("https://github.com/FJJDevs");
            while (true)
            {
                var index = webString.IndexOf("d-inline-block mb-1");
                if (index == -1)                //Findet kein Repo mehr im string
                    return repoInfoList;
                string subString = webString.Substring(index);
                webString = webString.Substring(index + 19);

                RepositoryInfo repoInfoObj = new RepositoryInfo();
                repoInfoObj.nameOfRepository = GetNameOfSubStringOfReposetory(subString);
                repoInfoObj.description = GetDescriptionOfReposetory(subString);
                repoInfoObj.linkFromReposetory = "https://github.com/FJJDevs/" + repoInfoObj.nameOfRepository;
                repoInfoObj.commits = SetUpCommitListWithInfo(repoInfoObj.linkFromReposetory, repoInfoObj.nameOfRepository);
                repoInfoObj.repoID = GetIDFromRepository(repoInfoObj.linkFromReposetory);
                Console.WriteLine(repoInfoObj.repoID);
                Console.WriteLine("\n");
                repoInfoList.Add(repoInfoObj);
            }
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
            Console.WriteLine(splitsubStringName[0].Replace("  ", ""));
            return splitsubStringName[0].Replace("  ", "");
        }
        private string GetDescriptionOfReposetory(string subString)
        {
            var indexOfSubString = subString.IndexOf("description");
            string subStringDescription = subString.Substring(indexOfSubString + 14);
            var splitSubStringDescription = subStringDescription.Split('<');
            string descriptionsubString = splitSubStringDescription[0].Replace("  ", "");
            Console.WriteLine(descriptionsubString.Substring(0, descriptionsubString.Length - 1));
            return descriptionsubString.Substring(0, descriptionsubString.Length - 1);
        }
    }
}
