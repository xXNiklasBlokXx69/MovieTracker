using MovieTracker.Data;
using MovieTracker.Models;
namespace MovieTracker.Services;

public class MovieService : IMovieService
{
    private List<Movie> movieList = new List<Movie>{
        new Movie{Id = 1, Title = "The Godfather", ReleaseYear = 1972, Watched = true, Rating = 10},
        new Movie{Id = 2, Title = "Dune: Part Two", ReleaseYear = 2024, Watched = false, Rating = null}
    };
    private MovieDbContext _context;
    public MovieService(MovieDbContext context)
    {
        _context = context;
    }
    public List<Movie> GetAll()
    {
        return _context.Movies.ToList();
    }
    public Movie? GetById(int id)
    {
        return _context.Movies.Find(id);
    }
    public Movie Add(Movie movie)
    {
        var newMovie = new Movie
        {
            Title = movie.Title,
            ReleaseYear = movie.ReleaseYear,
            Watched = movie.Watched,
            Rating = movie.Rating
        };
        _context.Movies.Add(newMovie);
        _context.SaveChanges();
        return newMovie;
    }
    public Movie? Update(int id, Movie movie)
    {
        var oldMovie = _context.Movies.Find(id);
        if (oldMovie == null) return null;
        oldMovie.Title = movie.Title;
        oldMovie.ReleaseYear = movie.ReleaseYear;
        oldMovie.Watched = movie.Watched;
        oldMovie.Rating = movie.Rating;
        _context.SaveChanges();
        return oldMovie;
    }
    public bool Delete(int id)
    {
        var deletedMovie = _context.Movies.Find(id);
        if (deletedMovie == null) return false;
        _context.Movies.Remove(deletedMovie);
        _context.SaveChanges();
        return true;
    }
}