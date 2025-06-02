using Newtonsoft.Json;
using OpenAI.Chat;
using System.Text;
using Api.Domain.Entities;
using Api.Data;
using Api.Utilities;
using Api.Models.DTOs.InternalDTOs;

namespace Api.Services.ApiServices
{
    public class GptApiService
    {
        private readonly string _apiKey;
        private readonly ChatClient _client;
        private readonly MusicDbContext _dbContext;

        public GptApiService(IConfiguration configuration, MusicDbContext dbContext)
        {
            _apiKey = configuration["openai"];
            _client = new(model: "gpt-4o", apiKey: _apiKey);
            _dbContext = dbContext;
        }

        public async Task<(DiscoTransaction, List<AlbumProcessingDto>, bool)> GetGptAlbumDetails(List<AlbumProcessingDto> spotifyAlbums, string artistName, DiscoTransaction discoTransaction)
        {
            List<string> albumsForGpt = new List<string>();

            foreach (var album in spotifyAlbums)
            {
                var albumAndYearDetails = $"Title: {album.Title}, Release Year: {album.ReleaseYear}";

                albumsForGpt.Add(albumAndYearDetails);
            }

            var genres = _dbContext.GenreTypes.Select(gt => gt.Name).ToList();
            var genresFormattedForGpt = string.Join(", ", genres);
            var prompt = new StringBuilder();

            prompt.AppendLine("For each album below, return a compact JSON array of objects in this format (avoid ```json):");
            prompt.AppendLine("- title: string");
            prompt.AppendLine("- description: ~200 characters");
            prompt.AppendLine("- genres: array of objects { id, name, subgenres[] } using only genres from our DB. Add multiple subgenres (string[]) based on genre.");
            prompt.AppendLine("- moods: array of 3 objects { name, valence [-1–1] of mood, arousal [-1–1] of mood }.");
            prompt.AppendLine("- album_theme: ~100 characters");
            prompt.AppendLine("- is_original_release: true/false — if not original studio release, mark false.");

            prompt.AppendLine("Use only genres from: " + string.Join(", ", genres) + ". Subgenres must be more specific than their parent genre and cannot duplicate any genre in the provided list (e.g., 'Blues' or 'Jazz' cannot be subgenres).");
            prompt.AppendLine("Output a compact JSON array — no line breaks, code blocks, or extra text.");
            prompt.AppendLine("Albums to process: " + string.Join(", ", albumsForGpt));

            discoTransaction.ResponseStatusCode = 200;
            discoTransaction.ErrorMessage = null;
            
            try
            {
                var chatCompletion = await _client.CompleteChatAsync(prompt.ToString());
                var responseContent = chatCompletion?.Value.Content[0].Text;
                var processedAlbums = new List<AlbumProcessingDto>();

                if (!string.IsNullOrEmpty(responseContent))
                {
                    try
                    {
                        var gptAlbums = JsonConvert.DeserializeObject<List<AlbumProcessingDto>>(responseContent);
                        
                        foreach (var spotifyAlbum in spotifyAlbums)
                        {
                            var matchingGptAlbum = gptAlbums.Where(g => g.Title == spotifyAlbum.Title).FirstOrDefault();
                            
                            if (matchingGptAlbum != null)
                            {
                                var updatedAlbum = new AlbumProcessingDto
                                {
                                     Title = spotifyAlbum.Title,
                                     Artists = spotifyAlbum.Artists,
                                     ReleaseYear = spotifyAlbum.ReleaseYear,
                                     TotalTracks = spotifyAlbum.TotalTracks,
                                     ImageUrl = spotifyAlbum.ImageUrl,
                                     SpotifyId = spotifyAlbum.SpotifyId,
                                     Label = spotifyAlbum.Label,
                                     PopularityScore = spotifyAlbum.PopularityScore,
                                     Description = matchingGptAlbum.Description,
                                     Genres = matchingGptAlbum.Genres,
                                     Moods = matchingGptAlbum.Moods,
                                     JazzEra = matchingGptAlbum.JazzEra,
                                     IsOriginalRelease = matchingGptAlbum.IsOriginalRelease,
                                     AlbumTheme = matchingGptAlbum.AlbumTheme,
                                };
                                processedAlbums.Add(updatedAlbum);
                            }
                        }
                    }
                    catch (JsonSerializationException ex)
                    {
                        Console.WriteLine($"JSON Serialization Exception: {ex.Message}");
                        Console.WriteLine($"GPT Response: {responseContent}");
                        discoTransaction.ErrorMessage = ex.Message;
                        discoTransaction.ResponseStatusCode = 500; 

                        return (discoTransaction, new List<AlbumProcessingDto>(), true); 
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Unexpected Exception: {ex.Message}");
                        discoTransaction.ErrorMessage = ex.Message;
                        discoTransaction.ResponseStatusCode = 500; 

                        return (discoTransaction, new List<AlbumProcessingDto>(), true); 
                    }
                }
                else
                {
                    Console.WriteLine("GPT response data null");
                    discoTransaction.ErrorMessage = "GPT response data was null";
                    discoTransaction.ResponseStatusCode = 500; 

                    return (discoTransaction, new List<AlbumProcessingDto>(), true); 
                }
                return (discoTransaction, processedAlbums, false);
            }
            
            catch (Exception ex)
            {
                Console.WriteLine($"GPT request failed: {ex.Message}");
                discoTransaction.ErrorMessage = ex.Message;
                discoTransaction.ResponseStatusCode = 500; 

                return (discoTransaction, new List<AlbumProcessingDto>(), true); 
            }
        }

