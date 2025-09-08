using Microsoft.AspNetCore.Mvc;
using Api.Services.ApiServices;
using Api.Services;
using Api.Services.ApiServices.Spotify;
using Api.Data;
using Microsoft.EntityFrameworkCore;
using Api.Models.DTOs.ResponseDTOs;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Api.Services.Contracts;

namespace Api.Controllers
{
    [Route("api/albums")]
    [ApiController]
    public class AlbumController : ControllerBase
    {
        private readonly Serilog.ILogger _logger;
        private readonly ISpotifyApiService _spotifyApiService;
        private readonly IGptApiService _openAIservice;
        private readonly IPopulateDbService _populateDbService;
        private readonly MusicDbContext _dbContext;


        public AlbumController(Serilog.ILogger logger, IGptApiService openAIService, ISpotifyApiService spotifyApiService, IPopulateDbService populateDbService, MusicDbContext dbContext)
        {
            _logger = logger;
            _openAIservice = openAIService;
            _spotifyApiService = spotifyApiService;
            _populateDbService = populateDbService;
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetAlbums(
            [FromQuery] string? query, 
            [FromQuery] int pageNumber = 1, 
            [FromQuery] int pageSize = 12)
        {
            var queryable = _dbContext.Albums.AsQueryable();

            if (!string.IsNullOrWhiteSpace(query))
            {
                var lowerCaseQuery = query.Trim().ToLower();
                queryable = queryable.Where(a => 
                    a.Title.ToLower().Contains(lowerCaseQuery) || 
                    a.AlbumArtists.Any(aa => aa.Artist.Name.ToLower().Contains(lowerCaseQuery))
                );
            }

            var totalCount = await queryable.CountAsync();

            var albums = await queryable
                // *** THIS IS THE CHANGE FOR SORTING ***
                .OrderByDescending(a => a.SortableDate) // Sort by date instead of title
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
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
                         EmotionalTone = am.Mood.EmotionalTone,
                         EnergyLevel = am.Mood.EnergyLevel
                     }).ToList(),
                     OriginalAlbumOrder = a.AlbumArtists.FirstOrDefault().OriginalAlbumOrder,
                })
                .ToListAsync();

            var result = new PaginatedResponseDto<AlbumResponseDto>
            {
                Items = albums,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                HasPreviousPage = pageNumber > 1,
                HasNextPage = pageNumber < (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return Ok(result);
        }


        [HttpGet("{Id:int}")]
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
                         EmotionalTone = am.Mood.EmotionalTone,
                         EnergyLevel = am.Mood.EnergyLevel
                     }).ToList(),
                     JazzEras = a.AlbumJazzEras.Select(aje => new JazzEraResponseDto
                     {
                         Name = aje.JazzEraType.Name,
                         Description = aje.JazzEraType.Description,
                         StartYear = aje.JazzEraType.StartYear,
                         EndYear = aje.JazzEraType.EndYear,

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
                            Name = am.Mood.Name,
                            EmotionalTone = am.Mood.EmotionalTone,
                            EnergyLevel = am.Mood.EnergyLevel
                        })
                        .ToList(),
                    JazzEras = a.AlbumJazzEras.Select(aje => new JazzEraResponseDto
                    {
                        Name = aje.JazzEraType.Name,
                        Description = aje.JazzEraType.Description,
                        StartYear = aje.JazzEraType.StartYear,
                        EndYear = aje.JazzEraType.EndYear
                    }).ToList(),
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