using DataBase;

namespace MoviesLibrary
{
    /// <summary>
    /// Class responsible for managing movie-related operations, such as creating a new movie, searching for movies, and handling user inputs.
    /// </summary>
    public class MoviesManager
    {
        // MovieModel instance to store movie information
        MovieModel movieModel = new MovieModel();

        // FileHandling instance for logging
        FileHandling log = new FileHandling();

        /// <summary>
        /// Creates and returns a new MovieModel instance by gathering input from the user.
        /// </summary>
        /// <returns>A MovieModel instance representing the new movie.</returns>
        public MovieModel NewMovie()
        {
            Console.Write("Title of the movie: ");
            movieModel.Name = GetInputString();

            Console.Write("Genre of the movie: ");
            movieModel.Genre = GetInputString();

            Console.Write("Release year of the movie: ");
            movieModel.ReleaseYear = GetInputInt();

            return movieModel;
        }

        /// <summary>
        /// Reads a string input from the user, ensuring it is not empty or whitespace.
        /// </summary>
        /// <returns>The non-empty and non-whitespace string entered by the user.</returns>
        private string GetInputString()
        {
            try
            {
                do
                {
                    string Input = Console.ReadLine() ?? "";

                    if (!string.IsNullOrWhiteSpace(Input))
                    {
                        return Input;
                    }
                    else
                    {
                        Console.Write("You need to write something: ");
                    }
                } while (true);
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
            return "Error";
        }

        /// <summary>
        /// Reads an integer input from the user, ensuring it is within a valid range.
        /// </summary>
        /// <returns>The valid integer entered by the user.</returns>
        private int GetInputInt()
        {
            int ActualYear = DateTime.Now.Year;
            try
            {
                do
                {
                    int.TryParse(Console.ReadLine() ?? "", out int Input);

                    if (Input >= 1500 && Input <= ActualYear)
                    {
                        return Input;
                    }
                    else
                    {
                        Console.Write($"Insert only valid numbers between 1500 and {ActualYear}: ");
                    }
                } while (true);
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
            return 0;
        }

        /// <summary>
        /// Prompts the user for search criteria (title, genre, or release year) and returns a formatted search string.
        /// </summary>
        /// <returns>A formatted search string based on user input.</returns>
        public string SearchMovies()
        {
            Console.WriteLine("How do you want to search?\n1 - By Title\n2 - By Genre\n3 - By Year");
            if (!int.TryParse(Console.ReadLine(), out int Choice))
            {
                Console.WriteLine("Insert only numbers!");
            }

            switch (Choice)
            {
                case 1:
                    Console.Write("Title: ");
                    return SearchString("T ");

                case 2:
                    Console.Write("Genre: ");
                    return SearchString("G ");

                case 3:
                    Console.Write("Release Year: ");
                    return SearchInt("R ");

                default:
                    Console.WriteLine("Insert a valid number!");
                    break;
            }
            return "Failed Search";
        }

        /// <summary>
        /// Reads a string input from the user, ensuring it is not empty or whitespace.
        /// </summary>
        /// <param name="searchType">The type of search being performed (e.g., Title, Genre).</param>
        /// <returns>A formatted search string based on user input.</returns>
        private string SearchString(string searchType)
        {
            try
            {
                do
                {
                    string Input = Console.ReadLine() ?? "";
                    if (string.IsNullOrWhiteSpace(Input))
                    {
                        Console.Write("Please, insert something!: ");
                    }
                    else
                    {
                        return searchType + Input;
                    }
                } while (true);
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
            return "Error";
        }

        /// <summary>
        /// Reads an integer input from the user, ensuring it is within a valid range.
        /// </summary>
        /// <param name="searchType">The type of search being performed (e.g., Release Year).</param>
        /// <returns>A formatted search string based on user input.</returns>
        private string SearchInt(string searchType)
        {
            int ActualYear = DateTime.Now.Year;
            try
            {
                do
                {
                    if (int.TryParse(Console.ReadLine(), out int Input) && Input >= 1500 && Input <= ActualYear)
                    {
                        string StringScore = searchType + Input.ToString();
                        return StringScore;
                    }
                    else
                    {
                        Console.Write($"Please, insert only valid numbers between 1500 and {ActualYear}: ");
                    }
                } while (true);
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

            return "Error";
        }
    }
}
