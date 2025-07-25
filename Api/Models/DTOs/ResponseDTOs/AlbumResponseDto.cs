using Api.Domain.Entities;

namespace Api.Models.DTOs.ResponseDTOs
{
    public class AlbumResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public string ImageUrl { get; set; }
        public string? Label { get; set; }
        public int? SortableDate { get; set; }
        public int? TotalTracks { get; set; }
        public bool? IsOriginalRelease { get; set; }
        public int? OriginalAlbumOrder { get; set; }
        public string? AdditionalArtists { get; set; }  
        public int? PopularityRating { get; set; }
        public int? AverageEmotionalTone {  get; set; }
        public int? AverageEnergyLevel { get; set; }
        public string? SpotifyId { get; set; }
        public string? YoutubeId { get; set; }
        public string? AppleMusicId { get; set; }
        public string? AmazonMusicId { get; set; }
        public string? PandoraId { get; set; }
        public List<string> JazzEras { get; set; }
        public List<ArtistResponseDto> Artists { get; set; }
        public List<string> Genres { get; set; }
        public List<string> Subgenres { get; set; }
        public List<MoodResponseDto> Moods { get; set; }
    }
}
