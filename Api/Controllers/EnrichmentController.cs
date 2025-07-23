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
        public EnrichmentController(MusicDbContext db)
        {
            _db = db;
        }

        [HttpPost("batchProcess")]
        public void Enrichrichment()
        {
           try
            {
                //EnrichOriginalAlbumOrder();
                NormalizeAlbumPopularityScores();
                NormalizeArtistPopularityScores();
                CalculateAlbumMoodScores();
                CalculateOriginalAlbumOrder();
            } 
            catch (Exception ex)
            {
                Console.WriteLine($"Error during enrichment: {ex.Message}");
            }
        }

        private void NormalizeAlbumPopularityScores()
        {
            var albums = _db.Albums.ToList();

            if (albums.Count == 0)
            {
                throw new Exception("Album count is 0");
            }

            foreach (var album in albums)
            {
                var lower = albums.Count(a => a.SpotifyPopularity < album.SpotifyPopularity);
                var percentile = (int)Math.Round((double)lower / albums.Count * 100, 0);

                album.PopularityScore = percentile + 1;
            }
            _db.SaveChanges();
        }

        private void NormalizeArtistPopularityScores()
        {
            var artists = _db.Artists.ToList();

            if (artists.Count == 0)
            {
                throw new Exception("Artist count is 0");
            }

            foreach (var artist in artists)
            {
                var lower = artists.Count(a => a.SpotifyPopularity < artist.SpotifyPopularity);
                var percentile = (int)Math.Round((double)lower / artists.Count * 100, 0);

                artist.PopularityScore = percentile + 1;
            }
            _db.SaveChanges();
        }

        private void CalculateAlbumMoodScores()
        {
            var albums = _db.Albums.Include(a => a.AlbumMoods).ThenInclude(am => am.Mood).ToList();
            foreach (var album in albums)
            {
                var moods = album.AlbumMoods.Select(aa => aa.Mood).ToList();
                if (album.AverageEmotionalTone == null)
                {
                    var averageEmotionalTone = (int)moods.Select(m => m.EmotionalTone).Sum() / moods.Select(m => m.EmotionalTone).Count();
                    album.AverageEmotionalTone = averageEmotionalTone;
                }

                if (album.AverageEnergyLevel == null)
                {
                    var averageEnergyLevel = (int)moods.Select(m => m.EnergyLevel).Sum() / moods.Select(m => m.EnergyLevel).Count();
                    album.AverageEnergyLevel = averageEnergyLevel;
                }
            }
            _db.SaveChanges();
        }

        private void CalculateOriginalAlbumOrder()
        {
            var artists = _db.Artists.Include(a => a.AlbumArtists).ThenInclude(aa => aa.Album).ToList().Select(a => new ArtistProcessingDto
            {
                Name = a.Name,
                AlbumArtists = a.AlbumArtists.Where(aa => aa.OriginalAlbumOrder == null).ToList(), 
            });

            foreach (var artist in artists)
            {
                var originalAlbums = artist.AlbumArtists.Select(aa => aa.Album).Where(a => a.IsOriginalRelease == true).OrderBy(a => a.SortableDate).ToList();

                for (var i = 0; i < originalAlbums.Count; i++) 
                {
                    var artistAlbum = originalAlbums[i].AlbumArtists.Where(aa => aa.Album.Id == originalAlbums[i].Id).FirstOrDefault();
                    artistAlbum.OriginalAlbumOrder = i + 1;  
                } 
            } 
            _db.SaveChanges();
        }
    }
}
