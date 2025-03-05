namespace Api.Models
{
    public class RequestDetails
    {
        public string? PrimaryArtistName { get; set; }
        public int? ArtistCount { get; set; } = 0;
        public int? AlbumCount { get; set; } = 0;
        public int? SubgenreCount { get; set; } = 0;
        public int? MoodCount { get; set; } = 0;
    }
}
