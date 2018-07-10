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
        public void GetHtmlCodeForClubReposis()
        {
            using (WebClient client = new WebClient())
            {
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create("https://github.com/FJJDevs");
                myRequest.Method = "GET";
                WebResponse myResponse = myRequest.GetResponse();
                StreamReader sr = new StreamReader(myResponse.GetResponseStream(), System.Text.Encoding.UTF8);
                string result = sr.ReadToEnd();                //Wird jedes mal nach einem Repo neu gesetzt
                sr.Close();
                int addedIndex = 0;
                while (true)
                {
                    var index = result.IndexOf("d-inline-block mb-1");     //+ 19 jedes mal wenn er ein repo findet
                    if (index == -1)                //Findet kein Repo mehr im string
                        return;
                    addedIndex = 19;
                    string subString = result.Substring(index);
                    result = result.Substring(index + addedIndex);
                    GetNameOfSubStringOfReposetory(subString);
                    GetDescriptionOfReposetory(subString);
                }
            }
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
            Console.WriteLine(splitSubStringDescription[0].Replace("  ", ""));
            return splitSubStringDescription[0].Replace("  ", "");
        }
    }
}
