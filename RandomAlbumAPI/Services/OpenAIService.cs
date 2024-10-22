using OpenAI.Chat;
using System.Text;

namespace RandomAlbumAPI.Services
{
    public class OpenAIService
    {
        private readonly string _apiKey;
        private readonly ChatClient _client;

        public OpenAIService (IConfiguration configuration)
        {
            _apiKey = configuration["openai"];
            _client = new(model: "gpt-4o", apiKey: _apiKey);
        }

        public async Task<ChatCompletion> GetAlbumDetailAsync(string artistName, string albumName)
        {
            string schema = @"{
              ""title"": { ""type"": ""string"" }, // album title
              ""artist_name"": { ""type"": ""string"" }, // musician's name
              ""release_date"": { ""type"": ""string"", ""format"": ""date"" }, // album release date (MM/DD/YYYY)
              ""genre"": { ""type"": ""string"" }, // primary music genre (e.g., rock, jazz)
              ""sub_genres"": { ""type"": ""array"", ""items"": { ""type"": ""string"" } }, // list of subgenres
              ""cover_image_url"": { ""type"": ""string"", ""format"": ""uri"" }, // URL of the album cover image
              ""total_tracks"": { ""type"": ""integer"" }, // total number of tracks in the album
              ""label"": { ""type"": ""string"" }, // record label
              ""mood"": { ""type"": ""array"", ""items"": { ""type"": ""string"" } }, // list of moods that describe the album
              ""recording_location"": { ""type"": ""string"" }, // where the album was recorded: city, state
              ""length"": { ""type"": ""integer"" }, // total album length in minutes
              ""release_notes"": { ""type"": ""string"" }, // artist's notes or comments on the release
              ""album_theme"": { ""type"": ""string"" }, // main theme or concept of the album
              ""story_behind_title"": { ""type"": ""string"" }, // explanation of the album's title and significance
              ""album_position"": { ""type"": ""integer"" }, // position of the album in the artist's discography (e.g., 1st, 5th)
              ""popular_tracks"": { ""type"": ""array"", ""items"": { ""type"": ""string"" } }, // list of the most popular tracks from the album
              ""awards"": { ""type"": ""array"", ""items"": { ""type"": ""string"" } }, // list of awards in 'award (year)' format
              ""cover_art_description"": { ""type"": ""string"" }, // brief description of the album cover design and concept
              ""personal_anecdotes"": { ""type"": ""string"" }, // artist or producer anecdotes about the making of the album
              ""setlist_context"": { ""type"": ""string"" }, // details on how tracks fit into live performances or tours
              ""created_at"": { ""type"": ""string"", ""format"": ""date-time"" }, // timestamp when the record was created
              ""updated_at"": { ""type"": ""string"", ""format"": ""date-time"" } // timestamp when the record was last updated
            }";

            var prompt = new StringBuilder();
            prompt.AppendLine($"For the album: {albumName} by {artistName}, return a json object using this schema:");
            prompt.AppendLine(schema);

            var chatCompletion = await _client.CompleteChatAsync(prompt.ToString());
           
            return chatCompletion;
        } 
    }
}
