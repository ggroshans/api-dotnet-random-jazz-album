using System.Net;
using System.Text.Json;

namespace Api.Services.ApiServices
{
    public class StreamingLinksService
    {
        public StreamingLinksService(){}
        public async Task<Dictionary<string,string>> GetLinks(string spotifyAlbumId)
        {
            using var _httpClient = new HttpClient();
            var url = $"https://open.spotify.com/album/{spotifyAlbumId}";
            var encodedUrl = WebUtility.UrlEncode(url);
            var songLinkUrl = $"https://api.song.link/v1-alpha.1/links?url={encodedUrl}";

            var response = await _httpClient.GetAsync(songLinkUrl);
            if (!response.IsSuccessStatusCode)
                return null;

            var responseBody = await response.Content.ReadAsStringAsync();
            var platforms = ParseResponse(responseBody);

            return platforms;
        }

        private Dictionary<string, string> ParseResponse(string responseBody)
        {
            var platforms = new Dictionary<string, string>();

            using var jsonDocument = JsonDocument.Parse(responseBody);
            var root = jsonDocument.RootElement;

            if (root.TryGetProperty("entitiesByUniqueId", out var streamingObjects)) 
            {
               foreach (var streamingObj in streamingObjects.EnumerateObject())
               {
                    var splitKey = streamingObj.Name.Split("::");
                    if (splitKey.Length == 2)
                    {
                        var platformName = splitKey[0];
                        var platformId = splitKey[1];
                        platforms[platformName] = platformId;
                    }
                }
            }
            return platforms;
        }

    }
}
