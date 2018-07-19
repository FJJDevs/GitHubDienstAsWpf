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
        public DataBase()
        {

        }
        private static MySqlConnection GetDbConnetion()
        {
            string connectionString = "SERVER=127.0.0.1;" +
                                      "DATABASE= gitdb;" +
                                      "UID = root";
            MySqlConnection connection = new MySqlConnection(connectionString);

            return connection;
        }
        /// <summary>
        /// Fügt die neuen GitHubEinträge (Repository) zu der Datenbank hinzu
        /// </summary>
        /// <param name="repoInfo"></param>
        public void AddNewRepoGitHubEntryToDB(RepositoryInfo repoInfo)
        {
            MySqlConnection connection = new MySqlConnection();
            try
            {
                connection = GetDbConnetion();
                connection.Open();
            }
            catch (Exception e)
            {
                Logger.LogMessage(e.ToString(), "AddNewGitHubEntrysToDB", "keine Verbindung  zur DB möglich");
                Console.WriteLine(e);
            }
            try
            {

                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "INSERT INTO `repositories`(`RepoName`, `RepoDescription`, `RepoLink`) VALUES('" + repoInfo.nameOfRepository + "', '" + repoInfo.description + "', '" + repoInfo.linkFromReposetory + "')";
                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Logger.LogMessage(e.ToString(), "AddNewGitHubEntrysToDB", "Fehler beim Hinzufügen von neuen Datensätzen in die DB");
                connection.Close();
            }
            //Bevor das gemacht werden kann muss überprüft werden welche Unterschiedlich sind (GitHubInfos und DBInfos)
            //Vlt auch gucken ob sich was geändert hat oder einfach etwas neues hinzugefügt wurde
        }
        public void AddNewCommitGitHubEntryToDB(CommitInfo commit)
        {
            MySqlConnection connection = new MySqlConnection();
            try
            {
                connection = GetDbConnetion();
                connection.Open();
            }
            catch (Exception e)
            {
                Logger.LogMessage(e.ToString(), "AddNewCommitGitHubEntryToDB", "keine Verbindung  zur DB möglich");
                Console.WriteLine(e);
            }
        }
        //Vielleicht sogar in der Methode updaten wenn es nicht zu vielen Informationen werden
        public void CheckForChangesInCommit(CommitInfo commit)
        {
            //Alle Variablen überprüfen ob sich etwas verändert hat zwischen DB und Github
            MySqlConnection connection = new MySqlConnection();
            try
            {
                connection = GetDbConnetion();
                connection.Open();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Logger.LogMessage(e.ToString(), "CheckForChangesInCommit", "keine Verbindung zur DB möglich");
            }

        }
        //Vielleicht sogar in der Methode updaten wenn es nicht zu vielen Informationen werden
        public void CheckForChangesInRepo(RepositoryInfo repository)
        {
            MySqlConnection connection = new MySqlConnection();
            try
            {
                connection = GetDbConnetion();
                connection.Open();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Logger.LogMessage(e.ToString(), "CheckForChangesInRepo", "keine Verbindung zur DB möglich");
            }
        }
        /// <summary>
        /// Überprüft ob neue GitHubEinträge hinzugefügt wurden
        /// </summary>
        public void CheckForNewRepoGitHubEntrys()
        {
            MySqlConnection connection = new MySqlConnection();
            try
            {
                connection = GetDbConnetion();
                connection.Open();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Logger.LogMessage(e.ToString(), "CheckForNewRepoGitHubEntrys", "Verbindung zu der DB konnte nicht aufgebaut werden");
            }
            try
            {

            }
            catch (Exception e)
            {
                Logger.LogMessage(e.ToString(), "CheckForNewRepoGitHubEntrys", "Fehler beim herausfinden von neuen Datensätzen (Repos) in der DB");
                Console.WriteLine(e);
                connection.Close();
            }
            //Hier wird gegucktob etwas neues Hinzugefügt wurde oder etwas geändert wurde an den Datensätzen
        }
        public void CheckForNewCommitGitHubEntrys()
        {
            MySqlConnection connection = new MySqlConnection();
            try
            {
                connection = GetDbConnetion();
                connection.Open();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Logger.LogMessage(e.ToString(), "CheckForNewCommitGitHubEntrys", "Verbindung zu der DB konnte nicht aufgebaut werden");
            }
            try
            {

            }
            catch (Exception e)
            {
                Logger.LogMessage(e.ToString(), "CheckForNewCommitGitHubEntrys", "Fehler beim herausfinden von neuen Datensätzen (Commits) in der DB");
                Console.WriteLine(e);
            }
            //Hier wird gegucktob etwas neues Hinzugefügt wurde oder etwas geändert wurde an den Datensätzen
        }
    }
}

