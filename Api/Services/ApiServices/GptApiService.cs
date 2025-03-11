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
                albumNames.Add(album.Name);
            }

            string schema = @"
    {
        ""name"": { ""type"": ""string""},
        ""description"": { ""type"": ""string"" }, // album description or release notes (~500 characters)
        ""genre"": { ""type"": ""string"" }, // primary music genre (Only option is jazz)
        ""subgenres"": { ""type"": ""array"", ""items"": { ""type"": ""string"" } }, // list of subgenres
        ""moods"": { ""type"": ""array"", ""items"": { ""type"": ""string"" } }, // list of moods that describe the album
        ""popular_tracks"": { ""type"": ""array"", ""items"": { ""type"": ""string"" } }, // list of the most popular tracks from the album
        ""album_theme"": { ""type"": ""string"" } // main theme or concept of the album
    }";

            var prompt = new StringBuilder();
            prompt.AppendLine("Iterate through this string of album names, return a JSON array where each object (album) adheres to this schema:");
            prompt.AppendLine(schema);
            prompt.AppendLine("Formatted in a single line without line breaks or extra spaces, and without any code block formatting or additional text.");
            prompt.AppendLine("Only update each album object from this Genres and these Subgenres");
            prompt.AppendLine("Genre: Jazz, Subgenres: Swing, Bebop, Hard Bop, Cool Jazz, Modal Jazz, Free Jazz, Fusion, Latin Jazz, Soul Jazz, Smooth Jazz, Gypsy Jazz, Acid Jazz, Post-Bop, Avant-Garde Jazz, Dixieland, Jazz-Funk, Jazz Rap, Big Band, Vocal Jazz, Jazz Blues, Neo-Bop, Third Stream, Ethio-Jazz, M-Base, Nu Jazz, Spiritual Jazz");
            prompt.AppendLine("Ensure that all albums are processed and included in the JSON response.");
            //prompt.AppendLine("Genre: Blues, Subgenres: Delta Blues, Chicago Blues, Texas Blues, Piedmont Blues, Memphis Blues, Country Blues, Urban Blues, Jump Blues, Electric Blues, Swamp Blues, Blues Rock, Boogie-Woogie, Soul Blues, Gospel Blues, Acoustic Blues, British Blues, West Coast Blues, Rhythm and Blues (R&B), Funk Blues, Hill Country Blues, Jazz Blues, Punk Blues, New Orleans Blues, Louisiana Blues, Contemporary Blues, Ragtime Blues");
            //prompt.AppendLine("Genre: Funk, Subgenres: P-Funk, Funk Rock, Funk Metal, Funk Soul, G-Funk, Jazz-Funk, Electro-Funk, Afrobeat, Go-Go, Psychedelic Funk, Disco Funk, Minneapolis Sound, Boogie, Funk Pop, Latin Funk, New Orleans Funk, Experimental Funk, Neo-Funk, Rare Groove, Future Funk");
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
                            var matchingGptAlbum = gptAlbums.Where(g => g.Name == spotifyAlbum.Name).FirstOrDefault();
                            
                            if (matchingGptAlbum != null)
                            {
                                var updatedAlbum = new AlbumProcessingDto
                                {
                                     Name = spotifyAlbum.Name,
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
