using Newtonsoft.Json;
using OpenAI.Chat;
using System.Text;
using Api.Domain.Entities;
using Api.Data;
using Api.Models;
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
            List<string> albumNames = new List<string>();

            foreach (var album in spotifyAlbums)
            {
                albumNames.Add(album.Title);
            }

            string schemaExplanation = @"
                Each album object must have the following fields:
                - title (string)
                - description (string, ~200 characters)
                - genre (string, one of: Jazz, Blues, Funk, Rock, Bluegrass)
                - subgenres (array of strings, valid subgenres for the genre)
                - moods (array of strings)
                - popular_tracks (array of strings)
                - album_theme (string)
                ";

            string schema = @"
            {
                ""title"": { ""type"": ""string""},
                ""description"": { ""type"": ""string"" },
                ""genre"": { ""type"": ""string"" },
                ""subgenres"": { ""type"": ""array"", ""items"": { ""type"": ""string"" } },
                ""moods"": { ""type"": ""array"", ""items"": { ""type"": ""string"" } },
                ""popular_tracks"": { ""type"": ""array"", ""items"": { ""type"": ""string"" } },
                ""album_theme"": { ""type"": ""string"" }
            }";

            var prompt = new StringBuilder();
            prompt.AppendLine("Iterate through the album names and return a JSON array where each album matches this schema:");
            prompt.AppendLine(schemaExplanation);
            prompt.AppendLine(schema);
            prompt.AppendLine("Each album must have a genre from: [Jazz, Blues, Funk, Rock, Bluegrass] and valid subgenres.");
            prompt.AppendLine("Ensure all albums are included in the JSON output.");
            prompt.AppendLine($"Albums: {string.Join(", ", albumNames)}");

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
                                     Genre = matchingGptAlbum.Genre,
                                     Subgenres = matchingGptAlbum.Subgenres,
                                     Moods = matchingGptAlbum.Moods,
                                     PopularTracks = matchingGptAlbum.PopularTracks,
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
            string schema = @"
        {
            ""biography"": { ""type"": ""string"" }, // artist/musician description or release notes (~500 characters)
            ""instrument"": { ""type"": ""string"" }, // the main instrument this artist/musician plays. If it's a band/group then return group
        }";
                var prompt = new StringBuilder();
                prompt.AppendLine($"" +
                    $"For the artist/musician: {artist.Name}, return a json object using this schema,  " +
                    $"formatted in a single line without line breaks or extra spaces," +
                    $"and without any code block formatting or additional text:");
                prompt.AppendLine(schema);

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
