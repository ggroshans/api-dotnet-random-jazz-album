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
                     Genres = a.AlbumGenres.Select(ag => ag.GenreType.Name).ToList(),
                     Subgenres = a.AlbumSubgenres.Select(asg => asg.Subgenre.Name).ToList(),
                     ImageUrl = a.ImageUrl,
                     Label = a.Label,
                     TotalTracks = a.TotalTracks,
                     IsOriginalRelease = a.IsOriginalRelease,
                     SortableDate = a.SortableDate,
                     PopularityRating = a.PopularityRating,
                     AverageEmotionalTone = a.AverageEmotionalTone,
                     AverageEnergyLevel = a.AverageEnergyLevel,
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
                         PopularityRating = aa.Artist.PopularityRating,
                     }).ToList(),
                     Moods = a.AlbumMoods.Select(am => new MoodResponseDto
                     {
                         Id = am.Mood.Id,
                         Name = am.Mood.Name,
                     }).ToList(),
                     OriginalAlbumOrder = a.AlbumArtists.FirstOrDefault().OriginalAlbumOrder,
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
                .AsNoTracking()
                .OrderBy(a => EF.Functions.Random())
                .Select(a => new AlbumResponseDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    Description = a.Description,
                    Genres = a.AlbumGenres
                        .Select(ag => ag.GenreType.Name)
                        .ToList(),
                    Subgenres = a.AlbumSubgenres
                        .Select(asg => asg.Subgenre.Name)
                        .ToList(),
                    ImageUrl = a.ImageUrl,
                    Label = a.Label,
                    TotalTracks = a.TotalTracks,
                    IsOriginalRelease = a.IsOriginalRelease,
                    SortableDate = a.SortableDate,
                    PopularityRating = a.PopularityRating,
                    AverageEmotionalTone = a.AverageEmotionalTone,
                    AverageEnergyLevel = a.AverageEnergyLevel,
                    SpotifyId = a.SpotifyId,
                    YoutubeId = a.YoutubeId,
                    AppleMusicId = a.AppleMusicId,
                    AmazonMusicId = a.AmazonMusicId,
                    PandoraId = a.PandoraId,
                    Artists = a.AlbumArtists
                        .Select(aa => new ArtistResponseDto
                        {
                            Id = aa.Artist.Id,
                            Name = aa.Artist.Name,
                            Biography = aa.Artist.Biography,
                            Genres = aa.Artist.Genres,
                            ImageUrl = aa.Artist.ImageUrl,
                            PopularityRating = aa.Artist.PopularityRating
                        })
                        .ToList(),
                    Moods = a.AlbumMoods
                        .Select(am => new MoodResponseDto
                        {
                            Id = am.Mood.Id,
                            Name = am.Mood.Name
                        })
                        .ToList(),
                    JazzEras = a.AlbumJazzEras
                        .Select(aje => aje.JazzEraType.Name)
                        .ToList(),
                    AdditionalArtists = a.AdditionalArtists,
                    OriginalAlbumOrder = a.AlbumArtists
                        .OrderBy(aa => aa.OriginalAlbumOrder)
                        .Select(aa => aa.OriginalAlbumOrder)
                        .FirstOrDefault()
                })
                .FirstOrDefaultAsync();

            if (album is null) return NotFound();
            return Ok(album);
        }
    }
}
