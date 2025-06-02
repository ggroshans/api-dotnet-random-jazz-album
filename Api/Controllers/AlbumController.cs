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

        [HttpGet("{Id}")]
        public async Task<IActionResult> GetAlbumById(int Id)
        {
            var album = await _dbContext.Albums
                .Where(a => a.Id == Id)
                .Select(a => new AlbumResponseDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    Description = a.Description,
                    Genre = a.AlbumGenres.Select(ag => new GenreResponseDto
                    {
                        Id = ag.GenreType.Id,
                        Name = ag.GenreType.Name,
                        Subgenres = ag.GenreType.Subgenres.Select(sg => new SubgenreResponseDto
                        {
                            Id = sg.Id,
                            Name = sg.Name
                        }).ToList(),
                    }).ToList(),
                    Theme = a.AlbumTheme,
                    ImageUrl = a.ImageUrl,
                    Label = a.Label,
                    PopularityScore = a.PopularityScore,
                    PercentileScore = a.PercentileScore,
                    ReleaseYear = a.ReleaseYear,
                    TotalTracks = a.TotalTracks.HasValue ? a.TotalTracks.Value : 0, 
                    SpotifyId = a.SpotifyId,
                    YoutubeId = a.YoutubeId,
                    AppleMusicId = a.AppleMusicId,
                    AmazonMusicId = a.AmazonMusicId,
                    PandoraId = a.PandoraId,
                    Artists = a.AlbumArtists.Select(aa => new ArtistResponseDto
                    {
                        Id = aa.Artist.Id,
                        Name = aa.Artist.Name,
                        Biography = aa.Artist.Biography,
                        Genres = aa.Artist.Genres,
                        ImageUrl = aa.Artist.ImageUrl,
                        PopularityScore = aa.Artist.PopularityScore,
                        PercentileScore = aa.Artist.PercentileScore,
                    }).ToList(),
                    Moods = a.AlbumMoods.Select(am => new MoodResponseDto
                    {
                        Id = am.Mood.Id,
                        Name = am.Mood.Name,
                    }).ToList(),
                })
                .FirstOrDefaultAsync();

            if (album == null)
            {
                return NotFound();
            }

            return Ok(album);
        }

        [HttpGet("random")]
        public async Task<IActionResult> GetRandomAlbum()
        {

            var album = await _dbContext.Albums
                 .Select(a => new AlbumResponseDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    Description = a.Description,
                     Genre = a.AlbumGenres.Select(ag => new GenreResponseDto
                     {
                         Id = ag.GenreType.Id,
                         Name = ag.GenreType.Name,
                         Subgenres = ag.GenreType.Subgenres.Select(sg => new SubgenreResponseDto
                         {
                             Id = sg.Id,
                             Name = sg.Name
                         }).ToList(),
                     }).ToList(),
                     Theme = a.AlbumTheme,
                    ImageUrl = a.ImageUrl,
                    Label = a.Label,
                    PopularityScore = a.PopularityScore,
                    PercentileScore = a.PercentileScore,
                    ReleaseYear = a.ReleaseYear,
                    TotalTracks = a.TotalTracks,
                    SpotifyId = a.SpotifyId,
                    YoutubeId = a.YoutubeId,
                    AppleMusicId = a.AppleMusicId,
                    AmazonMusicId = a.AmazonMusicId,
                    PandoraId = a.PandoraId,
                    Artists = a.AlbumArtists.Select(aa => new ArtistResponseDto
                    {
                        Id = aa.Artist.Id,
                        Name = aa.Artist.Name,
                        Biography = aa.Artist.Biography,
                        Genres = aa.Artist.Genres,
                        ImageUrl = aa.Artist.ImageUrl,
                        PopularityScore = aa.Artist.PopularityScore,
                        PercentileScore = aa.Artist.PercentileScore,
                    }).ToList(),
                    Moods = a.AlbumMoods.Select(am => new MoodResponseDto
                    {
                        Id = am.Mood.Id,
                        Name = am.Mood.Name,
                    }).ToList(),
                })
                .OrderBy(a => Guid.NewGuid()).FirstOrDefaultAsync();

            return Ok(album);
        }
    }
}
