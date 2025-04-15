using Api.Data;
using Api.Services.ApiServices.Spotify;
using Api.Services;
using Microsoft.AspNetCore.Mvc;
using Api.Services.ApiServices;
using System.Net;

namespace Api.Controllers
{
    [Route("api/admin")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private MusicDbContext _db {get; set;}
        private SpotifyApiService _spotifyApiService {get; set;}
        private GptApiService _openAiService { get; set; }
        private PopulateDbService _populateDbService { get; set; }

        public AdminController(MusicDbContext db, SpotifyApiService spotifyClient, GptApiService openAiService, PopulateDbService populateDbService)
        {
            _db = db;
            _spotifyApiService = spotifyClient;
            _openAiService = openAiService;
            _populateDbService = populateDbService;
        }

        [HttpPost("create-discography")]
        public async Task<IActionResult> CreateDiscographyFromArtist([FromBody] string artistName)
        {
            var spotifyAlbums = await _spotifyApiService.GetSpotifyAlbums(artistName);
            if (!spotifyAlbums.Any())
            {
                return NotFound($"Could not find any albums by {artistName} on Spotify.");
            }

            var (discoTransaction, processedAlbums, batchError) = await _openAiService.BatchProcessAlbums(spotifyAlbums, artistName);
            if (batchError)
            {

                return BadRequest($"Batch Error: {discoTransaction.ErrorMessage}");
            }

            await _populateDbService.PopulateAlbumAsync(discoTransaction, processedAlbums);
            return Ok(processedAlbums);
        }

        [HttpPost ("normalize-album-scores")]
        public void NormalizeAlbumPopularityScores()
        {
            var albums = _db.Albums.ToList();

            if (albums.Count == 0)
            {
                throw new Exception("Album count is 0");
            }

            foreach (var album in albums)
            {
                var lower = albums.Count(a => a.PopularityScore < album.PopularityScore);
                var percentile = Math.Round((double)lower / albums.Count * 100, 0);

                album.PercentileScore = percentile;
            }
            _db.SaveChanges();
        }

        [HttpPost("normalize-artist-scores")]
        public void NormalizeArtistPopularityScores()
        {
            var artists = _db.Artists.ToList();

            if (artists.Count == 0)
            {
                throw new Exception("Artist count is 0");
            }

            foreach (var artist in artists)
            {
                var lower = artists.Count(a => a.PopularityScore < artist.PopularityScore);
                var percentile = Math.Round((double)lower / artists.Count * 100, 0);

                artist.PercentileScore = percentile;
            }
            _db.SaveChanges();
        }

        [HttpPost("test")]
        public async Task<IActionResult> GetMusicLinks(string spotifyAlbumId)
        {
            var url = $"https://open.spotify.com/album/{spotifyAlbumId}";

            var encodedUrl = WebUtility.UrlEncode(url);

            var songLinkUrl = $"https://api.song.link/v1-alpha.1/links?url={encodedUrl}";

            var httpClient = new HttpClient();

            var songLinkResponse = await httpClient.GetAsync(songLinkUrl);
            var responseBody = await songLinkResponse.Content.ReadAsStringAsync();

            if (!songLinkResponse.IsSuccessStatusCode)
            {
                return NotFound();
            }

            return Ok(responseBody);
        }
    }
}
