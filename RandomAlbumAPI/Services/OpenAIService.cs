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

            string schema = @"
            {
              ""Title"": ""Album title: STRING"",
              ""ArtistName"": ""Musician's name: STRING"",
              ""ReleaseDate"": ""Album release date (MM/DD/YYYY): STRING"",
              ""Genre"": ""Primary music genre (e.g., rock, jazz): STRING"",
              ""SubGenres"": ""List of subgenres: STRING[]"",
              ""CoverImageURL"": ""URL of the album cover image: STRING"",
              ""TotalTracks"": ""Total number of tracks in the album: INT"",
              ""Description"": ""Description of the album: STRING"",
              ""Label"": ""Record label: STRING"",
              ""Mood"": ""List of moods that describe the album: STRING[]"",
              ""FeaturedArtists"": ""List of featured artists: STRING[]"",
              ""RecordingLocation"": ""Where the album was recorded: STRING"",
              ""Length"": ""Total album length in minutes: INT"",
              ""ReleaseNotes"": ""Artist's notes or comments on the release: STRING"",
              ""AlbumTheme"": ""Main theme or concept of the album: STRING"",
              ""FanRating"": ""Average fan rating (out of 10): INT"",
              ""StoryBehindTitle"": ""Explanation of the album's title and significance: STRING"",
              ""AlbumPosition"": ""Position of the album in the artist's discography (e.g., 1st, 5th): INT"",
              ""PopularTracks"": ""List of the most popular tracks from the album: STRING[]"",
              ""Awards"": ""List of awards in 'award (year)' format: STRING[]"",
              ""CoverArtDescription"": ""Brief description of the album cover design and concept: STRING"",
              ""PersonalAnecdotes"": ""Artist or producer anecdotes about the making of the album: STRING"",
              ""SetlistContext"": ""Details on how tracks fit into live performances or tours: STRING"",
              ""CreatedAt"": ""Timestamp when the record was created: INT"",
              ""UpdatedAt"": ""Timestamp when the record was last updated: INT""
            }";


            var prompt = new StringBuilder();
            prompt.AppendLine($"For the album: {albumName} by {artistName}, return a json object using this schema:");
            prompt.AppendLine(schema);

            var chatCompletion = await _client.CompleteChatAsync(prompt.ToString());
           
            return chatCompletion;
        } 
    }
}
