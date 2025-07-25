using Api.Domain.Entities;
using Newtonsoft.Json;

namespace Api.Models.DTOs.InternalDTOs
{
    public class ArtistProcessingDto
    {
        //spotify fields
        public int Id { get; set; }
        public string Name { get; set; }
        public List<string>? Genres { get; set; }
        public string ImageUrl { get; set; }
        public int SpotifyPopularity { get; set; }
        public string SpotifyId { get; set; }

        //gpt fields
        public string Biography { get; set; }

        [JsonProperty("birth_year")]
        public string? BirthYear { get; set; }

        [JsonProperty("death_year")]
        public string? DeathYear { get; set; }
        public string Instrument { get; set; }
        [JsonProperty("related_artists")]
        public List<string> RelatedArtists { get; set; } = new List<string>();
        public List<string> Influences { get; set; } = new List<string>();

        public List<AlbumArtist> AlbumArtists { get; set; } = new List<AlbumArtist>();
    }
}
