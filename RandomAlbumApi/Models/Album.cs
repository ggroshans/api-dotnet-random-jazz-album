using Newtonsoft.Json;
using RandomAlbumApi.Models;

namespace RandomAlbumApi.Models
{

    public class AlbumDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<ArtistDto> Artists { get; set; }
        public string ReleaseDate { get; set; }
        public int TotalTracks { get; set; }
        public string ImageUrl { get; set; }
        public string SpotifyId { get; set; }

        //gpt fields
        public string? Description { get; set; }
        public string? Genre { get; set; }
        public List<string>? Subgenres { get; set; }
        public List<string>? Moods { get; set; }
        [JsonProperty("album_position")]
        public int? AlbumPosition { get; set; }
        [JsonProperty("popular_tracks")]
        public List<string>? PopularTracks { get; set; }
        [JsonProperty("album_theme")]
        public string? AlbumTheme { get; set; }
    }

    public class ArtistDto
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string SpotifyId { get; set; }
    }

    public class Album
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Artist> Artists { get; set; }
        public string ReleaseDate { get; set; }
        public int TotalTracks { get; set; }
        public string ImageUrl { get; set; }
        public string SpotifyId { get; set; }

        //gpt fields
        public string? Description { get; set; }
        public Genre? Genre { get; set; }
        public List<Subgenre>? Subgenres { get; set; }
        public List<Mood>? Moods { get; set; } = new List<Mood>();
        public int? AlbumPosition { get; set; }
        public List<string>? PopularTracks { get; set; }
        public string? AlbumTheme { get; set; }
    }

    public class Artist
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string SpotifyId { get; set; }
    }

    public class Genre
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Subgenre> Subgenres { get; set; }
    }

    public class Subgenre
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int GenreId { get; set; }
        public Genre Genre { get; set; }
    }

    public class Mood
    { 
        public int Id { get; set; }
        public string Name { get; set; }    
        public List<Album> Albums { get; set; } = new List<Album>();
    }

    public class AlbumMoods
    {
        public int AlbumId { get; set; }
        public int MoodId { get; set; }
        public Album Album { get; set; }
        public Mood Mood { get; set; } 
    }

    public class ArtistAlbums
    {
        public int ArtistId { get; set; }
        public int AlbumId { get; set; }
        public Artist Artist { get; set; }
        public Mood Mood { get; set; }
    }
}
