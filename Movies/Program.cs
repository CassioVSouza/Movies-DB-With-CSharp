
using MoviesLibrary;
using System.Data.Entity.Migrations.Model;
using System.Data.SQLite;
using BCrypt.Net;
using AccountAPI;
using System.Security.Cryptography.X509Certificates;
using System.Data.SqlClient;

namespace Movies
{
    public class Program
    {
        static void Main(string[] args)
        {
            string ConnectionString = "Data Source=D:/EstudandosBancos/EstudandoBancos.db;Version=3;";

            DataBaseManager dataBaseManager = new DataBaseManager(ConnectionString);
            AccountManagement accountManagement = new AccountManagement();

            dataBaseManager.InitializeDataBase();
            Console.Write("Welcome to the system, do you want to:\n1 - Register\n2 - Login\n3 - Exit");
            int AccountChoice = Convert.ToInt32(Console.ReadLine());

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
            }catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                dataBaseManager.CloseDatabase();
            }
        }

        private static void MoviesOptions(string ConnectionString)
        {
            DataBaseManager dataBaseManager = new DataBaseManager(ConnectionString);
            MoviesManager moviesDB = new MoviesManager();

            try
            {
                dataBaseManager.InitializeDataBase();
                int Choice;
                do
                {
                    Console.WriteLine("Welcome, What do you want to do?\n1 - Add Movie\n2 - Show Movies\n3 - Edit Movie\n4 - Delete Movie\n5- Exit");
                    Choice = Convert.ToInt32(Console.ReadLine());

                    switch (Choice)
                    {
                        case 1:
                            dataBaseManager.InsertData(moviesDB.NewMovie());
                            break;
                        case 2:
                            dataBaseManager.ShowMoviesDB();
                            break;
                        case 3:
                            dataBaseManager.ShowMoviesDB();
                            dataBaseManager.EditMovies(moviesDB.NewMovie());
                            break;
                        case 4:
                            dataBaseManager.ShowMoviesDB();
                            dataBaseManager.DeleteMovie();
                            break;
                        case 5:
                            Console.WriteLine("Exited!");
                            break;
                        default: Console.WriteLine("Insert a valid number!");
                            break;
                    }

                } while (Choice != 5);
            }
            catch (FormatException ex)
            {
                Console.WriteLine(ex.Message);
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

                command.ExecuteNonQuery();
                Console.WriteLine("Sucessfully Insert");
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
                        int MovieID = Convert.ToInt32(reader["ID"]);
                        Console.WriteLine($"{MovieID} - {MovieName}, Rating +{MovieRated}, Score {MovieScore}");
                    }
                }
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
                    Console.WriteLine("Delete sucessfully!");
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
            try
            {
                string SQLiteSelectCommand = "SELECT * FROM Accounts WHERE Email = @Email";
                bool AccountFounded = false;
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
                                AccountFounded = true;
                                return Email;
                            }
                        }
                    }
                } while (AccountFounded == false);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return "This email is not registered!";
        }

        public bool CheckPassword(string Email)
        {
            AccountManagement accountManagement = new AccountManagement();
            PasswordManager passwordManager = new PasswordManager();

            string SQLiteSelectCommand = "SELECT * FROM Accounts WHERE Email = @Email";
            using(SQLiteCommand command = new SQLiteCommand(SQLiteSelectCommand, _connection))
            {
                command.Parameters.AddWithValue("@Email", Email);
                using(SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string DBPassword = reader["Password"].ToString() ?? string.Empty;
                        string UserPassword = accountManagement.LoginAccountPassword();

                        return passwordManager.CheckPassword(DBPassword, UserPassword) ? true : false;
                    }
                }
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
}
