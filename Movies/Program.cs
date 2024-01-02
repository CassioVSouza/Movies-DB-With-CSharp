using System;
using MoviesLibrary;
using AccountAPI;
using DataBase;

namespace Main
{
    /// <summary>
    /// Main program class responsible for handling user interactions, authentication, and movie-related options.
    /// </summary>
    public class Program
    {
        // Entry point of the application
        static void Main(string[] args)
        {
            // Initialize FileHandling instance for logging and retrieving connection string
            FileHandling logAndConnectionString = new FileHandling();
            string ConnectionString = logAndConnectionString.GetConnectionString();

            // Initialize DataBaseManager for database operations
            DataBaseManager dataBaseManager = new DataBaseManager(ConnectionString);

            // Initialize UserAuthentication for user registration and login
            UserAuthentication userAuthentication = new UserAuthentication();

            // Initialize database
            dataBaseManager.InitializeDataBase();

            // Display user options
            Console.Write("Welcome to the system, do you want to:\n1 - Register\n2 - Login\n3 - Exit");

            // Validate user input for account choice
            if (!int.TryParse(Console.ReadLine(), out int AccountChoice))
            {
                Console.WriteLine("Please, insert only numbers!");
            }

            try
            {
                // Perform actions based on user choice
                switch (AccountChoice)
                {
                    case 1:
                        userAuthentication.RegisterUser();
                        break;

                    case 2:
                        userAuthentication.LoginUser();
                        break;

                    case 3:
                        Console.WriteLine("Exited!");
                        break;

                    default:
                        Console.WriteLine("Insert a valid number!");
                        break;
                }
            }
            catch (FormatException ex)
            {
                Console.WriteLine("Insert only numbers!");
                logAndConnectionString.SaveInLog(ex.Message);
            }
            catch (Exception ex)
            {
                logAndConnectionString.SaveInLog(ex.Message);
            }
            finally
            {
                // Close the database connection
                dataBaseManager.CloseDatabase();
            }
        }

        /// <summary>
        /// Method for handling movie-related options after user authentication.
        /// </summary>
        /// <param name="ConnectionString">The database connection string.</param>
        /// <param name="accountModel">The user's account model.</param>
        public static void MoviesOptions(string ConnectionString, AccountModel accountModel)
        {
            // Initialize DataBaseManager for movie-related database operations
            DataBaseManager dataBaseManager = new DataBaseManager(ConnectionString);
            MoviesManager moviesManager = new MoviesManager();
            FileHandling log = new FileHandling();

            try
            {
                // Initialize database
                dataBaseManager.InitializeDataBase();
                int userOption;

                // Display movie-related options in a loop
                do
                {
                    Console.WriteLine("Welcome, What do you want to do?\n1 - Add Movie\n2 - Show Movies\n3 - Edit Movie\n4 - Delete Movie\n5 - Search Movie\n6 - Exit");
                    if (!int.TryParse(Console.ReadLine(), out userOption))
                    {
                        Console.WriteLine("Insert only numbers!");
                    }

                    // Perform actions based on user choice
                    switch (userOption)
                    {
                        case 1:
                            dataBaseManager.InsertData(moviesManager.NewMovie(), accountModel.Email ?? "");
                            break;

                        case 2:
                            dataBaseManager.ShowMoviesDB(accountModel.Email ?? "");
                            break;

                        case 3:
                            dataBaseManager.ShowMoviesDB(accountModel.Email ?? "");
                            dataBaseManager.EditMovies(moviesManager.NewMovie(), accountModel.Email ?? "");
                            break;

                        case 4:
                            dataBaseManager.ShowMoviesDB(accountModel.Email ?? "");
                            dataBaseManager.DeleteMovie(accountModel.Email ?? "");
                            break;

                        case 5:
                            dataBaseManager.ShowSearchedMovie(moviesManager.SearchMovies(), accountModel.Email ?? "");
                            break;

                        case 6:
                            Console.WriteLine("Exited!");
                            break;

                        default:
                            Console.WriteLine("Insert a valid number!");
                            break;
                    }
                } while (userOption != 6);
            }
            catch (IOException ex)
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
            finally
            {
                // Close the database connection
                dataBaseManager.CloseDatabase();
            }
        }
    }

