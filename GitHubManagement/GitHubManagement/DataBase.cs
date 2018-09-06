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
        private MySqlConnection connection;

        public DataBase(List<RepositoryInfo> gitHubRepoList)
        {
            repoInfosGitHub = gitHubRepoList;
            connection = GetDbConnetion();
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
            try
            {
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "INSERT INTO `repositories` (`RepoName`, `RepoDescription`, `RepoLink`, `RepoID`) " +
                    "                   VALUES('" + repoInfo.NameOfRepository + "', '" + repoInfo.Description + "', '" +
                                        repoInfo.LinkFromReposetory + "', '" + repoInfo.RepoID + "')";
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
            try
            {
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "DELETE  FROM `commits` WHERE RepoID = '" + repInfo.RepoID + "'";
                command.ExecuteNonQuery();
                command.CommandText = "DELETE  FROM `repositories` WHERE RepoID = '" + repInfo.RepoID + "'";
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
            try
            {
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "INSERT INTO `commits`(`CommitID`,`CommitTitle`, `Date`, `Description`, `RepoID` ) " +
                    "                  VALUES('" + commit.CommitId + "', '" + 
                                       commit.Title + "', '" + 
                                       commit.Date + "', '" + 
                                       commit.Description + "', '" + 
                                       commit.RepoId + "')";
                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception e)
            {
                Logger.LogMessage(e.ToString(), "AddNewCommitGitHubEntryToDB", "Keine Verbindung  zur DB möglich");
                Console.WriteLine(e);
            }

        }
        public void FillRepoDataBaseList()
        {
            // Alle Infos aus der db besorgen 
            // Ist das Repo neu ?
            // Hat sich das Repo vberändert / gelöscht
            try
            {
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM repositories";
                var reader = command.ExecuteReader();
                repoDataBaseList = new List<RepositoryInfoForDataBase>();
                while (reader.Read())
                {
                    RepositoryInfoForDataBase repoInfoObj = new RepositoryInfoForDataBase();
                    repoInfoObj.RepoID = reader.GetValue(0).ToString();
                    repoInfoObj.NameOfRepository = reader.GetValue(1).ToString();
                    repoInfoObj.Description = reader.GetValue(2).ToString();
                    repoInfoObj.LinkFromReposetory = reader.GetValue(3).ToString();
                    repoInfoObj.Commits = GetCommitsOfRepo(Convert.ToInt32(repoInfoObj.RepoID));
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
            try
            {
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM `commits` WHERE RepoID = " + repoID;
                var reader = command.ExecuteReader();
                repoDataBaseList = new List<RepositoryInfoForDataBase>();
                while (reader.Read())
                {

                    CommitInfo commitInf = new CommitInfo();
                    commitInf.CommitId = reader.GetValue(0).ToString();
                    commitInf.Title = reader.GetValue(1).ToString();
                    commitInf.Date = reader.GetValue(2).ToString();
                    commitInf.Description = reader.GetValue(3).ToString();
                    commitInf.RepoId = reader.GetValue(4).ToString();
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
                    if (repo.RepoID == dataBaseRepoObj.RepoID)
                    {
                        //Check If Something is Changed
                        dataBaseRepoObj.ExistsInGitHub = true;
                        repo.ExistsInDataBase = true;
                        CheckForChangesInRepo(repo, dataBaseRepoObj);
                        CheckForNewCommitGitHubEntrys(repo.Commits, dataBaseRepoObj.Commits);
                    }
                }
                if (!repo.ExistsInDataBase)
                {
                    AddNewRepoGitHubEntryToDB(repo);
                    CheckForNewCommitGitHubEntrys(repo.Commits, null);
                }
            }
            foreach (var deletRepo in repoDataBaseList)
            {
                if (deletRepo.ExistsInGitHub == true)
                {
                    RemoveOldRepository(deletRepo);
                }
            }
        }

        private void CheckForChangesInRepo(RepositoryInfo repo, RepositoryInfoForDataBase repoInfoDataBase)
        {

            if (repo.Description != repoInfoDataBase.Description)
            {
                try
                {
                    connection.Open();
                    MySqlCommand command = connection.CreateCommand();
                    command.CommandText = "UPDATE  `repositories` SET RepoDescription = '" + repo.Description + "' WHERE RepoID = '" + repo.RepoID + "'";
                    command.ExecuteNonQuery();
                    connection.Close();
                }

                catch (Exception e)

                {
                    Console.WriteLine(e);
                    Logger.LogMessage(e.ToString(), "CheckForNewCommitGitHubEntrys", "Verbindung zu der DB konnte nicht aufgebaut werden");

                }

            }
            if (repo.LinkFromReposetory != repoInfoDataBase.LinkFromReposetory)
            {
                try
                {
                    connection.Open();
                    MySqlCommand command = connection.CreateCommand();
                    command.CommandText = "UPDATE  `repositories` SET RepoLink = '" + repo.LinkFromReposetory + "' WHERE RepoID = '" + repo.RepoID + "'";
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
                foreach (var gitHubCommit in gitHubCommiListFromSingleRepo)
                {
                    if (dataBaseCommitListFromSingleRepo != null)
                    {
                        foreach (var dataBaseCommit in dataBaseCommitListFromSingleRepo)
                        {
                            if (gitHubCommit.CommitId == dataBaseCommit.CommitId)
                            {

                                gitHubCommit.IsInDatabase = true;
                                if (gitHubCommit.Title != dataBaseCommit.Title)
                                {
                                    try
                                    {
                                        connection.Open();
                                        MySqlCommand command = connection.CreateCommand();
                                        command.CommandText = "UPDATE  `commits` SET CommitTitle = '" + gitHubCommit.Title +
                                                              "' WHERE CommitID = '" + gitHubCommit.CommitId + "'";
                                        command.ExecuteNonQuery();
                                        connection.Close();
                                    }

                                    catch (Exception e)

                                    {
                                        Console.WriteLine(e);
                                        Logger.LogMessage(e.ToString(),
                                                         "CheckForNewCommitGitHubEntrys",
                                                         "Verbindung zu der DB konnte nicht aufgebaut werden");

                                    }
                                }
                                if (gitHubCommit.Description != dataBaseCommit.Description)
                                {
                                    try
                                    {

                                        connection.Open();
                                        MySqlCommand command = connection.CreateCommand();
                                        command.CommandText = "UPDATE  `commits` SET Description = '" + gitHubCommit.Description +
                                                              "' WHERE CommitID = '" + gitHubCommit.CommitId + "'";
                                        command.ExecuteNonQuery();
                                        connection.Close();
                                    }

                                    catch (Exception e)

                                    {
                                        Console.WriteLine(e);
                                        Logger.LogMessage(e.ToString(),
                                                         "CheckForNewCommitGitHubEntrys",
                                                         "Verbindung zu der DB konnte nicht aufgebaut werden");

                                    }
                                }
                            }
                        }
                    }
                    if (!gitHubCommit.IsInDatabase)
                        AddNewCommitGitHubEntryToDB(gitHubCommit);
                }
            }
            catch (Exception e)
            {
                Logger.LogMessage(e.ToString(),
                                 "CheckForNewCommitGitHubEntrys",
                                 "Fehler beim herausfinden von neuen Datensätzen (Commits) in der DB");
                Console.WriteLine(e);
            }
            // Hier wird gegucktob etwas neues Hinzugefügt wurde oder etwas geändert wurde an den Datensätzen
        }
    }
}

