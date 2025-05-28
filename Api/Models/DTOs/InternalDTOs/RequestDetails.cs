namespace Api.Models.DTOs.InternalDTOs
{
    public class RequestDetails
    {
        public string? PrimaryArtistName { get; set; }
        public int? NewArtistCount { get; set; } = 0;
        public int? NewAlbumCount { get; set; } = 0;
        public int? NewSubgenreCount { get; set; } = 0;
        public int? NewMoodCount { get; set; } = 0;
    }
}
