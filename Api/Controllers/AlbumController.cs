using Microsoft.AspNetCore.Mvc;
using Api.Services.ApiServices;
using Api.Services;
using Api.Services.ApiServices.Spotify;
using Api.Data;
using Microsoft.EntityFrameworkCore;
using Api.Models.DTOs.ResponseDTOs;

namespace Api.Controllers
{

    [Route("api/album")]
    [ApiController]
    public class AlbumController : ControllerBase
    {
        
        private readonly Serilog.ILogger _logger;
        private readonly SpotifyApiService _spotifyApiService;
        private readonly GptApiService _openAIservice;
        private readonly PopulateDbService _populateDbService;
        private readonly MusicDbContext _dbContext;

        public AlbumController(Serilog.ILogger logger, GptApiService openAIService, SpotifyApiService spotifyApiService, PopulateDbService populateDbService, MusicDbContext dbContext)
        {
            _logger = logger;
            _openAIservice = openAIService;
            _spotifyApiService = spotifyApiService;
            _populateDbService = populateDbService;
            _dbContext = dbContext;
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
                .OrderBy(a => Guid.NewGuid()).FirstOrDefaultAsync();

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
                PercentileScore = randomAlbum.PercentileScore,
                PopularTracks = randomAlbum.PopularTracks,
                ReleaseYear = randomAlbum.ReleaseYear,
                TotalTracks = randomAlbum.TotalTracks,
                SpotifyId = randomAlbum.SpotifyId,
                YoutubeId = randomAlbum.YoutubeId,
                AppleMusicId = randomAlbum.AppleMusicId,
                AmazonMusicId = randomAlbum.AmazonMusicId,
                PandoraId = randomAlbum.PandoraId,
                Artists = randomAlbum.AlbumArtists.Select(aa => new ArtistResponseDto
                {
                    Id = aa.Artist.Id,
                    Name = aa.Artist.Name,
                    Biography = aa.Artist.Biography,
                    Genres = aa.Artist.Genres,
                    ImageUrl = aa.Artist.ImageUrl,
                    PopularityScore = aa.Artist.PopularityScore,
                    PercentileScore = aa.Artist.PercentileScore,
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
