using System;
using System.Net.Http.Headers;
using Api.DTOs;
using Api.Services.AuthServices.Spotify;
using Newtonsoft.Json;

namespace Api.Services.ApiServices.Spotify
{
    public class SpotifyApiService
    {

        private readonly ISpotifyClient _client;
        private readonly IConfiguration _configuration;
        private readonly Serilog.ILogger _logger;

        private readonly string _spotifyClientId;
        private readonly string _spotifyClientSecret;
        private string _spotifyAccessToken;

        private const string _baseUrl = "https://api.spotify.com/v1/search";

        public SpotifyApiService(ISpotifyClient client, IConfiguration configuration, Serilog.ILogger logger)
        {
            _client = client;
            _configuration = configuration;
            _logger = logger;

            _spotifyClientId = _configuration["spotifyClientId"];
            _spotifyClientSecret = _configuration["spotifyClientSecret"];
        }

        public async Task<string> GetToken()
        {
            _spotifyAccessToken = await _client.GetAccessToken(_spotifyClientId, _spotifyClientSecret);
            return _spotifyAccessToken;
        }

        public async Task<List<AlbumDto>> GetSpotifyAlbums(string artistName)
        {
            await GetToken();

            List<AlbumDto> albums = new List<AlbumDto>();
            const int Limit = 50;
            int offset = 0;
            int total = 0;

            do
            {
                _logger.Information($"Loop fired: Offset={offset} Total={total}");

                string query = Uri.EscapeDataString($"artist:{artistName}"); // Format the query string
                string searchUrl = $"https://api.spotify.com/v1/search?q={query}&type=album&locale=en-US&offset={offset}&limit={Limit}";

                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _spotifyAccessToken);
                    var response = await httpClient.GetAsync(searchUrl);
                    response = response.EnsureSuccessStatusCode();

                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    SpotifyAlbumsApiResponse deserializedSpotifyData = JsonConvert.DeserializeObject<SpotifyAlbumsApiResponse>(jsonResponse);

                    offset = deserializedSpotifyData.Albums.Offset;
                    total = deserializedSpotifyData.Albums.Total;

                    var filteredAlbums = FilterAlbumsByType(deserializedSpotifyData, artistName);

                    foreach (var album in filteredAlbums)
                    {
                        albums.Add(album);
                    }
                }
                offset = offset + Limit;
            } while (offset < total);

            return albums;
        }

        public List<AlbumDto> FilterAlbumsByType(SpotifyAlbumsApiResponse deserializedSpotifyData, string artistName)
        {
            var filteredAlbums = new List<AlbumDto>();

            var albums = deserializedSpotifyData.Albums.Items;

            foreach (var album in albums)
            {
                if (album.AlbumType == "album"
                    && album.Artists.Any(x => x.Name.ToLower() == artistName.ToLower())
                    && !filteredAlbums.Any(x => x.Name.Equals(album.Name, StringComparison.OrdinalIgnoreCase)))
                {
                    var image = album.Images.Where(image => image.Height == 640 && image.Width == 640).Select(image => image.Url).FirstOrDefault();

                    List<ArtistDto> artists = album.Artists.Select(spotifyArtist => new ArtistDto
                        {
                            Name = spotifyArtist.Name,
                            SpotifyId = spotifyArtist.Id,
                        }).ToList();

                    var filteredAlbum = new AlbumDto()
                    {
                        Name = album.Name,
                        Artists = artists,
                        ReleaseYear = album.ReleaseDate.Substring(0, 4),
                        TotalTracks = album.TotalTracks,
                        ImageUrl = image,
                        SpotifyId = album.Id,
                    };
                    filteredAlbums.Add(filteredAlbum);
                }
            }
            return filteredAlbums;
        }

        public async Task<ArtistDto> GetSpotifyArtist(ArtistDto artist)
        {
            string searchUrl = $"https://api.spotify.com/v1/artists/{artist.SpotifyId}";
            SpotifyArtistApiResponse deserializedSpotifyData;
            ArtistDto spotifyArtist;

            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _spotifyAccessToken);
                var response = await httpClient.GetAsync(searchUrl);
                response = response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                deserializedSpotifyData = JsonConvert.DeserializeObject<SpotifyArtistApiResponse>(jsonResponse);

                spotifyArtist = new ArtistDto()
                {
                    Name = deserializedSpotifyData.Name,
                    Genres = deserializedSpotifyData.Genres,
                    ImageUrl = deserializedSpotifyData.Images.OrderByDescending(img => img.Height).FirstOrDefault().Url,
                    PopularityScore = deserializedSpotifyData.PopularityScore,
                };
            }
            return spotifyArtist;
        }
    }
}