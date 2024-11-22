namespace Api.Services.AuthServices.Spotify
{
    public interface ISpotifyClient
    {
        public Task<string> GetAccessToken(string clientId, string clientSecret);
    }
}
