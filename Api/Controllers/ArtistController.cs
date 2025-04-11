using Microsoft.AspNetCore.Mvc;
using Api.Services.ApiServices;
using Api.Services;
using Api.Services.ApiServices.Spotify;
using Api.Data;
using Microsoft.EntityFrameworkCore;
using Api.Domain.Entities;

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

        [HttpGet("get-artist")]
        public async Task<IActionResult> getArtist(int artistId)
        {
            var artist =  await _dbContext.Artists.FirstOrDefaultAsync(a => a.Id == artistId);

            if (artist == null)
            {
                return NotFound();
            }

            return Ok(artist);
        }
    }
} 
