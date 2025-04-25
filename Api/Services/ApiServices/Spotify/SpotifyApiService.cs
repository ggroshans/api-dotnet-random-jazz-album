using System.Net.Http.Headers;
using Api.Models.DTOs.InternalDTOs;
using Api.Services.ApiServices.Spotify.DTOs;
using Api.Services.ApiServices.Spotify.SpotifyAuthService;
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

        public async Task<List<AlbumProcessingDto>> GetSpotifyAlbums(string artistName)
        {
            // Uses 'Search For Item' spotify api call to retrieve a list of albums from search argument

            await GetToken();

            List<AlbumProcessingDto> processedAlbums = new List<AlbumProcessingDto>();
            const int Limit = 50;
            int offset = 0;
            int total = 0;

            do
            {
                _logger.Information($"Loop fired: Offset={offset} Total={total}");

                //string query = Uri.EscapeDataString($"artist:{artistName}"); // Format the query string
                string encodedQuery = Uri.EscapeDataString($"artist: {artistName}");
                //string doubleEncodedQuery = Uri.EscapeDataString(firstEncodedQuery);
                string albumsUrl = $"https://api.spotify.com/v1/search?q={encodedQuery}&type=album&locale=en-US&offset={offset}&limit={Limit}";


                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _spotifyAccessToken);
                    var albumsResponse = await httpClient.GetAsync(albumsUrl);
                    albumsResponse = albumsResponse.EnsureSuccessStatusCode();

                    var jsonResponse = await albumsResponse.Content.ReadAsStringAsync();
                    SpotifyApiAlbumsDto deserializedAlbumsData = JsonConvert.DeserializeObject<SpotifyApiAlbumsDto>(jsonResponse);

                    offset = deserializedAlbumsData.Albums.Offset;
                    total = deserializedAlbumsData.Albums.Total;
                    List<SpotifyAlbum> spotifyAlbums = deserializedAlbumsData.Albums.Items;

                    foreach (var album in spotifyAlbums)
                    {
                        if (!processedAlbums.Any(x => x.Title.Equals(album.Name, StringComparison.OrdinalIgnoreCase)))
                        {
                            var filteredAlbum = FilterAlbumProperties(album, artistName);

                            if (filteredAlbum != null)
                            {
                                var additionalAlbumDetails = await GetAdditionalAlbumDetails(album);
                                
                                filteredAlbum.Label = additionalAlbumDetails.Label;
                                filteredAlbum.PopularityScore = additionalAlbumDetails.PopularityScore;

                                processedAlbums.Add(filteredAlbum);
                            }
                        }
                    }
                }
                offset = offset + Limit;
            } while (offset < total);

            return processedAlbums;
        }

        public AlbumProcessingDto FilterAlbumProperties(SpotifyAlbum album, string artistName)
        {
            if (album.AlbumType != "album" || !album.Artists.Any(x => x.Name.ToLower() == artistName.ToLower()))
            {
                return null;
            }

            // Rest of your logic here...
            var filteredAlbum = new AlbumProcessingDto();
            var image = album.Images.Where(image => image.Height == 640 && image.Width == 640).Select(image => image.Url).FirstOrDefault();

            List<ArtistProcessingDto> artists = album.Artists.Select(spotifyArtist => new ArtistProcessingDto
            {
                Name = spotifyArtist.Name,
                SpotifyId = spotifyArtist.Id,
            }).ToList();

            filteredAlbum = new AlbumProcessingDto()
            {
                Title = album.Name,
                Artists = artists,
                ReleaseYear = album.ReleaseDate.Substring(0, 4),
                TotalTracks = album.TotalTracks,
                ImageUrl = image,
                SpotifyId = album.Id,
            };
            return filteredAlbum;
        }

        public async Task<SpotifyApiAlbumDetailsDto> GetAdditionalAlbumDetails(SpotifyAlbum album)
        {
            // Uses 'Get Album' spotify api call to retrieve more album details

            string albumDetailBaseUrl = $"https://api.spotify.com/v1/albums";
            SpotifyApiAlbumDetailsDto detailsDto = new SpotifyApiAlbumDetailsDto();

            using (HttpClient httpClient = new HttpClient())
            {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _spotifyAccessToken);
                    var albumDetailsResponse = await httpClient.GetAsync($"{albumDetailBaseUrl}/{album.Id}");
                    albumDetailsResponse = albumDetailsResponse.EnsureSuccessStatusCode();

                    var jsonDetailsResponse = await albumDetailsResponse.Content.ReadAsStringAsync();
                    detailsDto = JsonConvert.DeserializeObject<SpotifyApiAlbumDetailsDto>(jsonDetailsResponse);
            }

            return new SpotifyApiAlbumDetailsDto
            {
                Label = detailsDto.Label,
                PopularityScore = detailsDto.PopularityScore,
            };
        }

        public async Task<ArtistProcessingDto> GetSpotifyArtist(ArtistProcessingDto artist)
        {
            // Uses 'Get Artist' spotify api call to retrieve more artist details

            string searchUrl = $"https://api.spotify.com/v1/artists/{artist.SpotifyId}";
            SpotifyArtistApiResponse deserializedSpotifyData;
            ArtistProcessingDto spotifyArtist;

            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _spotifyAccessToken);
                var response = await httpClient.GetAsync(searchUrl);
                response = response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                deserializedSpotifyData = JsonConvert.DeserializeObject<SpotifyArtistApiResponse>(jsonResponse);

                spotifyArtist = new ArtistProcessingDto()
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