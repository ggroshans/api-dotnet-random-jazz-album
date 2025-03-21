using Microsoft.AspNetCore.Mvc;
using Api.Services.ApiServices;
using Api.Services;
using Api.Services.ApiServices.Spotify;
using Api.Data;
using Microsoft.EntityFrameworkCore;
using Api.Domain.Entities;
using Api.Models.DTOs.InternalDTOs;
using Api.Models.DTOs.ResponseDTOs;

namespace Api.Controllers
{

    [Route("api/artist")]
    [ApiController]
    public class ArtistController : ControllerBase
    {
        
        private readonly Serilog.ILogger _logger;
        private readonly SpotifyApiService _spotifyApiService;
        private readonly GptApiService _openAIservice;
        private readonly PopulateDbService _populateDbService;
        private readonly MusicDbContext _dbContext;

        public ArtistController(Serilog.ILogger logger, GptApiService openAIService, SpotifyApiService spotifyApiService, PopulateDbService populateDbService, MusicDbContext dbContext)
        {
            _logger = logger;
            _openAIservice = openAIService;
            _spotifyApiService = spotifyApiService;
            _populateDbService = populateDbService;
            _dbContext = dbContext;
        }


        [HttpGet("normalize-artist-scores")]
        public async Task<ActionResult> NormalizeArtistPopularityScores()
        {
            var artists = _dbContext.Artists.ToList();

            if (artists.Count == 0)
            {
                return Ok("Error retrieving artists.");
            }

            foreach (var artist in artists)
            {
                var lower = artists.Count(a => a.PopularityScore < artist.PopularityScore);
                var percentile = Math.Round((double)lower / artists.Count * 100, 0);

                artist.PercentileScore = percentile;
            }
            _dbContext.SaveChanges();
            return Ok("Scores normalized");
        }
    }
}
