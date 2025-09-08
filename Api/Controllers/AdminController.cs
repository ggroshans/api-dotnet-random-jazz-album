using System.ComponentModel.DataAnnotations;
using Api.Data;
using Api.Models.DTOs.RequestDtos;
using Api.Services;
using Api.Services.ApiServices;
using Api.Services.ApiServices.Spotify;
using Api.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{ 

    [Route("api/admin")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private MusicDbContext _db { get; }
        private ISpotifyApiService _spotifyApiService { get; }
        private IGptApiService _openAiService { get; }
        private IPopulateDbService _populateDbService { get; }

        public AdminController(
            MusicDbContext db,
            ISpotifyApiService spotifyClient,
            IGptApiService openAiService,
            IPopulateDbService populateDbService)
        {
            _db = db;
            _spotifyApiService = spotifyClient;
            _openAiService = openAiService;
            _populateDbService = populateDbService;
        }

        /// <summary>
        /// Import a full discography for the given artist.
        /// Body: { "artistName": "Miles Davis" }
        /// </summary>
        [HttpPost("create-discography")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateDiscographyFromArtist([FromBody] CreateDiscographyRequest req)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var artistName = (req.ArtistName ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(artistName))
                return BadRequest(new { message = "artistName is required." });

            // 1) Get albums from Spotify
            var spotifyAlbums = await _spotifyApiService.GetSpotifyAlbums(artistName);
            if (spotifyAlbums == null || !spotifyAlbums.Any())
            {
                return NotFound($"Could not find any albums by {artistName} on Spotify.");
            }

            // 2) Batch process albums with your GPT/OpenAI enrichment
            var (discoTransaction, processedAlbums, batchError) =
                await _openAiService.BatchProcessAlbums(spotifyAlbums, artistName);

            if (batchError)
            {
                return BadRequest($"Batch Error: {discoTransaction?.ErrorMessage ?? "Unknown error"}");
            }

            // 3) Persist results
            await _populateDbService.PopulateAlbumAsync(discoTransaction!, processedAlbums);

            // 4) Return processed albums (or a message)
            return Ok(processedAlbums);
        }
    }
}
