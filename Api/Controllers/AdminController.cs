using Api.Data;
using Api.Services.ApiServices.Spotify;
using Api.Services;
using Microsoft.AspNetCore.Mvc;
using Api.Services.ApiServices;
using System.Net;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [Route("api/admin")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private MusicDbContext _db {get; set;}
        private SpotifyApiService _spotifyApiService {get; set;}
        private GptApiService _openAiService { get; set; }
        private PopulateDbService _populateDbService { get; set; }

        public AdminController(MusicDbContext db, SpotifyApiService spotifyClient, GptApiService openAiService, PopulateDbService populateDbService)
        {
            _db = db;
            _spotifyApiService = spotifyClient;
            _openAiService = openAiService;
            _populateDbService = populateDbService;
        }

        [HttpPost("create-discography")]
        public async Task<IActionResult> CreateDiscographyFromArtist([FromBody] string artistName)
        {
            var spotifyAlbums = await _spotifyApiService.GetSpotifyAlbums(artistName);
            if (!spotifyAlbums.Any())
            {
                return NotFound($"Could not find any albums by {artistName} on Spotify.");
            }

            var (discoTransaction, processedAlbums, batchError) = await _openAiService.BatchProcessAlbums(spotifyAlbums, artistName);
            if (batchError)
            {

                return BadRequest($"Batch Error: {discoTransaction.ErrorMessage}");
            }

            await _populateDbService.PopulateAlbumAsync(discoTransaction, processedAlbums);
            return Ok(processedAlbums);
        }
    }
}