        public async Task<ArtistProcessingDto> GetGptArtistDetails(ArtistProcessingDto artist)
        {
            var prompt = new StringBuilder();
            prompt.AppendLine($"For the artist {artist.Name}, return a compact JSON object with these fields (avoid ```json):");
            prompt.AppendLine("- biography: ~500 characters");
            prompt.AppendLine("- instrument: string (main instrument, or 'group' if band)");
            prompt.AppendLine("- related_artists: array of strings (related artists/musicians)");
            prompt.AppendLine("- influences: array of strings (artists who influenced this artist)");
            prompt.AppendLine("Output a single-line JSON object without line breaks, code blocks, or extra text.");

            var chatCompletion = await _client.CompleteChatAsync(prompt.ToString());
            var responseContent = chatCompletion?.Value.Content[0].Text;

            if (!string.IsNullOrEmpty(responseContent))
                {
                    try
                    {
                        var artistDetails = JsonConvert.DeserializeObject<ArtistProcessingDto>(responseContent);

                        if (artistDetails != null)
                        {
                            artist.Biography = artistDetails.Biography;
                            artist.Instrument = artistDetails.Instrument;
                            artist.RelatedArtists = artistDetails.RelatedArtists;
                            artist.Influences = artistDetails.Influences;
                        }
                    }

                    catch (Exception ex)
                    {
                        Console.WriteLine($"JSON Parsing Error for artist {artist.Name}: {ex.Message}");
                    }
                }
            return artist;
        }

        public async Task<(DiscoTransaction, List<AlbumProcessingDto>, bool)> BatchProcessAlbums(List<AlbumProcessingDto> albums, string artistName)
        {
            var discoTransaction = new DiscoTransaction
            {
                Id = new Guid(),
                TimeStamp = DateTime.UtcNow
            };
            var requestDetails = new RequestDetails
            {
                PrimaryArtistName = StringUtils.CapitalizeAndFormat(artistName),
            };

            discoTransaction.RequestDetails = JsonConvert.SerializeObject(requestDetails);

            List<AlbumProcessingDto> unprocessedAlbums = albums;
            List<AlbumProcessingDto> gptAlbums = new List<AlbumProcessingDto>();
            List<AlbumProcessingDto> processedAlbums = new List<AlbumProcessingDto>();
            var albumCount = albums.Count;
            bool batchError = false;
           
            while (unprocessedAlbums.Count > 0)
            {
                var batchSize = Math.Min(20, unprocessedAlbums.Count);
                var albumsBatch = unprocessedAlbums.Take(batchSize).ToList();
                (discoTransaction, gptAlbums, batchError) = await GetGptAlbumDetails(albumsBatch, artistName, discoTransaction);

                if (batchError == true)
                {
                    break;
                }

                processedAlbums.AddRange(gptAlbums);
                unprocessedAlbums = unprocessedAlbums.Skip(batchSize).ToList();
            }

            await _dbContext.DiscoTransactions.AddAsync(discoTransaction);
            await _dbContext.SaveChangesAsync();

            return (discoTransaction, processedAlbums, batchError);
        }
    }
}
