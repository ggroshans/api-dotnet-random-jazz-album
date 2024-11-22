using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace Api.Services.AuthServices.Spotify
{
    public class SpotifyClient : ISpotifyClient
    {
        private const string TokenUrl = "https://accounts.spotify.com/api/token"; // Corrected URL

        async Task<string> ISpotifyClient.GetAccessToken(string clientId, string clientSecret)
        {
            using (var httpClient = new HttpClient())
            {
                var byteArray = System.Text.Encoding.ASCII.GetBytes($"{clientId}:{clientSecret}");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                var parameters = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "client_credentials")
                });

                var response = await httpClient.PostAsync(TokenUrl, parameters);
                response.EnsureSuccessStatusCode();
                var jsonResponse = await response.Content.ReadAsStringAsync();

                dynamic responseObject = JsonConvert.DeserializeObject(jsonResponse);
                string accessToken = responseObject.access_token;

                return accessToken;            
            }
        }
    }
}
