using System;
using System.Linq;
using System.Threading.Tasks;
using Api.Data;
using Api.Models.DTOs.InternalDTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [Route("api/enrichment")]
    [ApiController]
    public class EnrichmentController : ControllerBase
    {
        private readonly MusicDbContext _db;
        public EnrichmentController(MusicDbContext db) { _db = db; }

        [HttpPost("batch-process")]
        public async Task<IActionResult> BatchProcess()
        {
            try
            {
                await NormalizeAlbumPopularityRatingsAsync();
                await NormalizeArtistPopularityRatingsAsync();
                await CalculateAlbumMoodScoresAsync();
                await CalculateOriginalAlbumOrderAsync();

                return Ok(new
                {
                    message = "Enrichment batch completed.",
                    steps = new[]
                    {
                        "NormalizeAlbumPopularityRatings",
                        "NormalizeArtistPopularityRatings",
                        "CalculateAlbumMoodScores",
                        "CalculateOriginalAlbumOrder"
                    },
                    completedUtc = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during enrichment: {ex.Message}");
                return StatusCode(500, new { error = "Enrichment failed.", message = ex.Message });
            }
        }

        private async Task NormalizeAlbumPopularityRatingsAsync()
        {
            var albums = await _db.Albums.ToListAsync();
            if (albums.Count == 0) throw new Exception("Album count is 0");

            foreach (var album in albums)
            {
                var lower = albums.Count(a => a.SpotifyPopularity < album.SpotifyPopularity);
                var percentile = (int)Math.Round((double)lower / albums.Count * 100, 0);
                album.PopularityRating = percentile + 1;
            }
            await _db.SaveChangesAsync();
        }

        private async Task NormalizeArtistPopularityRatingsAsync()
        {
            var artists = await _db.Artists.ToListAsync();
            if (artists.Count == 0) throw new Exception("Artist count is 0");

            foreach (var artist in artists)
            {
                var lower = artists.Count(a => a.SpotifyPopularity < artist.SpotifyPopularity);
                var percentile = (int)Math.Round((double)lower / artists.Count * 100, 0);
                artist.PopularityRating = percentile + 1;
            }
            await _db.SaveChangesAsync();
        }

        private async Task CalculateAlbumMoodScoresAsync()
        {
            var albums = await _db.Albums
                .Include(a => a.AlbumMoods)
                .ThenInclude(am => am.Mood)
                .ToListAsync();

            foreach (var album in albums)
            {
                var moods = album.AlbumMoods.Select(aa => aa.Mood).ToList();
                if (moods.Count == 0) continue;

                if (album.AverageEmotionalTone == null)
                {
                    var avgTone = (int)(moods.Select(m => m.EmotionalTone).Sum() / (double)moods.Count);
                    album.AverageEmotionalTone = avgTone;
                }

                if (album.AverageEnergyLevel == null)
                {
                    var avgEnergy = (int)(moods.Select(m => m.EnergyLevel).Sum() / (double)moods.Count);
                    album.AverageEnergyLevel = avgEnergy;
                }
            }
            await _db.SaveChangesAsync();
        }

        private async Task CalculateOriginalAlbumOrderAsync()
        {
            var artists = await _db.Artists
                .Include(a => a.AlbumArtists)
                .ThenInclude(aa => aa.Album)
                .ToListAsync();

            var processing = artists.Select(a => new ArtistProcessingDto
            {
                Name = a.Name,
                AlbumArtists = a.AlbumArtists
                    .Where(aa => aa.OriginalAlbumOrder == null)
                    .ToList()
            });

            foreach (var artist in processing)
            {
                var originals = artist.AlbumArtists
                    .Select(aa => aa.Album)
                    .Where(a => a.IsOriginalRelease == true)
                    .OrderBy(a => a.SortableDate)
                    .ToList();

                for (var i = 0; i < originals.Count; i++)
                {
                    var album = originals[i];
                    var artistAlbum = album.AlbumArtists.FirstOrDefault(aa => aa.Album.Id == album.Id);
                    if (artistAlbum != null) artistAlbum.OriginalAlbumOrder = i + 1;
                }
            }
            await _db.SaveChangesAsync();
        }
    }
}
