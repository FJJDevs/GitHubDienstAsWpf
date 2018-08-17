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
        private List<RepositoryInfoForDataBase> repoDataBaseList;
        private List<RepositoryInfo> repoInfosGitHub;
        public DataBase(List<RepositoryInfo> gitHubRepoList)
        {
            repoInfosGitHub = gitHubRepoList;
        }
        private static MySqlConnection GetDbConnetion()
        {
            string connectionString = "SERVER=127.0.0.1;" +
                                      "DATABASE= gitdb;" +
                                      "UID = root;" +
                                      "SslMode=none;";
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
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "INSERT INTO `repositories` (`RepoName`, `RepoDescription`, `RepoLink`, `RepoID`) VALUES('" + repoInfo.nameOfRepository + "', '" + repoInfo.description + "', '" + repoInfo.linkFromReposetory + "', '" + repoInfo.repoID + "')";
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


        public void RemoveOldRepository(RepositoryInfoForDataBase repInfo)
        {
            MySqlConnection connection = new MySqlConnection();
            try
            {
                                connection = GetDbConnetion();
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "DELETE  FROM `commits` WHERE RepoID = '" + repInfo.repoID + "'";
                command.ExecuteNonQuery();
                command.CommandText = "DELETE  FROM `repositories` WHERE RepoID = '" + repInfo.repoID + "'";
                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Logger.LogMessage(e.ToString(), "AddNewGitHubEntrysToDB", "Fehler beim Hinzufügen von neuen Datensätzen in die DB");
                connection.Close();
            }
        }
        public void AddNewCommitGitHubEntryToDB(CommitInfo commit)
        {
            MySqlConnection connection = new MySqlConnection();
            try
            {
                connection = GetDbConnetion();
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "INSERT INTO `commits`(`CommitID`,`CommitTitle`, `Date`, `Description`, `RepoID` ) VALUES('" + commit.commitID + "', '" + commit.titel + "', '" + commit.date + "', '" + commit.description + "', '" + commit.repoID + "')";
                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception e)
            {
                Logger.LogMessage(e.ToString(), "AddNewCommitGitHubEntryToDB", "keine Verbindung  zur DB möglich");
                Console.WriteLine(e);
            }

        }
        public void FillRepoDataBaseList()
        {
            MySqlConnection connection = new MySqlConnection();

            //Alle Infos aus der db besorgen 
            //Ist das Repo neu ?
            //Hat sich das Repo vberändert / gelöscht
            try
            {
                connection = GetDbConnetion();
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM repositories";
                var reader = command.ExecuteReader();
                repoDataBaseList = new List<RepositoryInfoForDataBase>();
                while (reader.Read())
                {
                    RepositoryInfoForDataBase repoInfoObj = new RepositoryInfoForDataBase();
                    repoInfoObj.repoID = reader.GetValue(0).ToString();
                    repoInfoObj.nameOfRepository = reader.GetValue(1).ToString();
                    repoInfoObj.description = reader.GetValue(2).ToString();
                    repoInfoObj.linkFromReposetory = reader.GetValue(3).ToString();
                    repoInfoObj.commits = GetCommitsOfRepo(Convert.ToInt32(repoInfoObj.repoID));
                    repoDataBaseList.Add(repoInfoObj);
                }
            }
            catch (Exception e)
            {

            }
            CheckIfRepoIsNew();
        }

        //Vielleicht sogar in der Methode updaten wenn es nicht zu vielen Informationen werden
        public List<CommitInfo> GetCommitsOfRepo(int repoID)
        {
            List<CommitInfo> commitInfoList = new List<CommitInfo>();
            MySqlConnection connection = new MySqlConnection();
            try
            {
                connection = GetDbConnetion();
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM `commits` WHERE RepoID = " + repoID;
                var reader = command.ExecuteReader();
                repoDataBaseList = new List<RepositoryInfoForDataBase>();
                while (reader.Read())
                {

                    CommitInfo commitInf = new CommitInfo();
                    commitInf.commitID = reader.GetValue(0).ToString();
                    commitInf.titel = reader.GetValue(1).ToString();
                    commitInf.date = reader.GetValue(2).ToString();
                    commitInf.description = reader.GetValue(3).ToString();
                    commitInf.repoID = reader.GetValue(4).ToString();
                    commitInfoList.Add(commitInf);
                }
                return commitInfoList;
            }
            catch (Exception e)

            {
                Console.WriteLine(e);
                Logger.LogMessage(e.ToString(), "CheckForNewCommitGitHubEntrys", "Verbindung zu der DB konnte nicht aufgebaut werden");
                return null;
            }
        }
        private void CheckIfRepoIsNew()
        {
            foreach (var repo in repoInfosGitHub)
            {
                foreach (var dataBaseRepoObj in repoDataBaseList)
                {
                    if(repo.repoID == dataBaseRepoObj.repoID)
                    {
                        //Check If Something is Changed
                        dataBaseRepoObj.existsInGitHub = true;
                        repo.existsInDataBase = true;
                        CheckForChangesInRepo(repo, dataBaseRepoObj);
                        CheckForNewCommitGitHubEntrys(repo.commits, dataBaseRepoObj.commits);
                    }
                }
                if (!repo.existsInDataBase)
                {
                    AddNewRepoGitHubEntryToDB(repo);
                    CheckForNewCommitGitHubEntrys(repo.commits, null);
                }
            }
            foreach (var deletRepo in repoDataBaseList)
            {
                if(deletRepo.existsInGitHub == true)
                {
                    RemoveOldRepository(deletRepo);
                }
            }
        }
        private void CheckForChangesInRepo(RepositoryInfo repo, RepositoryInfoForDataBase repoInfoDataBase)
        {
          
            if(repo.description != repoInfoDataBase.description)
            {
                try
                {
                    MySqlConnection connection = new MySqlConnection();
                    connection = GetDbConnetion();
                    connection.Open();
                    MySqlCommand command = connection.CreateCommand();
                    command.CommandText = "UPDATE  `repositories` SET RepoDescription = '" + repo.description + "' WHERE RepoID = '" + repo.repoID + "'";
                    command.ExecuteNonQuery();
                    connection.Close();
                }

                catch (Exception e)

                {
                    Console.WriteLine(e);
                    Logger.LogMessage(e.ToString(), "CheckForNewCommitGitHubEntrys", "Verbindung zu der DB konnte nicht aufgebaut werden");

                }

            }
            if (repo.linkFromReposetory != repoInfoDataBase.linkFromReposetory)
            {
                try
                {
                    MySqlConnection connection = new MySqlConnection();
                    connection = GetDbConnetion();
                    connection.Open();
                    MySqlCommand command = connection.CreateCommand();
                    command.CommandText = "UPDATE  `repositories` SET RepoLink = '" + repo.linkFromReposetory + "' WHERE RepoID = '" + repo.repoID + "'";
                    command.ExecuteNonQuery();
                    connection.Close();
                }

                catch (Exception e)

                {
                    Console.WriteLine(e);
                    Logger.LogMessage(e.ToString(), "CheckForNewCommitGitHubEntrys", "Verbindung zu der DB konnte nicht aufgebaut werden");
                 
                }
            }
            
        }
        public void CheckForNewCommitGitHubEntrys(List<CommitInfo> gitHubCommiListFromSingleRepo, List<CommitInfo> dataBaseCommitListFromSingleRepo)
        {
            try
            {
                foreach (var gitHubCommit  in gitHubCommiListFromSingleRepo)
                {
                    if (dataBaseCommitListFromSingleRepo != null)
                    {
                        foreach (var dataBaseCommit in dataBaseCommitListFromSingleRepo)
                        {
                            if (gitHubCommit.commitID == dataBaseCommit.commitID)
                            {

                                gitHubCommit.isInDatabase = true;
                                if (gitHubCommit.titel != dataBaseCommit.titel)
                                {
                                    try
                                    {
                                        MySqlConnection connection = new MySqlConnection();
                                        connection = GetDbConnetion();
                                        connection.Open();
                                        MySqlCommand command = connection.CreateCommand();
                                        command.CommandText = "UPDATE  `commits` SET CommitTitle = '" + gitHubCommit.titel + "' WHERE CommitID = '" + gitHubCommit.commitID + "'";
                                        command.ExecuteNonQuery();
                                        connection.Close();
                                    }

                                    catch (Exception e)

                                    {
                                        Console.WriteLine(e);
                                        Logger.LogMessage(e.ToString(), "CheckForNewCommitGitHubEntrys", "Verbindung zu der DB konnte nicht aufgebaut werden");

                                    }
                                }
                                if (gitHubCommit.description != dataBaseCommit.description)
                                {
                                    try
                                    {

                                        MySqlConnection connection = new MySqlConnection();
                                        connection = GetDbConnetion();
                                        connection.Open();
                                        MySqlCommand command = connection.CreateCommand();
                                        command.CommandText = "UPDATE  `commits` SET Description = '" + gitHubCommit.description + "' WHERE CommitID = '" + gitHubCommit.commitID + "'";
                                        command.ExecuteNonQuery();
                                        connection.Close();
                                    }

                                    catch (Exception e)

                                    {
                                        Console.WriteLine(e);
                                        Logger.LogMessage(e.ToString(), "CheckForNewCommitGitHubEntrys", "Verbindung zu der DB konnte nicht aufgebaut werden");

                                    }
                                }
                            }
                        }
                    }
                    if (!gitHubCommit.isInDatabase)
                        AddNewCommitGitHubEntryToDB(gitHubCommit);
                }
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

