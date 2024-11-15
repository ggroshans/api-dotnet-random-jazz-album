using Newtonsoft.Json;

namespace RandomAlbumApi.Models
{
    public class AlbumDto
    {
        //spotify fields
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
        [JsonProperty("popular_tracks")]
        public List<string>? PopularTracks { get; set; }
        [JsonProperty("album_theme")]
        public string? AlbumTheme { get; set; }
    }

    public class ArtistDto
    {
        public int Id { get; set; } 
        public string Name { get; set; }
        public string Type { get; set; }
        public string SpotifyId { get; set; }
    }
}
