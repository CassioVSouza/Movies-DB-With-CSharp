using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using MoviesLibrary;
using AccountAPI;

namespace DataBase
{
    public class DataBaseManager
    {
        private readonly string _stringConnection;
        private SQLiteConnection _connection;

        public DataBaseManager(string connection)
        {
            _stringConnection = connection;
            _connection = new SQLiteConnection(connection);
        }
        public void InitializeDataBase()
        {
            _connection.Open();

            string CreateMovieTable = "CREATE TABLE IF NOT EXISTS Movies (ID INTEGER PRIMARY KEY AUTOINCREMENT, Name TEXT NOT NULL, " +
                                      "Rated INTEGER NOT NULL, Score INTEGER NOT NULL)";

            string CreateAccountsTable = "CREATE TABLE IF NOT EXISTS Accounts (ID INTEGER PRIMARY KEY AUTOINCREMENT, " +
                                         "Name TEXT NOT NULL, Email TEXT NOT NULL, Password TEXT NOT NULL, PhoneNumber TEXT NOT NULL)";

            using (SQLiteCommand command = new SQLiteCommand(CreateAccountsTable, _connection))
            {
                command.ExecuteNonQuery();
                using (SQLiteCommand secondCommand = new SQLiteCommand(CreateMovieTable, _connection))
                {
                    secondCommand.ExecuteNonQuery();
                }
            }

            Console.WriteLine("Connected!");
        }

        public void InsertData(MovieModel movie)
        {
            string InsertSQLCommand = "INSERT INTO Movies (Name, Rated, Score) VALUES (@Name, @Rated, @Score)";

            using (SQLiteCommand command = new SQLiteCommand(InsertSQLCommand, _connection))
            {
                command.Parameters.AddWithValue("@Name", movie.Name);
                command.Parameters.AddWithValue("@Rated", movie.Rated);
                command.Parameters.AddWithValue("@Score", movie.Score);

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    Console.WriteLine("Successfully inserted movie information.");
                }
                else
                {
                    Console.WriteLine("Error: Movie not inserted");
                }
            }
        }

