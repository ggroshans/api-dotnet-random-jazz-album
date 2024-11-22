using Microsoft.AspNetCore.Mvc;
using Api.Services.ApiServices;
using Api.Models;
using Api.Services;
using Api.Services.ApiServices.Spotify;

namespace Api.Controllers
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
            var spotifyAlbums = await _spotifyApiService.GetSpotifyAlbums(albumRequest.ArtistName);
            var gptAlbums = await _openAIservice.GetGptAlbumDetails(spotifyAlbums, albumRequest.ArtistName);
            await _populateDbService.PopulateAlbumAsync(gptAlbums);
            return Ok(gptAlbums);
        }
    }
}
