namespace Api.Models
{
    public class Album
    {
        //spotify fields
        public int Id { get; set; }
        public string Name { get; set; }
        public List<AlbumArtist> AlbumArtists { get; set; } = new List<AlbumArtist>();
        public string ReleaseYear { get; set; }
        public int TotalTracks { get; set; }
        public string ImageUrl { get; set; }
        public string SpotifyId { get; set; }

        //gpt fields
        public string? Description { get; set; }
        public List<AlbumGenre> AlbumGenres { get; set; } = new List<AlbumGenre>();
        public List<AlbumSubgenre>? AlbumSubgenres { get; set; } = new List<AlbumSubgenre>();
        public List<AlbumMood>? AlbumMoods { get; set; } = new List<AlbumMood>();
        public List<string>? PopularTracks { get; set; }
        public string? AlbumTheme { get; set; }
    }

    public class Artist
    {
        //spotify fields
        public int Id { get; set; }
        public string Name { get; set; }
        public List<string>? Genres { get; set; }
        public string? ImageUrl { get; set; }    
        public int PopularityScore { get; set; }
        public string SpotifyId { get; set; }

        //gpt fields
        public string Biography { get; set; }

        //relationships
        public List<AlbumArtist> AlbumArtists { get; set; } = new List<AlbumArtist>();
    }

    public class Genre
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Subgenre> Subgenres { get; set; } = new List<Subgenre>();
        public List<AlbumGenre> AlbumGenres { get; set; } = new List<AlbumGenre>();
    }

    public class Subgenre
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int GenreId { get; set; }
        public Genre Genre { get; set; }
        public List<AlbumSubgenre> AlbumSubgenres { get; set; } = new List<AlbumSubgenre>();
    }

    public class Mood
    { 
        public int Id { get; set; }
        public string Name { get; set; }    
        public List<AlbumMood> AlbumMoods { get; set; } = new List<AlbumMood>();
    }


    public class AlbumArtist
    {
        public int AlbumId { get; set; }
        public Album Album { get; set; }
        public int ArtistId { get; set; }
        public Artist Artist { get; set; }

    }

    public class AlbumGenre
    {
        public int AlbumId { get; set; }
        public Album Album { get; set; }
        public int GenreId { get; set; }
        public Genre Genre { get; set; }
    }

    public class AlbumSubgenre
    {
        public int AlbumId { get; set; }
        public Album Album { get; set; }
        public int SubgenreId { get; set; }
        public Subgenre Subgenre { get; set; }
    }

    public class AlbumMood
    {
        public int AlbumId { get; set; }
        public Album Album { get; set; }
        public int MoodId { get; set; }
        public Mood Mood { get; set; }
    }
}
