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
        [JsonProperty("release_date")]
        public string? ReleaseDate { get; set; }
        [JsonProperty("release_date_precision")]
        public string? ReleaseDatePrecision { get; set; }
        public int TotalTracks { get; set; }
        public string ImageUrl { get; set; }
        public int SpotifyPopularity { get; set; }
        public string Label { get; set; }
        public string SpotifyId { get; set; }

        //gpt fields
        public string? Description { get; set; }
        [JsonProperty("is_original_release")]
        public bool IsOriginalRelease { get; set; }
        [JsonProperty("jazz_eras")]
        public List<int> JazzEras { get; set; }
        public List<GenreTypeProcessingDto> Genres { get; set; }
        public List<MoodProcessingDto>? Moods { get; set; }

        //computed fields
        public int? SortableDate { get; set; }
    }
}
