using Microsoft.EntityFrameworkCore;
using MovieTracker.Models;
namespace MovieTracker.Data;

public class MovieDbContext : DbContext
{
    public MovieDbContext(DbContextOptions<MovieDbContext> dbContextOptions) : base(dbContextOptions) { }

    public DbSet<Movie> Movies
    {
        get;
        set;
    }

}