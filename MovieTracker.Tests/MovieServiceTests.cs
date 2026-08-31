namespace MovieTracker.Tests;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using MovieTracker.Data;
using MovieTracker.Models;
using MovieTracker.Services;

public class MovieServiceTests
{
    private static (MovieDbContext Context, SqliteConnection Connection) CreateTestContext()
    {
        var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<MovieDbContext>()
            .UseSqlite(connection)
            .Options;

        var context = new MovieDbContext(options);
        context.Database.EnsureCreated();

        return (context, connection);
    }

    [Fact]
    public void GetAll_EmptyDatabase_ReturnsEmptyList()
    {
        var (context, connection) = CreateTestContext();

        using (connection)
        using (context)
        {
            // Arrange
            var movieService = new MovieService(context);

            // Act
            var result = movieService.GetAll();

            // Assert
            Assert.Empty(result);
        }
    }

    [Fact]
    public void GetAll_MultipleMovies_ReturnsAllMovies()
    {
        var (context, connection) = CreateTestContext();

        using (connection)
        using (context)
        {
            // Arrange
            context.Movies.AddRange(
                new Movie
                {
                    Title = "The Odyssey",
                    ReleaseYear = 2026,
                    Watched = true,
                    Rating = 10
                },
                new Movie
                {
                    Title = "Arrival",
                    ReleaseYear = 2016,
                    Watched = true,
                    Rating = 9
                }
            );

            context.SaveChanges();

            var movieService = new MovieService(context);

            // Act
            var result = movieService.GetAll();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, movie => movie.Title == "The Odyssey");
            Assert.Contains(result, movie => movie.Title == "Arrival");
        }
    }

    [Fact]
    public void GetById_ExistingId_ReturnsMovie()
    {
        var (context, connection) = CreateTestContext();

        using (connection)
        using (context)
        {
            // Arrange
            var movieToFind = new Movie
            {
                Title = "The Odyssey",
                ReleaseYear = 2026,
                Watched = true,
                Rating = 10
            };

            context.Movies.Add(movieToFind);
            context.SaveChanges();

            var movieService = new MovieService(context);

            // Act
            var result = movieService.GetById(movieToFind.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(movieToFind.Id, result.Id);
            Assert.Equal(movieToFind.Title, result.Title);
            Assert.Equal(movieToFind.ReleaseYear, result.ReleaseYear);
            Assert.Equal(movieToFind.Watched, result.Watched);
            Assert.Equal(movieToFind.Rating, result.Rating);
        }
    }

    [Fact]
    public void GetById_NonexistentId_ReturnsNull()
    {
        var (context, connection) = CreateTestContext();

        using (connection)
        using (context)
        {
            // Arrange
            var movieService = new MovieService(context);

            // Act
            var result = movieService.GetById(67);

            // Assert
            Assert.Null(result);
        }
    }

    [Fact]
    public void Add_ValidMovie_AddsAndReturnsMovie()
    {
        var (context, connection) = CreateTestContext();

        using (connection)
        using (context)
        {
            // Arrange
            var movieService = new MovieService(context);

            var movieToAdd = new Movie
            {
                Id = 1234567,
                Title = "The Odyssey",
                ReleaseYear = 2026,
                Watched = true,
                Rating = 10
            };

            // Act
            var result = movieService.Add(movieToAdd);

            // Assert
            Assert.NotEqual(1234567, result.Id);
            Assert.True(result.Id > 0);

            var storedMovie = movieService.GetById(result.Id);

            Assert.NotNull(storedMovie);
            Assert.Equal(result.Id, storedMovie.Id);
            Assert.Equal(result.Title, storedMovie.Title);
            Assert.Equal(result.ReleaseYear, storedMovie.ReleaseYear);
            Assert.Equal(result.Watched, storedMovie.Watched);
            Assert.Equal(result.Rating, storedMovie.Rating);
        }
    }

    [Fact]
    public void Update_ExistingId_UpdatesAndReturnsMovie()
    {
        var (context, connection) = CreateTestContext();

        using (connection)
        using (context)
        {
            // Arrange
            var movieToAdd = new Movie
            {
                Title = "The Odyssey",
                ReleaseYear = 2026,
                Watched = false,
                Rating = null
            };

            context.Movies.Add(movieToAdd);
            context.SaveChanges();

            var movieService = new MovieService(context);

            var movieToUpdate = new Movie
            {
                Id = 999,
                Title = "The Odyssey",
                ReleaseYear = 2026,
                Watched = true,
                Rating = 8
            };

            // Act
            var result = movieService.Update(movieToAdd.Id, movieToUpdate);

            // Assert
            Assert.NotNull(result);

            Assert.Equal(movieToAdd.Id, result.Id);
            Assert.NotEqual(movieToUpdate.Id, result.Id);
            Assert.Equal(movieToUpdate.Title, result.Title);
            Assert.Equal(movieToUpdate.ReleaseYear, result.ReleaseYear);
            Assert.Equal(movieToUpdate.Watched, result.Watched);
            Assert.Equal(movieToUpdate.Rating, result.Rating);

            var storedMovie = movieService.GetById(result.Id);

            Assert.NotNull(storedMovie);
            Assert.Equal(result.Id, storedMovie.Id);
            Assert.Equal(result.Title, storedMovie.Title);
            Assert.Equal(result.Rating, storedMovie.Rating);
            Assert.True(storedMovie.Watched);
        }
    }

    [Fact]
    public void Update_NonexistentId_ReturnsNull()
    {
        var (context, connection) = CreateTestContext();

        using (connection)
        using (context)
        {
            // Arrange
            var movieService = new MovieService(context);

            var movieToUpdate = new Movie
            {
                Title = "The Odyssey",
                ReleaseYear = 2026,
                Watched = true,
                Rating = 8
            };

            // Act
            var result = movieService.Update(67, movieToUpdate);

            // Assert
            Assert.Null(result);
        }
    }

    [Fact]
    public void Delete_ExistingId_ReturnsTrueAndDeletesMovie()
    {
        var (context, connection) = CreateTestContext();

        using (connection)
        using (context)
        {
            // Arrange
            var movieToDelete = new Movie
            {
                Title = "The Odyssey",
                ReleaseYear = 2026,
                Watched = false,
                Rating = null
            };

            context.Movies.Add(movieToDelete);
            context.SaveChanges();

            var movieService = new MovieService(context);

            // Act
            var result = movieService.Delete(movieToDelete.Id);

            // Assert
            Assert.True(result);

            var deletedMovie = movieService.GetById(movieToDelete.Id);
            Assert.Null(deletedMovie);
        }
    }

    [Fact]
    public void Delete_NonexistentId_ReturnsFalse()
    {
        var (context, connection) = CreateTestContext();

        using (connection)
        using (context)
        {
            // Arrange
            var movieService = new MovieService(context);

            // Act
            var result = movieService.Delete(67);

            // Assert
            Assert.False(result);
        }
    }
}