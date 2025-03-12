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

    [Route("api/albums")]
    [ApiController]
    public class AlbumsController : ControllerBase
    {
        
        private readonly Serilog.ILogger _logger;
        private readonly SpotifyApiService _spotifyApiService;
        private readonly GptApiService _openAIservice;
        private readonly PopulateDbService _populateDbService;
        private readonly MusicDbContext _dbContext;

        public AlbumsController(Serilog.ILogger logger, GptApiService openAIService, SpotifyApiService spotifyApiService, PopulateDbService populateDbService, MusicDbContext dbContext)
        {
            _logger = logger;
            _openAIservice = openAIService;
            _spotifyApiService = spotifyApiService;
            _populateDbService = populateDbService;
            _dbContext = dbContext;
        }

        [HttpPost("populate-from-artist")]
        public async Task<IActionResult> PopulateAlbumsFromArtist([FromBody] AlbumProcessingRequestDto requestQuery)
        {
            var spotifyAlbums = await _spotifyApiService.GetSpotifyAlbums(requestQuery.ArtistName);
            if (!spotifyAlbums.Any())
            {
                return NotFound($"Could not find any albums by {requestQuery.ArtistName} on Spotify.");
            }

            var (discoTransaction, processedAlbums, batchError) = await _openAIservice.BatchProcessAlbums(spotifyAlbums, requestQuery.ArtistName);
            if (batchError) 
            {

                return BadRequest($"Batch Error: {discoTransaction.ErrorMessage}"); 
            }

            await _populateDbService.PopulateAlbumAsync(discoTransaction, processedAlbums);
            return Ok(processedAlbums);
        }


        [HttpGet("random-album")]
        public async Task<IActionResult> GetRandomAlbum()
        {
            var randomAlbum = await _dbContext.Albums
                .Include(a => a.Genre)
                .Include(a => a.AlbumArtists)
                    .ThenInclude(aa => aa.Artist)
                .Include(a => a.AlbumMoods)
                    .ThenInclude(am => am.Mood)
                .Include(a => a.AlbumSubgenres)
                    .ThenInclude(asg => asg.Subgenre)
                .OrderBy(a => Guid.NewGuid())
                .FirstOrDefaultAsync();

            var album = new AlbumResponseDto
            {
                Id = randomAlbum.Id,
                Title = randomAlbum.Title,
                Description = randomAlbum.Description,
                Genre = new GenreResponseDto 
                { 
                    Id = randomAlbum.Genre.Id, 
                    Name = randomAlbum.Genre.Name 
                },
                Theme = randomAlbum.AlbumTheme,
                ImageUrl = randomAlbum.ImageUrl,
                Label = randomAlbum.Label,
                PopularityScore = randomAlbum.PopularityScore,
                PopularTracks = randomAlbum.PopularTracks,
                ReleaseYear = randomAlbum.ReleaseYear,
                TotalTracks = randomAlbum.TotalTracks,
                Artists = randomAlbum.AlbumArtists.Select(aa => new ArtistResponseDto
                    {
                        Id = aa.Artist.Id,
                        Name = aa.Artist.Name,
                        Biography = aa.Artist.Biography,
                        Genres = aa.Artist.Genres,
                        ImageUrl = aa.Artist.ImageUrl,
                        PopularityScore = aa.Artist.PopularityScore,
                    }
                ).ToList(),
                Subgenres = randomAlbum.AlbumSubgenres.Select(asg => new SubgenreResponseDto
                {
                        Id = asg.Subgenre.Id,
                        Name = asg.Subgenre.Name
                }).ToList(),
                Moods = randomAlbum.AlbumMoods.Select(am => new MoodResponseDto 
                    {
                        Id = am.Mood.Id,
                        Name = am.Mood.Name,
                    }
                ).ToList(),
            };
            return Ok(album);
        }
    }
}
