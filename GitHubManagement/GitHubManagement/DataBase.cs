using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;

namespace GitHubManagement
{
    public class DataBase
    {
        MySqlConnectionStringBuilder conn_string = new MySqlConnectionStringBuilder();
     
       
        public DataBase()
        {

        }
        public DataBase(List<RepositoryInfo> listGitHubRepoInfos)
        {

        }
        public static MySqlConnection GetDbConnetion()
        {
            string connectionString = "SERVER=127.0.0.1;" +
                                      "DATABASE= gitdb;" +
                                      "UID = root";
            MySqlConnection connection = new MySqlConnection(connectionString);

            return connection;

        }
        public void AddNewGitHubEntrysToDB(RepositoryInfo repoInfo)
        {
            MySqlConnection connection = new MySqlConnection();
            try
            {
               connection = GetDbConnetion();
               connection.Open();
            }
            catch(Exception e)
            {
                Logger.LogMessage(e.ToString(),"AddNewGitHubEntrysToDB", "Fehler beim Hinzufügen von neuen Datensätzen in die DB / Oder keine Verbindung  zur DB");
                Console.WriteLine(e);
            }
            try
            {
               
                 MySqlCommand command = connection.CreateCommand();
                 command.CommandText = "INSERT INTO `repositories`(`RepoName`, `RepoDescription`, `RepoLink`) VALUES('"+repoInfo.nameOfRepository+"', '"+repoInfo.description+"', '"+repoInfo.linkFromReposetory+"')";
                ;
                command.ExecuteNonQuery();
                }
                catch(Exception e)
                {
                    Console.WriteLine(e);
                }
                //Bevor das gemacht werden kann muss überprüft werden welche Unterschiedlich sind (GitHubInfos und DBInfos)
                //Vlt auch gucken ob sich was geändert hat oder einfach etwas neues hinzugefügt wurde

            }
        public void CheckForNewGitHubEntrys()
        {
            try
            {

            }
            catch(Exception e)
            {
                Logger.LogMessage(e.ToString(), "CheckForNewGitHubEntrys", "Fehler beim herausfinden von neuen Datensätzen in der DB / Oder keine Verbindung  zur DB");
                Console.WriteLine(e);
            }
            //Hier wird gegucktob etwas neues Hinzugefügt wurde oder etwas geändert wurde an den Datensätzen
        }
    }
}

