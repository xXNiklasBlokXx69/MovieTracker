using MovieTracker.Models;
using MovieTracker.Services;
public interface IMovieService
{
    public List<Movie> GetAll();
    public Movie? GetById(int id);
    public Movie Add(Movie movie);
    public Movie? Update(int id, Movie movie);
    public bool Delete(int id);

}