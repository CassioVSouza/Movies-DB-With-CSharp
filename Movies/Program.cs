
using MoviesLibrary;
using AccountAPI;
using System.Data.Entity.Migrations.Model;
using System.Data.SQLite;
using BCrypt.Net;

namespace Movies
{
    public class Program
    {
        static void Main(string[] args)
        {
            FileHandling logAndConnectionString = new FileHandling();

            string ConnectionString = logAndConnectionString.GetConnectionString();

            DataBaseManager dataBaseManager = new DataBaseManager(ConnectionString);
            
            AccountManagement accountManagement = new AccountManagement();

            dataBaseManager.InitializeDataBase();

            Console.Write("Welcome to the system, do you want to:\n1 - Register\n2 - Login\n3 - Exit");
            if(!int.TryParse(Console.ReadLine(), out int AccountChoice)) 
            {
                Console.WriteLine("Please, insert only numbers!");
            }

            try
            {
                switch (AccountChoice)
                {
                    case 1:
                        dataBaseManager.CreateAccount(accountManagement.RegisterAccount());
                        break;

                    case 2:
                        if (dataBaseManager.CheckPassword(dataBaseManager.CheckEmailAccount()))
                        {
                            Console.WriteLine("Welcome!");
                            MoviesOptions(ConnectionString);
                        }
                        else
                        {
                            Console.WriteLine("Attempt to login failed!");
                        }
                        break;

                    case 3:
                        Console.WriteLine("Exited!");
                        break;

                    default:
                        Console.WriteLine("Insert a valid number!");
                        break;
                }
            }
            catch (Exception ex)
            {
                logAndConnectionString.SaveInLog(ex.Message);
            }
            finally
            {
                dataBaseManager.CloseDatabase();
            }
        }

        private static void MoviesOptions(string ConnectionString)
        {
            DataBaseManager dataBaseManager = new DataBaseManager(ConnectionString);
            MoviesManager moviesManager = new MoviesManager();
            FileHandling log = new FileHandling();

            try
            {
                dataBaseManager.InitializeDataBase();
                int Choice;
                
                do
                {
                    Console.WriteLine("Welcome, What do you want to do?\n1 - Add Movie\n2 - Show Movies\n3 - Edit Movie\n4 - Delete Movie\n5 - Search Movie\n6 - Exit");
                    if (!int.TryParse(Console.ReadLine(), out Choice))
                    {
                        Console.WriteLine("Insert only numbers!");
                    }
                    
                    switch (Choice)
                    {
                        case 1:
                            dataBaseManager.InsertData(moviesManager.NewMovie());
                            break;

                        case 2:
                            dataBaseManager.ShowMoviesDB();
                            break;

                        case 3:
                            dataBaseManager.ShowMoviesDB();
                            dataBaseManager.EditMovies(moviesManager.NewMovie());
                            break;

                        case 4:
                            dataBaseManager.ShowMoviesDB();
                            dataBaseManager.DeleteMovie();
                            break;

                        case 5:
                            dataBaseManager.ShowSearchedMovie(moviesManager.SearchMovies());
                            break;

                        case 6:
                            Console.WriteLine("Exited!");
                            break;

                        default:
                            Console.WriteLine("Insert a valid number!");
                            break;
                    }
                } while (Choice != 6);
            }
            catch (Exception ex)
            {
                log.SaveInLog(ex.Message);
            }
            finally
            {
                dataBaseManager.CloseDatabase();
            }
        }

    }

    class DataBaseManager
    {
        private readonly string _stringConnection;
        private SQLiteConnection _connection;

        public DataBaseManager(string connection)
        {
            _stringConnection = connection;
            _connection = new SQLiteConnection(connection);
        }
        internal void InitializeDataBase()
        {
            _connection.Open();

            string CreateMovieTable = "CREATE TABLE IF NOT EXISTS Movies (ID INTEGER PRIMARY KEY AUTOINCREMENT, Name TEXT NOT NULL, " +
                                      "Rated INTEGER NOT NULL, Score INTEGER NOT NULL)";

            string CreateAccountsTable = "CREATE TABLE IF NOT EXISTS Accounts (ID INTEGER PRIMARY KEY AUTOINCREMENT, " +
                                         "Name TEXT NOT NULL, Email TEXT NOT NULL, Password TEXT NOT NULL, PhoneNumber TEXT NOT NULL)";

            using(SQLiteCommand command = new SQLiteCommand(CreateAccountsTable, _connection))
            {
                command.ExecuteNonQuery();
                using (SQLiteCommand secondCommand = new SQLiteCommand(CreateMovieTable, _connection))
                {
                    secondCommand.ExecuteNonQuery();
                }
            }

            Console.WriteLine("Connected!");
        }

        internal void InsertData(MovieModel movie)
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

        internal void ShowMoviesDB()
        {
            string SQLiteCommand = "SELECT * FROM Movies";

            using(SQLiteCommand command = new SQLiteCommand(SQLiteCommand, _connection))
            {
                using(SQLiteDataReader reader = command.ExecuteReader())
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

        internal void ShowSearchedMovie(string searchInput)
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

        internal void EditMovies(MovieModel movie)
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

        internal void DeleteMovie()
        {
            Console.Write("Name of the movie that you want to delete: ");
            string MovieName = Console.ReadLine() ?? string.Empty;

            string SQLiteDeleteCommand = "DELETE FROM Movies WHERE Name = @Name";

            using(SQLiteCommand command = new SQLiteCommand(SQLiteDeleteCommand, _connection))
            {
                command.Parameters.AddWithValue("@Name", MovieName);

                int RowsAffected = command.ExecuteNonQuery();

                if(RowsAffected > 0)
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
                  else {
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

        internal void CloseDatabase()
        {
            _connection.Close();
        }

    }

    class PasswordManager
    {
        internal string HashPassword(AccountModel Account)
        {
            string salt = BCrypt.Net.BCrypt.GenerateSalt();

            string HashPassword = BCrypt.Net.BCrypt.HashPassword(Account.Password, salt);

            return HashPassword;
        }

        internal bool CheckPassword(string DBPassword, string UserPassword)
        {
            return BCrypt.Net.BCrypt.Verify(UserPassword, DBPassword);
        }
    }

    class FileHandling
    {
        public void SaveInLog(string Error)
        {
            try
            {
                string filePath = GetFilePath();

                using(StreamWriter file = new StreamWriter(filePath))
                {
                    file.WriteLine(Error);
                }
            }catch(Exception ex) 
            {
                Console.WriteLine($"An error happened trying to save the log!: {ex}");
            }
        }

        private string GetFilePath()
        {
            string directoryPath = "LogMoviesApplication";
            string fileName = "Log.txt";
            string filePath = Path.Combine(directoryPath, fileName);

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            return filePath;
        }

        internal string GetConnectionString()
        {
            string directoryPath = "MovieDatabase";
            string fileName = "Database.db";
            string connectionString = Path.Combine("Data Source=" + directoryPath, fileName + ";Version=3;");

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            return connectionString;
        }
    }
}
