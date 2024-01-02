using DataBase;

namespace MoviesLibrary
{
    public class MoviesManager
    {
        MovieModel movieModel = new MovieModel();
        FileHandling log = new FileHandling();
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
                        Console.Write($"Insert only valid numbers beetwen 1500 and {ActualYear}: ");
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

        public string SearchMovies()
        {
            Console.WriteLine("How do you want to search?\n1 - By Title\n2 - By Genre\n3 - By Year");
            if(!int.TryParse(Console.ReadLine(), out int Choice))
            {
                Console.WriteLine("Insert only numbers!");
            }

            switch(Choice)
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
                        Console.Write($"Please, insert only valid numbers beetwen 1500 and {ActualYear}: ");
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
