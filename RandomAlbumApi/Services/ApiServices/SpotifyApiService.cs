using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using System.Threading;
using RandomAlbumApi.Models;
using RandomAlbumApi.Services.AuthServices.Spotify;
using Microsoft.AspNetCore.Authentication;

public class SpotifyService
{

    private readonly ISpotifyClient _client;
    private readonly IConfiguration _configuration;
    
    private readonly string _spotifyClientId;
    private readonly string _spotifyClientSecret;

    private const string _baseUrl = "https://api.spotify.com/v1/search";


    public SpotifyService(ISpotifyClient client, IConfiguration configuration)
    {
        _client = client;
        _configuration = configuration;

        _spotifyClientId = _configuration["spotifyClientId"];
        _spotifyClientSecret = _configuration["spotifyClientSecret"];
    }

    public async Task<string> GetToken()
    {
        var token = await _client.GetAccessToken(_spotifyClientId, _spotifyClientSecret);
        return token;
    }

    public async Task<string> GetAllAlbums(string artistName, string accessToken)
    {
        List<SpotifyAlbum> albums = new List<SpotifyAlbum>();

        string query = "%2520artist%3AMiles%2520Davis&type=album&locale=en-US&offset=0&limit=20";

        using (HttpClient httpClient = new HttpClient())
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await httpClient.GetAsync($"{_baseUrl}?query={query}");
            response = response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            return jsonResponse;
        }    
           
    }
}