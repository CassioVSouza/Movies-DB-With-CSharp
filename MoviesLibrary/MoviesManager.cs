using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesLibrary
{
    public class MoviesManager
    {
        MovieModel movie = new MovieModel();
        public MovieModel NewMovie()
        {
            string Name;
            int Rated, Score;
            Console.Write("Name of the movie: ");
            Name = Console.ReadLine() ?? string.Empty;

            Console.Write("Rating of the movie: ");
            Rated = Convert.ToInt32(Console.ReadLine());

            Console.Write("Score of the movie: ");
            Score = Convert.ToInt32(Console.ReadLine());

            movie.Name = Name;
            movie.Rated = Rated;
            movie.Score = Score;

            return movie;
        }

    }
}
