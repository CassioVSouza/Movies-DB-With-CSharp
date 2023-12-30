
using MoviesLibrary;
using AccountAPI;
using DataBase;
using System.Data.Entity.Migrations.Model;
using System.Data.SQLite;
using BCrypt.Net;
using System.Data.Entity;

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
}
