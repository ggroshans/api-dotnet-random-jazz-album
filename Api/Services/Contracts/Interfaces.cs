using Api.Domain.Entities;
using Api.Models.DTOs.InternalDTOs;

namespace Api.Services.Contracts
{
    public interface ISpotifyApiService
    {
        Task<IEnumerable<AlbumProcessingDto>> GetSpotifyAlbums(string artistName);
        Task<ArtistProcessingDto> GetSpotifyArtist(ArtistProcessingDto artist);
    }

    public interface IGptApiService
    {
        Task<(DiscoTransaction? discoTransaction, List<AlbumProcessingDto> processedAlbums, bool batchError)>
            BatchProcessAlbums(IEnumerable<AlbumProcessingDto> spotifyAlbums, string artistName);
        Task<ArtistProcessingDto> GetGptArtistDetails(object spotifyArtist);
    }

    public interface IPopulateDbService
    {
        Task PopulateAlbumAsync(DiscoTransaction discoTransaction, IEnumerable<AlbumProcessingDto> processedAlbums);
    }

    public interface IStreamingLinksService
    {
        Task<Dictionary<string,string>> GetLinksForAlbumAsync(string albumId);
        Task SaveLinksAsync(int albumId, StreamingLinksDto links);
    }
}
