using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using System.Threading;
using RandomAlbumApi.Models;
using RandomAlbumApi.Services.AuthServices.Spotify;
using Microsoft.AspNetCore.Authentication;
using Newtonsoft.Json;
using System.Text;
using RandomAlbumApi.Services.ApiServices.Spotify;

public class SpotifyApiService
{

    private readonly ISpotifyClient _client;
    private readonly IConfiguration _configuration;
    private readonly Serilog.ILogger _logger;
    
    private readonly string _spotifyClientId;
    private readonly string _spotifyClientSecret;

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
        var token = await _client.GetAccessToken(_spotifyClientId, _spotifyClientSecret);
        return token;
    }

    public async Task<List<AlbumDto>> GetAllAlbums(string artistName, string accessToken)
    {
        List<AlbumDto> albums = new List<AlbumDto>();
        const int Limit = 50;
        int offset = 0;
        int total = 0;

        do
        {
            _logger.Information($"Loop fired: Offset={offset} Total={total}");
            //string query = $"%2520artist%3A{firstName}%2520{lastName}&type=album&locale=en-US&offset={offset}&limit={Limit}";

            string query = Uri.EscapeDataString($"artist:{artistName}"); // Format the query string
            string searchUrl = $"https://api.spotify.com/v1/search?q={query}&type=album&locale=en-US&offset={offset}&limit={Limit}";

            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var response = await httpClient.GetAsync(searchUrl);
                response = response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                SpotifyApiResponse deserializedSpotifyData = JsonConvert.DeserializeObject<SpotifyApiResponse>(jsonResponse);

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

    public List<AlbumDto> FilterAlbumsByType(SpotifyApiResponse deserializedSpotifyData, string artistName)
    {
        var filteredAlbums = new List<AlbumDto>();

        var albums = deserializedSpotifyData.Albums.Items;

        foreach (var album in albums)
        {
            if (album.AlbumType == "album" && album.Artists.Any(x => x.Name == artistName))
            {
                var image = album.Images.Where(image => image.Height == 640 && image.Width == 640).Select(image => image.Url).FirstOrDefault();
                List<ArtistDto> artists = album.Artists.Select(spotifyArtist => new ArtistDto
                {
                    Name = spotifyArtist.Name,
                    Type = spotifyArtist.Type,
                    SpotifyId = spotifyArtist.Id,
                }).ToList();


                var filteredAlbum = new AlbumDto()
                {
                    Name = album.Name,
                    Artists = artists,
                    ReleaseDate = album.ReleaseDate,
                    TotalTracks = album.TotalTracks,
                    ImageUrl = image,
                    SpotifyId = album.Id,
                };
                filteredAlbums.Add(filteredAlbum);
            }
        }
        return filteredAlbums;
    }
}