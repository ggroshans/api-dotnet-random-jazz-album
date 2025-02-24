using Newtonsoft.Json;
using OpenAI.Chat;
using Api.Models;
using System.Text;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Api.Services.ApiServices
{
    public class GptApiService
    {
        private readonly string _apiKey;
        private readonly ChatClient _client;

        public GptApiService(IConfiguration configuration)
        {
            _apiKey = configuration["openai"];
            _client = new(model: "gpt-4o", apiKey: _apiKey);
        }

        //public async Task<ChatCompletion> GetAlbumDetailAsync(string artistName, string albumName)
        //{
        //    string schema = @"{
        //      ""title"": { ""type"": ""string"" }, // album title
        //      ""artist_name"": { ""type"": ""string"" }, // musician's name
        //      ""release_year"": { ""type"": ""int"" }, // album release year 
        //      ""genre"": { ""type"": ""string"" }, // primary music genre (e.g., rock, jazz)
        //      ""sub_genres"": { ""type"": ""array"", ""items"": { ""type"": ""string"" } }, // list of subgenres
        //      ""cover_image_url"": { ""type"": ""string"", ""format"": ""uri"" }, // URL of the album cover image
        //      ""total_tracks"": { ""type"": ""integer"" }, // total number of tracks in the album
        //      ""label"": { ""type"": ""string"" }, // record label
        //      ""mood"": { ""type"": ""array"", ""items"": { ""type"": ""string"" } }, // list of moods that describe the album
        //      ""recording_location"": { ""type"": ""string"" }, // where the album was recorded: city, state
        //      ""length"": { ""type"": ""integer"" }, // total album length in minutes
        //      ""release_notes"": { ""type"": ""string"" }, // artist's notes or comments on the release
        //      ""album_theme"": { ""type"": ""string"" }, // main theme or concept of the album
        //      ""story_behind_title"": { ""type"": ""string"" }, // explanation of the album's title and significance
        //      ""album_position"": { ""type"": ""integer"" }, // position of the album in the artist's discography (e.g., 1st, 5th)
        //      ""popular_tracks"": { ""type"": ""array"", ""items"": { ""type"": ""string"" } }, // list of the most popular tracks from the album
        //      ""awards"": { ""type"": ""array"", ""items"": { ""type"": ""string"" } }, // list of awards in 'award (year)' format
        //      ""cover_art_description"": { ""type"": ""string"" }, // brief description of the album cover design and concept
        //      ""personal_anecdotes"": { ""type"": ""string"" }, // artist or producer anecdotes about the making of the album
        //      ""setlist_context"": { ""type"": ""string"" }, // details on how tracks fit into live performances or tours
        //      ""created_at"": { ""type"": ""string"", ""format"": ""date-time"" }, // timestamp when the record was created
        //      ""updated_at"": { ""type"": ""string"", ""format"": ""date-time"" } // timestamp when the record was last updated
        //    }";

        //    var prompt = new StringBuilder();
        //    prompt.AppendLine($"For the album: {albumName} by {artistName}, return a json object using this schema:");
        //    prompt.AppendLine(schema);

        //    var chatCompletion = await _client.CompleteChatAsync(prompt.ToString());

        //    return chatCompletion;
        //} 

        public async Task<List<AlbumDto>> GetGptAlbumDetails(List<AlbumDto> albums, string artistName)
        {
            List<string> albumNames = new List<string>();

            foreach (var album in albums)
            {
                albumNames.Add(album.Name);
            }

            string schema = @"
            {
            ""name"": { ""type"": ""string""},
            ""description"": { ""type"": ""string"" }, // album description or release notes (~500 characters)
            ""genre"": { ""type"": ""string"" }, // primary music genre (Only options are jazz, blues, funk)
            ""subgenres"": { ""type"": ""array"", ""items"": { ""type"": ""string"" } }, // list of subgenres
            ""moods"": { ""type"": ""array"", ""items"": { ""type"": ""string"" } }, // list of moods that describe the album
            ""popular_tracks"": { ""type"": ""array"", ""items"": { ""type"": ""string"" } }, // list of the most popular tracks from the album
            ""album_theme"": { ""type"": ""string"" } // main theme or concept of the album
            }";

            var prompt = new StringBuilder();
            prompt.AppendLine("Iterate through this string of album names, return a JSON array where each object (album) adheres to this schema:");
            prompt.AppendLine(schema);
            prompt.AppendLine("Formatted in a single line without line breaks or extra spaces, and without any code block formatting or additional text.");
            prompt.AppendLine("Only update each album object from these Genres and Subgenres:");
            prompt.AppendLine("Genre: Jazz, Subgenres: Swing, Bebop, Hard Bop, Cool Jazz, Modal Jazz, Free Jazz, Fusion, Latin Jazz, Soul Jazz, Smooth Jazz, Gypsy Jazz, Acid Jazz, Post-Bop, Avant-Garde Jazz, Dixieland, Jazz-Funk, Jazz Rap, Big Band, Vocal Jazz, Jazz Blues, Neo-Bop, Third Stream, Ethio-Jazz, M-Base, Nu Jazz, Spiritual Jazz");
            prompt.AppendLine("Genre: Blues, Subgenres: Delta Blues, Chicago Blues, Texas Blues, Piedmont Blues, Memphis Blues, Country Blues, Urban Blues, Jump Blues, Electric Blues, Swamp Blues, Blues Rock, Boogie-Woogie, Soul Blues, Gospel Blues, Acoustic Blues, British Blues, West Coast Blues, Rhythm and Blues (R&B), Funk Blues, Hill Country Blues, Jazz Blues, Punk Blues, New Orleans Blues, Louisiana Blues, Contemporary Blues, Ragtime Blues");
            prompt.AppendLine("Genre: Funk, Subgenres: P-Funk, Funk Rock, Funk Metal, Funk Soul, G-Funk, Jazz-Funk, Electro-Funk, Afrobeat, Go-Go, Psychedelic Funk, Disco Funk, Minneapolis Sound, Boogie, Funk Pop, Latin Funk, New Orleans Funk, Experimental Funk, Neo-Funk, Rare Groove, Future Funk");
            prompt.AppendLine($"Albums: {string.Join(", ", albumNames)}");

            var chatCompletion = await _client.CompleteChatAsync(prompt.ToString());
            var responseContent = chatCompletion?.Value.Content[0].Text;
            var albumDetails = new List<AlbumDto>();

            if (!string.IsNullOrEmpty(responseContent))
            {
                try
                { 
                    albumDetails = JsonConvert.DeserializeObject<List<AlbumDto>>(responseContent);
                }
                catch (JsonSerializationException ex)
                {
                    Console.WriteLine($"JSON Serialization Exception: {ex.Message}");
                    Console.WriteLine($"GPT Response: {responseContent}"); 
                }
                catch (Exception ex) 
                {
                    Console.WriteLine($"Unexpected Exception: {ex.Message}");
                }
            }
            else 
            {
                Console.WriteLine("GPT response data null");
            }

            return albumDetails;
        }

        public async Task<ArtistDto> GetGptArtistDetails(ArtistDto artist)
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

                        var artistDetails = JsonConvert.DeserializeObject<ArtistDto>(responseContent);

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
    }
}
