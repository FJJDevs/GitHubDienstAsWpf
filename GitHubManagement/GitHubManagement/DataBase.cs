using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace GitHubManagement
{
    public class DataBase
    {
        private SqlConnection myConnection = new SqlConnection(@"Server=PCName\SQLServername,1434;Initial Catalog=Datenbankname;User ID=Username;Password=Passwort; Connection Timeout=3");
        private SqlCommand command;

        public DataBase(List<RepositoryInfo> listGitHubRepoInfos)
        {

        }
        public void AddNewGitHubEntrysToDB()
        {
            try
            {
                myConnection.Open();
                //Execute command
            }
            catch(Exception e)
            {
                Logger.LogMessage(e.ToString(),"AddNewGitHubEntrysToDB", "Fehler beim Hinzufügen von neuen Datensätzen in die DB / Oder keine Verbindung  zur DB");
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

