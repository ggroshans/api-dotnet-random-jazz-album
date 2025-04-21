namespace Api.Services.ApiServices.Spotify.SpotifyAuthService
{
    public interface ISpotifyClient
    {
        public Task<string> GetAccessToken(string clientId, string clientSecret);
    }
}
