using MovieTracker.Models;
using MovieTracker.Services;
using MovieTracker.Data;
using Microsoft.EntityFrameworkCore;

static bool IsInvalidMovie(Movie movie)
{
    return string.IsNullOrWhiteSpace(movie.Title) || movie.ReleaseYear < 1888 || (movie.Rating != null && (movie.Rating > 10 || movie.Rating < 1));
}

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddScoped<IMovieService, MovieService>();
builder.Services.AddDbContext<MovieDbContext>(options => options.UseSqlite("Data Source=movies.db"));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/api/movies", (IMovieService iMovieService) =>
{
    return iMovieService.GetAll();
});
app.MapGet("/api/movies/{id}", (int id, IMovieService movieService) =>
{
    var rightMovie = movieService.GetById(id);
    if (rightMovie == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(rightMovie);
});
app.MapPost("/api/movies", (Movie movie, IMovieService movieService) =>
{
    if (IsInvalidMovie(movie))
    {
        return Results.BadRequest();
    }
    var newMovie = movieService.Add(movie);
    return Results.Created($"/api/movies/{newMovie.Id}", newMovie);
});
app.MapPut("/api/movies/{id}", (int id, Movie movieToUpdate, IMovieService movieService) =>
{
    if (IsInvalidMovie(movieToUpdate))
    {
        return Results.BadRequest();
    }
    var updatedMovie = movieService.Update(id, movieToUpdate);
    if (updatedMovie == null) return Results.NotFound();
    return Results.Ok(updatedMovie);
});
app.MapDelete("/api/movies/{id}", (int id, IMovieService movieService) =>
{
    var movieDeleted = movieService.Delete(id);
    if (!movieDeleted) return Results.NotFound();
    return Results.NoContent();
});

app.Run();
