using Microsoft.AspNetCore.Mvc;
using Api.Services.ApiServices;
using Api.Services;
using Api.Services.ApiServices.Spotify;
using Api.DTOs;

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

        [HttpPost("spotify")]
        public async Task<IActionResult> CreateAlbumsFromArtist([FromBody] AlbumRequestDto albumRequest)
        {
            var spotifyAlbums = await _spotifyApiService.GetSpotifyAlbums(albumRequest.ArtistName);
            if (spotifyAlbums.Count == 0)
            {
                return NotFound("Zero Albums returned by spotify API");
            }
            var (discoTransactionId, processedAlbums) = await _openAIservice.BatchProcessAlbums(spotifyAlbums, albumRequest.ArtistName);
            await _populateDbService.PopulateAlbumAsync(discoTransactionId, processedAlbums);
            return Ok(processedAlbums);
        }
    }
}