    /// <summary>
    /// Class for handling user authentication, registration, and login.
    /// </summary>
    class UserAuthentication
    {
        private string? _password;
        private bool? _authenticated = false;

        // FileHandling instance for logging and retrieving connection string
        FileHandling logAndConnectionString = new FileHandling();

        // Method for registering a new user
        internal void RegisterUser()
        {
            // Retrieve database connection string
            string ConnectionString = logAndConnectionString.GetConnectionString();

            // Initialize AccountManagement and AccountModel
            AccountManagement accountManagement = new AccountManagement();
            AccountModel accountModel = new AccountModel();

            // Initialize DataBaseManager for database operations
            DataBaseManager dataBaseManager = new DataBaseManager(ConnectionString);
            dataBaseManager.InitializeDataBase();

            try
            {
                // Gather user input for name, email, and password
                while (true)
                {
                    Console.Write("Please, insert your name: ");
                    string name = Console.ReadLine() ?? "";
                    if (accountManagement.CheckName(name))
                    {
                        accountModel.Name = name;
                        break;
                    }
                }

                while (true)
                {
                    Console.Write("Please, insert your email: ");
                    string email = Console.ReadLine() ?? "";
                    if (accountManagement.CheckPatternEmail(email) && !dataBaseManager.CheckIfEmailExists(email))
                    {
                        accountModel.Email = email;
                        break;
                    }
                }

                while (true)
                {
                    Console.Write("Please, insert your password: ");
                    string password = Console.ReadLine() ?? "";
                    if (accountManagement.CheckPatternPassword(password))
                    {
                        accountModel.Password = password;
                        dataBaseManager.CreateAccount(accountModel);
                        _authenticated = true;
                        break;
                    }
                }

                // Redirect to movie-related options upon successful registration
                if ((bool)_authenticated)
                {
                    Program.MoviesOptions(logAndConnectionString.GetConnectionString(), accountModel);
                }
            }
            catch (IOException ex)
            {
                logAndConnectionString.SaveInLog(ex.Message);
            }
            catch (ArgumentException ex)
            {
                logAndConnectionString.SaveInLog(ex.Message);
            }
            catch (Exception ex)
            {
                logAndConnectionString.SaveInLog(ex.Message);
            }

            // Create the user account and close the database connection
            dataBaseManager.CreateAccount(accountModel);
            dataBaseManager.CloseDatabase();
        }

        // Method for user login
        internal void LoginUser()
        {
            // Retrieve database connection string
            string ConnectionString = logAndConnectionString.GetConnectionString();

            // Initialize AccountManagement, AccountModel, and PasswordManager
            AccountManagement accountManagement = new AccountManagement();
            AccountModel accountModel = new AccountModel();
            PasswordManager passwordManager = new PasswordManager();

            // Initialize DataBaseManager for database operations
            DataBaseManager dataBaseManager = new DataBaseManager(ConnectionString);
            dataBaseManager.InitializeDataBase();

            try
            {
                // Gather user input for email and password
                while (true)
                {
                    Console.Write("Please, insert your email: ");
                    string email = Console.ReadLine() ?? "";
                    if (dataBaseManager.CheckIfEmailExists(email))
                    {
                        accountModel.Email = email;
                        break;
                    }
                }

                while (true)
                {
                    Console.Write("Please, insert your password: ");
                    _password = Console.ReadLine() ?? "";
                    if (passwordManager.CheckPassword(dataBaseManager.CheckPassword(_password, accountModel.Email), _password))
                    {
                        _authenticated = true;
                        break;
                    }
                }

                // Redirect to movie-related options upon successful login
                if ((bool)_authenticated)
                {
                    Program.MoviesOptions(logAndConnectionString.GetConnectionString(), accountModel);
                }
            }
            catch (IOException ex)
            {
                logAndConnectionString.SaveInLog(ex.Message);
            }
            catch (ArgumentException ex)
            {
                logAndConnectionString.SaveInLog(ex.Message);
            }
            catch (Exception ex)
            {
                logAndConnectionString.SaveInLog(ex.Message);
            }

            // Close the database connection
            dataBaseManager.CloseDatabase();
        }
    }
}
