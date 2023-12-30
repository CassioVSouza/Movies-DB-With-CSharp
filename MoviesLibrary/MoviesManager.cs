using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesLibrary
{
    public class MoviesManager
    {
        MovieModel movieModel = new MovieModel();
        public MovieModel NewMovie()
        {
            do
            {
                Console.Write("Name of the movie: ");
                string Name = Console.ReadLine() ?? "";

                if (!string.IsNullOrWhiteSpace(Name))
                {
                    movieModel.Name = Name;
                    break;
                }
                else
                {
                    Console.WriteLine("You need to write something!");
                }
            } while (true);

            do
            {
                Console.Write("Rating of the movie: ");
                if (int.TryParse(Console.ReadLine(), out int Rated) && CheckingRate(Rated))
                {
                    movieModel.Rated = Rated;
                    break;
                }
            } while (true);

            do
            {
                Console.Write("Score of the movie: ");
                if (int.TryParse(Console.ReadLine(), out int Score) && Score >= 0 && Score <= 100)
                {
                    movieModel.Score = Score;
                    break;
                }
                else
                {
                    Console.WriteLine("Insert only valid numbers beetwen 0 and 100!");
                }
            } while (true);


            return movieModel;
        }

        private bool CheckingRate(int Rated)
        {
            if(Rated == 10 || Rated == 12 || Rated == 14 || Rated == 16 || Rated == 18)
            {
                return true;
            }
            else
            {
                Console.WriteLine("Insert only valid numbers! They are: 10, 12, 14, 16, 18");
            }
            return false;
        }

        public string SearchMovies()
        {
            Console.WriteLine("How do you want to search?\n1 - By Name\n2 - By Rating\n3 - By Score");
            if(!int.TryParse(Console.ReadLine(), out int Choice))
            {
                Console.WriteLine("Insert only numbers!");
            }

            switch(Choice)
            {
                case 1:
                    return SearchByName();
                
                case 2:
                    return SearchByRating();

                case 3:
                    return SearchByScore();

                default:
                    Console.WriteLine("Insert a valid number!");
                    break;
            }
            return "Failed Search";
        }

        private string SearchByName()
        {
            do
            {
                Console.Write("Name: ");
                string Name = Console.ReadLine() ?? "";
                if (string.IsNullOrWhiteSpace(Name))
                {
                    Console.WriteLine("Please, insert something!");
                }
                else
                {
                    return "N " + Name;
                }
            } while (true);
        }

        private string SearchByRating()
        {
            do
            {
                Console.Write("Rating: ");
                if(int.TryParse(Console.ReadLine(), out int Rated) && CheckingRate(Rated))
                {
                    string StringRating = "R " + Rated.ToString();
                    return StringRating;
                }
                else
                {
                    Console.WriteLine("Please, insert only numbers!");
                }
            } while (true);
        }

        private string SearchByScore()
        {
            do
            {
                Console.Write("Score: ");
                if (int.TryParse(Console.ReadLine(), out int Score) && Score >= 0 && Score <= 100)
                {
                    string StringScore = "S " + Score.ToString();
                    return StringScore;
                }
                else
                {
                    Console.WriteLine("Please, insert only valid numbers beetwen 0 and 100!");
                }
            } while (true);
        }
    }
}
