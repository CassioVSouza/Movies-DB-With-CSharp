
using MoviesLibrary;
using AccountAPI;
using DataBase;

namespace Main
{
    public class Program
    {
        static void Main(string[] args)
        {
            FileHandling logAndConnectionString = new FileHandling();
            string ConnectionString = logAndConnectionString.GetConnectionString();
            DataBaseManager dataBaseManager = new DataBaseManager(ConnectionString);          
            UserAuthentication userAuthentication = new UserAuthentication();
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
                dataBaseManager.CloseDatabase();
            }
        }

        public static void MoviesOptions(string ConnectionString, AccountModel accountModel)
        {
            DataBaseManager dataBaseManager = new DataBaseManager(ConnectionString);
            MoviesManager moviesManager = new MoviesManager();
            FileHandling log = new FileHandling();

            try
            {
                dataBaseManager.InitializeDataBase();
                int userOption;
                
                do
                {
                    Console.WriteLine("Welcome, What do you want to do?\n1 - Add Movie\n2 - Show Movies\n3 - Edit Movie\n4 - Delete Movie\n5 - Search Movie\n6 - Exit");
                    if (!int.TryParse(Console.ReadLine(), out userOption))
                    {
                        Console.WriteLine("Insert only numbers!");
                    }
                    
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
                dataBaseManager.CloseDatabase();
            }
        }

    }

    class UserAuthentication
    {
        private string? _password;
        private bool? _authenticated = false;

        FileHandling logAndConnectionString = new FileHandling();
        internal void RegisterUser()
        {
            string ConnectionString = logAndConnectionString.GetConnectionString();

            AccountManagement accountManagement = new AccountManagement();
            AccountModel accountModel = new AccountModel();

            DataBaseManager dataBaseManager = new DataBaseManager(ConnectionString);
            dataBaseManager.InitializeDataBase();

            try
            {
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

            dataBaseManager.CreateAccount(accountModel);
            dataBaseManager.CloseDatabase();
        }

        internal void LoginUser()
        {
            string ConnectionString = logAndConnectionString.GetConnectionString();

            AccountManagement accountManagement = new AccountManagement();
            AccountModel accountModel = new AccountModel();
            PasswordManager passwordManager = new PasswordManager();

            DataBaseManager dataBaseManager = new DataBaseManager(ConnectionString);
            dataBaseManager.InitializeDataBase();

            try
            {
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
            dataBaseManager.CloseDatabase();
        }
    }
}
