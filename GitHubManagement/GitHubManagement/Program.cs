using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitHubManagement
{
    class Program
    {
        static void Main(string[] args)
        {
            GitHubClass gitClass = new GitHubClass();
            var list = gitClass.SetUpRepoListWithInfo();
            DataBase dataBaseObj = new DataBase(list);
            dataBaseObj.FillRepoDataBaseList();
            Console.ReadLine();
        }
    }
}
