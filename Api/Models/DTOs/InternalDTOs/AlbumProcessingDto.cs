using Api.Domain.Entities;
using Newtonsoft.Json;

namespace Api.Models.DTOs.InternalDTOs
{
    public class AlbumProcessingDto
    {
        //spotify fields
        public int Id { get; set; }
        public string Title { get; set; }
        public List<ArtistProcessingDto> Artists { get; set; }
        public string ReleaseYear { get; set; }
        public int TotalTracks { get; set; }
        public string ImageUrl { get; set; }
        public int PopularityScore { get; set; }
        public string Label { get; set; }
        public string SpotifyId { get; set; }

        //gpt fields
        public string? Description { get; set; }
        public List<GenreProcessingDto> Genres { get; set; }
        public List<string>? Subgenres { get; set; }
        public List<MoodProcessingDto>? Moods { get; set; }
        [JsonProperty("popular_tracks")]
        public List<string>? PopularTracks { get; set; }
        [JsonProperty("album_theme")]
        public string? AlbumTheme { get; set; }
    }
}
