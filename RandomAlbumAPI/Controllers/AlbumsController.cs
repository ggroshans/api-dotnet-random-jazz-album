using Microsoft.AspNetCore.Mvc;
using RandomAlbumApi.Services.ApiServices;
using RandomAlbumApi.Services.AuthServices.Spotify;
using RandomAlbumAPI.Models;

namespace RandomAlbumAPI.Controllers
{

    [Route("api/Album")]
    [ApiController]
    public class AlbumsController : ControllerBase
    {
        
        private readonly ILogger<AlbumsController> _logger;
        private readonly SpotifyService _spotifyService;
        public readonly GptApiService _openAIservice;


        public AlbumsController(ILogger<AlbumsController> logger, GptApiService openAIService, SpotifyService spotifyService)
        {
            _logger = logger;
            _openAIservice = openAIService;
            _spotifyService = spotifyService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAlbum([FromBody] AlbumRequest albumRequest)
        {
            _logger.LogInformation("FIRED: CreateAlbum endpoint was called");
            _logger.LogInformation($"Artist Name: {albumRequest.ArtistName}, Album Name: {albumRequest.AlbumName}");
            var token = await _spotifyService.GetToken();

            var albums = await _spotifyService.GetAllAlbums(albumRequest.ArtistName, token);
            //var chatCompletionObject = _openAIservice.GetAlbumDetailAsync(albumRequest.ArtistName, albumRequest.AlbumName).Result;
            //var response = chatCompletionObject.Content[0].Text;
            return Ok(albums);

        }
         

    }
}
