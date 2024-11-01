using Microsoft.AspNetCore.Mvc;
using RandomAlbumApi.Services.ApiServices;
using RandomAlbumApi.Services.AuthServices.Spotify;
using RandomAlbumAPI.Models;
using Serilog;

namespace RandomAlbumAPI.Controllers
{

    [Route("api/Album")]
    [ApiController]
    public class AlbumsController : ControllerBase
    {
        
        private readonly Serilog.ILogger _logger;
        private readonly SpotifyApiService _spotifyApiService;
        public readonly GptApiService _openAIservice;


        public AlbumsController( Serilog.ILogger logger, GptApiService openAIService, SpotifyApiService spotifyApiService)
        {
            _logger = logger;
            _openAIservice = openAIService;
            _spotifyApiService = spotifyApiService;
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
            return Ok(populatedAlbums);
        }
    }
}
