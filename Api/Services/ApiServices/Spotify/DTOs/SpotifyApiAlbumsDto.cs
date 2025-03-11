using Newtonsoft.Json;

namespace Api.Services.ApiServices.Spotify.DTOs
{
    public class SpotifyApiAlbumsDto
    {
        public SpotifyAlbumsResponseData Albums { get; set; }
    }

    public class SpotifyAlbumsResponseData
    {
        public string Href { get; set; }
        public List<SpotifyAlbum> Items { get; set; }
        public int Limit { get; set; }
        public int Offset { get; set; }
        public int Total { get; set; }
    }

    public class SpotifyAlbum
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("album_type")]
        public string AlbumType { get; set; }

        [JsonProperty("artists")]
        public List<SpotifyArtist> Artists { get; set; }

        [JsonProperty("images")]
        public List<SpotifyImage> Images { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("release_date")]
        public string ReleaseDate { get; set; }

        [JsonProperty("total_tracks")]
        public int TotalTracks { get; set; }
    }


    public class SpotifyArtist
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class SpotifyImage
    {
        public int Height { get; set; }
        public int Width { get; set; }
        public string Url { get; set; }

    }

    public class SpotifyArtistApiResponse
    {


        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("genres")]
        public List<string> Genres { get; set; }

        [JsonProperty("images")]
        public List<SpotifyImage> Images { get; set; }

        [JsonProperty("popularity")]
        public int PopularityScore { get; set; }

    }
}
