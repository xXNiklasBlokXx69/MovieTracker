namespace MovieTracker.Models;

public class Movie
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public int ReleaseYear { get; set; }
    public bool Watched { get; set; }
    public int? Rating { get; set; }
}