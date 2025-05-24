using Microsoft.AspNetCore.Mvc;
using Api.Services.ApiServices;
using Api.Services;
using Api.Services.ApiServices.Spotify;
using Api.Data;
using Microsoft.EntityFrameworkCore;
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

        [HttpGet("get-artist")]
        public async Task<IActionResult> getArtist(int artistId)
        {
            var artistEntity =  await _dbContext.Artists.FirstOrDefaultAsync(a => a.Id == artistId);
            var artistAlbums = await _dbContext.AlbumArtists.Where(aa => aa.ArtistId == artistEntity.Id).Select(a => a.Album).Distinct().ToListAsync();

            var albumScores = artistAlbums.Select(a => a.PercentileScore).ToList();

            if (artistEntity == null)
            {
                return NotFound();
            }
                var noteableAlbums = artistAlbums.OrderByDescending(a => a.PercentileScore).Take(4).Select(album => new AlbumResponseDto
            {
                Id = album.Id,
                Title = album.Title,
                Description = album.Description,
                ImageUrl = album.ImageUrl,
                Genre = album.AlbumGenres.Select(ag => new GenreResponseDto
                {
                    Id = ag.GenreType.Id,
                    Name = ag.GenreType.Name,
                    Subgenres = ag.GenreType.Subgenres.Select(sg => new SubgenreResponseDto
                    {
                        Id = sg.Id,
                        Name = sg.Name
                    }).ToList(),
                }).ToList(),
                Label = album.Label,
                Moods = album.AlbumMoods.Select(am => new MoodResponseDto
                {
                    Id = am.Mood.Id,
                    Name = am.Mood.Name,
                }).ToList(),
                PopularityScore = album.PopularityScore,
                PercentileScore = album.PercentileScore,
                PopularTracks = album.PopularTracks,
                ReleaseYear = album.ReleaseYear,
                Theme = album.AlbumTheme,
                TotalTracks = album.TotalTracks,
            }).ToList();

            var artist = new ArtistResponseDto
            {
                Id = artistEntity.Id,
                Name = artistEntity.Name,
                Biography = artistEntity.Biography,
                Genres = artistEntity.Genres,
                ImageUrl = artistEntity.ImageUrl,
                PopularityScore = artistEntity.PopularityScore,
                PercentileScore = artistEntity.PercentileScore,
                Instrument = artistEntity.Instrument,
                NoteableAlbums = noteableAlbums,
                TotalAlbums = artistAlbums.Count,
                AverageAlbumScore = (int)albumScores.Sum() / artistAlbums.Count,
                DebutYear = artistAlbums.Select(aa => aa.ReleaseYear).OrderBy(a => a).FirstOrDefault().ToString(),
            };

            return Ok(artist);
        }
    }
} 
