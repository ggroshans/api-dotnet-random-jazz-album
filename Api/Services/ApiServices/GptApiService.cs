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
            List<string> albumsForGpt = new List<string>();
            foreach (var album in spotifyAlbums)
            {
                var albumAndYearDetails = $"Title: {album.Title}, Release Year: {album.ReleaseYear}";

                albumsForGpt.Add(albumAndYearDetails);
            }


            var genres = _dbContext.GenreTypes.Select(gt => gt.Name).ToList();
            var genresFormattedForGpt = string.Join(", ", genres);
            //var jazzEras = _dbContext.JazzEraTypes.ToList();
            //string jazzErasFormattedForGpt = JsonConvert.SerializeObject(jazzEras);

            //string schemaExplanation = $@"
            //    Each album object must have the following fields:
            //    - title (string; title of album)
            //    - description (string, ~200 characters description of album)
            //    - genres (array of objects, each obj has a genre name (and a corresponding genre id based on genre types) that has to come from these types: {genresFormattedForGpt} and within each obj create a list of valid subgenres that fall under the genre umbrella based on the album. An album isnt limited to only one genre obj.)
            //    - moods (array of three objects; valence and arousal values should reflect the name of the mood itself so the mood can be reused for other albums.)
            //    - album_theme (string; ~100 character theme of album)
            //    - jazz_era (object; pick the correct id and corresponding name using the jazz era information here: {jazzErasFormattedForGpt}. If is_original_release is false, don't use release year to determine era.)
            //    - is_original_release (boolean; using the title and release year decide true if the album is the original studio release year and false if it's a re-release / reissued album, not a studio album or compilation (group studio albums does not count as a compilation.)";

            //string schema = @"
            //{
            //    ""title"": { ""type"": ""string""},
            //    ""description"": { ""type"": ""string"" },
            //    ""genres"": {
            //        ""type"": ""array"",
            //        ""items"": {
            //            ""type"": ""object"",
            //            ""properties"": {
            //                ""id"": { ""type"": ""number"" },
            //                ""name"": { ""type"": ""string"" },
            //                ""subgenres"": {
            //                    ""type"": ""array"",
            //                    ""items"": { ""type"": ""string"" }
            //                }
            //            }
            //        }
            //    },
            //    ""moods"": {
            //        ""type"": ""array"",
            //        ""items"": {
            //            ""type"": ""object"",
            //            ""properties"": {
            //                ""name"": { ""type"": ""string"" },
            //                ""valence"": { ""type"": ""number"", ""minimum"": -1, ""maximum"": 1 },
            //                ""arousal"": { ""type"": ""number"", ""minimum"": -1, ""maximum"": 1 }
            //            },
            //            ""required"": [""name"", ""valence"", ""arousal""]
            //        }
            //    },
            //    ""album_theme"": { ""type"": ""string"" },
            //    ""jazz_era"": {
            //        ""type"": ""object"",
            //        ""properties"": {
            //            ""id"": { ""type"": ""number"" },
            //            ""name"": { ""type"": ""string"" }
            //        }
            //    },
            //    ""is_original_release"": { ""type"": ""boolean"" },
            //}";

            //string schemaExplanation = $@"
            //    Each album object must have the following fields:
            //    - title (string; title of album)
            //    - description (string; ~200 characters description of album)
            //    - genres (array of objects; each obj has a genre name (and a corresponding genre id based on genre types) that has to come from these types: {genresFormattedForGpt} and within each obj create a list of valid subgenres that fall under the genre umbrella based on the album. An album isnt limited to only one genre obj.)
            //    - moods (array of three objects; valence and arousal values should reflect the name of the mood itself so the mood can be reused for other albums.)
            //    - album_theme (string; ~100 character theme of album)
            //    - is_original_release (boolean; using the title and release year decide true if the album is the original studio release year and false if it's a re-release / reissued album, not a studio album or compilation (group studio albums does not count as a compilation.)";

            //string schema = @"
            //{
            //    ""title"": { ""type"": ""string""},
            //    ""description"": { ""type"": ""string"" },
            //    ""genres"": {
            //        ""type"": ""array"",
            //        ""items"": {
            //            ""type"": ""object"",
            //            ""properties"": {
            //                ""id"": { ""type"": ""number"" },
            //                ""name"": { ""type"": ""string"" },
            //                ""subgenres"": {
            //                    ""type"": ""array"",
            //                    ""items"": { ""type"": ""string"" }
            //                }
            //            }
            //        }
            //    },
            //    ""moods"": {
            //        ""type"": ""array"",
            //        ""items"": {
            //            ""type"": ""object"",
            //            ""properties"": {
            //                ""name"": { ""type"": ""string"" },
            //                ""valence"": { ""type"": ""number"", ""minimum"": -1, ""maximum"": 1 },
            //                ""arousal"": { ""type"": ""number"", ""minimum"": -1, ""maximum"": 1 }
            //            },
            //            ""required"": [""name"", ""valence"", ""arousal""]
            //        }
            //    },
            //    ""album_theme"": { ""type"": ""string"" },
            //    ""is_original_release"": { ""type"": ""boolean"" },
            //}";

            var prompt = new StringBuilder();

            prompt.AppendLine("For each album below, return a compact JSON array of objects in this format (avoid ```json):");
            prompt.AppendLine("- title: string");
            prompt.AppendLine("- description: ~200 characters");
            prompt.AppendLine("- genres: array of objects { id, name, subgenres[] } using only genres from our DB. Add multiple subgenres (string[]) based on genre.");
            prompt.AppendLine("- moods: array of 3 objects { name, valence [-1–1] of mood, arousal [-1–1] of mood }.");
            prompt.AppendLine("- album_theme: ~100 characters");
            prompt.AppendLine("- is_original_release: true/false — if not original studio release, mark false.");

            prompt.AppendLine("Use only genres from: " + string.Join(", ", genres) + "and each album has unqiue genre/subgenres.");
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

            // ** Add to artist eventually

            //""related_artists"": {
            //    ""type"": ""array"",
            //        ""items"": { ""type"": ""string""},
            //    },
            //    ""influences"": {
            //    ""type"": ""array"",
            //        ""items"": { ""type"": ""string""},
            //    },

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
