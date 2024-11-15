using Microsoft.AspNetCore.Mvc;
using RandomAlbumApi.Services.ApiServices;
using RandomAlbumApi.Services.AuthServices.Spotify;
using RandomAlbumApi.Models;
using Serilog;
using RandomAlbumApi.Data;
using Api.Services;

namespace RandomAlbumApi.Controllers
{

    [Route("api/Album")]
    [ApiController]
    public class AlbumsController : ControllerBase
    {
        
        private readonly Serilog.ILogger _logger;
        private readonly SpotifyApiService _spotifyApiService;
        public readonly GptApiService _openAIservice;
        private readonly PopulateDbService _populateDbService;



        public AlbumsController( Serilog.ILogger logger, GptApiService openAIService, SpotifyApiService spotifyApiService, PopulateDbService populateDbService)
        {
            _logger = logger;
            _openAIservice = openAIService;
            _spotifyApiService = spotifyApiService;
            _populateDbService = populateDbService;
        }

        [HttpPost("gpt")]
        public async Task<IActionResult> CreateAlbum([FromBody] AlbumRequest albumRequest)
        {

            var chatCompletionObject = _openAIservice.GetAlbumDetailAsync(albumRequest.ArtistName, albumRequest.AlbumName).Result;
            var response = chatCompletionObject.Content[0].Text;
            return Ok(response);

        }
        [HttpPost("spotify")]
        public async Task<IActionResult> CreateAlbumsFromArtist([FromBody] AlbumRequest albumRequest)
        {
            var token = await _spotifyApiService.GetToken();
            var spotifyAlbums = await _spotifyApiService.GetAllAlbums(albumRequest.ArtistName, token);
            var populatedAlbums = await _openAIservice.PopulateAlbumDetails(spotifyAlbums, albumRequest.ArtistName);
            await _populateDbService.SeedAlbumAsync(populatedAlbums);
            return Ok(populatedAlbums);
        }
    }
}
