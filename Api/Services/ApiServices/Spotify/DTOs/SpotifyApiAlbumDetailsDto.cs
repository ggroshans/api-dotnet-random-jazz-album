using Newtonsoft.Json;

namespace Api.Services.ApiServices.Spotify.DTOs
{
    public class SpotifyApiAlbumDetailsDto
    {
        [JsonProperty("popularity")]
        public int PopularityScore { get; set; }

        [JsonProperty("label")]
        public string Label { get; set; }
    }
}
