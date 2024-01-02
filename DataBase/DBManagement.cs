// Importing necessary libraries
using System.Data.SQLite;
using MoviesLibrary;
using AccountAPI;

namespace DataBase
{
    // Class for managing the SQLite database operations
    public class DataBaseManager : IDatabase, IDisposable
    {
        private readonly string _stringConnection;
        private SQLiteConnection _connection;
        private string? _password;
        private int? _Id;
        private bool disposed = false;

        // FileHandling instance for logging purposes
        FileHandling log = new FileHandling();

        // Implementation of IDisposable interface
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected method for disposing resources
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    _connection?.Close();
                    _connection?.Dispose();
                }
                disposed = true;
            }
        }

        // Destructor to ensure disposal of resources
        ~DataBaseManager()
        {
            Dispose(false);
        }

        // Constructor to initialize the database connection
        public DataBaseManager(string connection)
        {
            _stringConnection = connection;
            _connection = new SQLiteConnection(connection);
        }

        // Method to initialize the database and create necessary tables
        public void InitializeDataBase()
        {
            string CreateMovieTable = "CREATE TABLE IF NOT EXISTS Movies (MovieID INTEGER PRIMARY KEY AUTOINCREMENT, Title TEXT NOT NULL, " +
                                      "Genre TEXT NOT NULL, ReleaseYear INTEGER NOT NULL, UserID INTEGER, FOREIGN KEY (UserID) REFERENCES User(UserID))";

            string CreateAccountsTable = "CREATE TABLE IF NOT EXISTS User (UserID INTEGER PRIMARY KEY AUTOINCREMENT, " +
                                         "Name TEXT NOT NULL, Email TEXT NOT NULL, Password TEXT NOT NULL)";

            try
            {
                _connection.Open();

                using (SQLiteCommand command = new SQLiteCommand(CreateAccountsTable, _connection))
                {
                    command.ExecuteNonQuery();
                    using (SQLiteCommand secondCommand = new SQLiteCommand(CreateMovieTable, _connection))
                    {
                        secondCommand.ExecuteNonQuery();
                    }
                }

                Console.WriteLine("Database Connected!");
            }
            catch (ArgumentException ex)
            {
                log.SaveInLog(ex.Message);
            }
            catch (Exception ex)
            {
                log.SaveInLog(ex.Message);
            }
        }

        // Method to insert movie data into the database
        public void InsertData(MovieModel movie, string Email)
        {
            string InsertSQLCommand = "INSERT INTO Movies (Title, Genre, ReleaseYear, UserID) VALUES (@Title, @Genre, @ReleaseYear, @UserID)";
            _Id = GetUserID(Email);

            using (SQLiteCommand command = new SQLiteCommand(InsertSQLCommand, _connection))
            {
                command.Parameters.AddWithValue("@Title", movie.Name);
                command.Parameters.AddWithValue("@Genre", movie.Genre);
                command.Parameters.AddWithValue("@ReleaseYear", movie.ReleaseYear);
                command.Parameters.AddWithValue("@UserID", _Id);

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

        // Method to retrieve and display movies from the database for a specific user
        public void ShowMoviesDB(string Email)
        {
            string SQLiteCommand = "SELECT * FROM Movies WHERE UserID = @UserID";

            _Id = GetUserID(Email);

            try
            {
                using (SQLiteCommand command = new SQLiteCommand(SQLiteCommand, _connection))
                {
                    command.Parameters.AddWithValue("@UserID", _Id);
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string MovieName = reader["Title"].ToString() ?? string.Empty;
                            string MovieGenre = reader["Genre"].ToString() ?? string.Empty;
                            int MovieYear = Convert.ToInt32(reader["ReleaseYear"]);

                            Console.WriteLine($"Name: {MovieName}, Genre: {MovieGenre}, Release Year: {MovieYear}");
                        }
                    }
                }
            }
            catch (ArgumentException ex)
            {
                log.SaveInLog(ex.Message);
            }
            catch (Exception ex)
            {
                log.SaveInLog(ex.Message);
            }
        }

        // Method to search and display movies based on a specified criteria
        public void ShowSearchedMovie(string searchInput, string Email)
        {
            string whereClause = string.Empty;
            string parameterName = string.Empty;
            _Id = GetUserID(Email)
;
            try
            {
                if (searchInput.Length >= 2)
                {
                    switch (searchInput[0])
                    {
                        case 'T':
                            whereClause = "Title = @Title";
                            parameterName = "@Title";
                            break;
                        case 'G':
                            whereClause = "Genre = @Genre";
                            parameterName = "@Genre";
                            break;
                        case 'R':
                            whereClause = "ReleaseYear = @ReleaseYear";
                            parameterName = "@ReleaseYear";
                            break;
                        default:
                            Console.WriteLine("Invalid search input.");
                            return;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid search input.");
                }
            }
            catch (ArgumentException ex)
            {
                log.SaveInLog(ex.Message);
            }
            catch (Exception ex)
            {
                log.SaveInLog(ex.Message);
            }


            int searchValue;
            int.TryParse(searchInput.Substring(2), out searchValue);

            string sqlCommand = $"SELECT * FROM Movies WHERE {whereClause} AND UserID = @UserID";

            try
            {
                using (SQLiteCommand command = new SQLiteCommand(sqlCommand, _connection))
                {
                    command.Parameters.AddWithValue("@UserID", _Id);
                    if (searchInput[0] == 'T' || searchInput[0] == 'G')
                    {
                        command.Parameters.AddWithValue(parameterName, searchInput.Substring(2));
                    }
                    else if (searchInput[0] == 'R')
                    {
                        command.Parameters.AddWithValue(parameterName, searchValue);
                    }

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string movieName = reader["Title"].ToString() ?? string.Empty;
                            string Genre = reader["Genre"].ToString() ?? string.Empty;
                            int ReleaseYear = Convert.ToInt32(reader["ReleaseYear"]);

                            Console.WriteLine($"Title: {movieName}, Genre: {Genre}, Release Year: {ReleaseYear}");
                        }
                    }
                }
            }
            catch (ArgumentException ex)
            {
                log.SaveInLog(ex.Message);
            }
            catch (Exception ex)
            {
                log.SaveInLog(ex.Message);
            }
        }

        // Method to edit movie information in the database
        public void EditMovies(MovieModel movie, string Email)
        {
            Console.Write("Insert the Name of the movie that you want do edit: ");
            string MovieChoice = Console.ReadLine() ?? string.Empty;
            _Id = GetUserID(Email);

            string updateSqlCommand = "UPDATE Movies SET Title = @NewName, Genre = @Genre, ReleaseYear = @ReleaseYear WHERE Title = @OldName AND UserID = @UserID";

            try
            {
                using (SQLiteCommand command = new SQLiteCommand(updateSqlCommand, _connection))
                {
                    command.Parameters.AddWithValue("@OldName", MovieChoice);
                    command.Parameters.AddWithValue("@UserID", _Id);
                    command.Parameters.AddWithValue("@NewName", movie.Name);
                    command.Parameters.AddWithValue("@Genre", movie.Genre);
                    command.Parameters.AddWithValue("@ReleaseYear", movie.ReleaseYear);

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
            catch (InvalidOperationException ex)
            {
                log.SaveInLog(ex.Message);
            }
            catch (ArgumentException ex)
            {
                log.SaveInLog(ex.Message);
            }
            catch (Exception ex)
            {
                log.SaveInLog(ex.Message);
            }
        }

        // Method to delete a movie from the database
        public void DeleteMovie(string Email)
        {
            Console.Write("Name of the movie that you want to delete: ");
            string MovieName = Console.ReadLine() ?? string.Empty;

            _Id = GetUserID(Email);

            string SQLiteDeleteCommand = "DELETE FROM Movies WHERE Title = @Title AND UserID = @UserID";

            try
            {
                using (SQLiteCommand command = new SQLiteCommand(SQLiteDeleteCommand, _connection))
                {
                    command.Parameters.AddWithValue("@Title", MovieName);
                    command.Parameters.AddWithValue("@UserID", _Id);

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
            catch (ArgumentException ex)
            {
                log.SaveInLog(ex.Message);
            }
            catch (Exception ex)
            {
                log.SaveInLog(ex.Message);
            }
        }

        // Method to create a user account in the database
        public void CreateAccount(AccountModel Account)
        {
            PasswordManager passwordManager = new PasswordManager();

            string SQLiteCreateCommand = "INSERT INTO User (Name, Email, Password) VALUES (@Name, @Email, @Password)";
            try
            {
                using (SQLiteCommand command = new SQLiteCommand(SQLiteCreateCommand, _connection))
                {
                    command.Parameters.AddWithValue("@Name", Account.Name);
                    command.Parameters.AddWithValue("@Email", Account.Email);
                    command.Parameters.AddWithValue("@Password", passwordManager.HashPassword(Account));

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
            catch (ArgumentException ex)
            {
                log.SaveInLog(ex.Message);
            }
            catch (Exception ex)
            {
                log.SaveInLog(ex.Message);
            }
        }

        // Method to check if an email already exists in the database
        public bool CheckIfEmailExists(string Email)
        {
            string SQLiteCommand = "SELECT * FROM User WHERE Email = @Email";

            try
            {
                using (SQLiteCommand command = new SQLiteCommand(SQLiteCommand, _connection))
                {
                    command.Parameters.AddWithValue("@Email", Email);
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return true;
                        }
                        else
                        {
                            Console.WriteLine("Email Available!");
                        }
                    }
                }
            }
            catch (ArgumentException ex)
            {
                log.SaveInLog(ex.Message);
            }
            catch (Exception ex)
            {
                log.SaveInLog(ex.Message);
            }

            return false;
        }

        // Method to retrieve hashed password for a specified email from the database
        public string CheckPassword(string Password, string Email)
        {
            _password = Password;

            string SQLCommand = "SELECT * FROM User WHERE Email = @Email";

            try
            {
                using (SQLiteCommand command = new SQLiteCommand(SQLCommand, _connection))
                {
                    command.Parameters.AddWithValue("@Email", Email);
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            _password = reader["Password"].ToString() ?? "";
                            return _password;
                        }
                    }
                }
            }
            catch (ArgumentException ex)
            {
                log.SaveInLog(ex.Message);
            }
            catch (Exception ex)
            {
                log.SaveInLog(ex.Message);
            }

            return "Password not finded!";
        }

        // Method to get the User ID for a specified email
        private int GetUserID(string Email)
        {
            string SQLiteSelectCommand = "SELECT * FROM User WHERE Email = @Email";

            try
            {
                using (SQLiteCommand command = new SQLiteCommand(SQLiteSelectCommand, _connection))
                {
                    command.Parameters.AddWithValue("@Email", Email);
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            _Id = Convert.ToInt32(reader["UserID"]);
                            return (int)_Id;
                        }
                    }
                }
            }
            catch (ArgumentException ex)
            {
                log.SaveInLog(ex.Message);
            }
            catch (Exception ex)
            {
                log.SaveInLog(ex.Message);
            }
            return -1;
        }

        // Method to close the database connection
        public void CloseDatabase()
        {
            _connection.Close();
            Dispose();
        }

    }
}