        public void ShowMoviesDB()
        {
            string SQLiteCommand = "SELECT * FROM Movies";

            using (SQLiteCommand command = new SQLiteCommand(SQLiteCommand, _connection))
            {
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string MovieName = reader["Name"].ToString() ?? string.Empty;
                        int MovieRated = Convert.ToInt32(reader["Rated"]);
                        int MovieScore = Convert.ToInt32(reader["Score"]);

                        Console.WriteLine($"Name: {MovieName}, Rating +{MovieRated}, Score {MovieScore}");
                    }
                }
            }
        }

        public void ShowSearchedMovie(string searchInput)
        {
            FileHandling log = new FileHandling();

            string whereClause = string.Empty;
            string parameterName = string.Empty;

            try
            {
                if (searchInput.Length >= 2)
                {
                    switch (searchInput[0])
                    {
                        case 'N':
                            whereClause = "Name = @Name";
                            parameterName = "@Name";
                            break;
                        case 'R':
                            whereClause = "Rated = @Rated";
                            parameterName = "@Rated";
                            break;
                        case 'S':
                            whereClause = "Score = @Score";
                            parameterName = "@Score";
                            break;
                        default:
                            Console.WriteLine("Invalid search input.");
                            return;
                    }

                    int searchValue;
                    int.TryParse(searchInput, out searchValue);

                    string sqlCommand = $"SELECT * FROM Movies WHERE {whereClause}";

                    using (SQLiteCommand command = new SQLiteCommand(sqlCommand, _connection))
                    {
                        if (searchInput[0] == 'N')
                        {
                            command.Parameters.AddWithValue(parameterName, searchInput.Substring(2));
                        }
                        else if (searchInput[0] == 'R' || searchInput[0] == 'S')
                        {
                            command.Parameters.AddWithValue(parameterName, searchValue);
                        }

                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string movieName = reader["Name"].ToString() ?? string.Empty;
                                int rated = Convert.ToInt32(reader["Rated"]);
                                int score = Convert.ToInt32(reader["Score"]);

                                Console.WriteLine($"Name: {movieName}, Rating: +{rated}, Score: {score}");
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Invalid search input.");
                }
            }
            catch (Exception ex)
            {
                log.SaveInLog(ex.Message);
            }
        }

        public void EditMovies(MovieModel movie)
        {
            Console.Write("Insert the Name of the movie that you want do edit: ");
            string MovieChoice = Console.ReadLine() ?? string.Empty;

            string updateSqlCommand = "UPDATE Movies SET Name = @NewName, Rated = @Rated, Score = @Score WHERE Name = @OldName";

            using (SQLiteCommand command = new SQLiteCommand(updateSqlCommand, _connection))
            {
                command.Parameters.AddWithValue("@OldName", MovieChoice);
                command.Parameters.AddWithValue("@NewName", movie.Name);
                command.Parameters.AddWithValue("@Rated", movie.Rated);
                command.Parameters.AddWithValue("@Score", movie.Score);

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    Console.WriteLine("Successfully updated movie information.");
                }
                else
                {
                    Console.WriteLine("Movie not found or update failed.");
                }
            }
        }

        public void DeleteMovie()
        {
            Console.Write("Name of the movie that you want to delete: ");
            string MovieName = Console.ReadLine() ?? string.Empty;

            string SQLiteDeleteCommand = "DELETE FROM Movies WHERE Name = @Name";

            using (SQLiteCommand command = new SQLiteCommand(SQLiteDeleteCommand, _connection))
            {
                command.Parameters.AddWithValue("@Name", MovieName);

                int RowsAffected = command.ExecuteNonQuery();

                if (RowsAffected > 0)
                {
                    Console.WriteLine("Sucessfully deleted!");
                }
                else
                {
                    Console.WriteLine("Movie not found or an error has happened during the process!");
                }
            }
        }

        public void CreateAccount(AccountModel Account)
        {
            PasswordManager passwordManager = new PasswordManager();

            string SQLiteCreateCommand = "INSERT INTO Accounts (Name, Email, Password, PhoneNumber) VALUES (@Name, @Email, @Password, @PhoneNumber)";
            using (SQLiteCommand command = new SQLiteCommand(SQLiteCreateCommand, _connection))
            {
                command.Parameters.AddWithValue("@Name", Account.Name);
                command.Parameters.AddWithValue("@Email", Account.Email);
                command.Parameters.AddWithValue("@Password", passwordManager.HashPassword(Account));
                command.Parameters.AddWithValue("@PhoneNumber", Account.PhoneNumber);
                int RowsAffected = command.ExecuteNonQuery();

                if (RowsAffected > 0)
                {
                    Console.WriteLine("Account created sucessfully");
                }
                else
                {
                    Console.WriteLine("An error happened!");
                }
            }
        }
        public string CheckEmailAccount()
        {
            AccountManagement accountManagement = new AccountManagement();
            FileHandling log = new FileHandling();

            string SQLiteSelectCommand = "SELECT * FROM Accounts WHERE Email = @Email";

            try
            {
                do
                {
                    using (SQLiteCommand command = new SQLiteCommand(SQLiteSelectCommand, _connection))
                    {
                        string Email = accountManagement.LoginAccountEmail();

                        command.Parameters.AddWithValue("@Email", Email);

                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Console.WriteLine($"Account finded!");
                                return Email;
                            }
                        }
                    }
                } while (true);
            }
            catch (Exception ex)
            {
                log.SaveInLog(ex.Message);
            }
            return "This email is not registered!";
        }

        public bool CheckPassword(string Email)
        {
            AccountManagement accountManagement = new AccountManagement();
            PasswordManager passwordManager = new PasswordManager();
            FileHandling log = new FileHandling();

            string SQLiteSelectCommand = "SELECT * FROM Accounts WHERE Email = @Email";

            try
            {
                using (SQLiteCommand command = new SQLiteCommand(SQLiteSelectCommand, _connection))
                {
                    command.Parameters.AddWithValue("@Email", Email);

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string DBPassword = reader["Password"].ToString() ?? string.Empty;
                            string UserPassword = accountManagement.LoginAccountPassword();

                            return passwordManager.CheckPassword(DBPassword, UserPassword) ? true : false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.SaveInLog(ex.Message);
            }
            return false;
        }

        public void CloseDatabase()
        {
            _connection.Close();
        }

    }
}
