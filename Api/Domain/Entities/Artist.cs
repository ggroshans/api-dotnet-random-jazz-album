using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Domain.Entities
{
    public class Artist
    {
        // spotify fields
        public int Id { get; set; }
        public string Name { get; set; }
        public List<string>? Genres { get; set; }
        public string? ImageUrl { get; set; }
        public int PopularityScore { get; set; }
        public string SpotifyId { get; set; }

        // gpt fields
        public string? BirthYear { get; set; }
        public string? DeathYear { get; set; }
        public string? Biography { get; set; }
        public string? Instrument { get; set; }
        public string? AdditionalArtists { get; set; }
        public List<string> RelatedArtists { get; set; } = new List<string>();
        public List<string> Influences { get; set; } = new List<string>();

        // computed fields
        public double? PercentileScore { get; set; }
        public int AlbumCount { get; set; }
        [NotMapped]
        public List<Album> NoteableAlbums { get; set; } = new List<Album>();
        public int? AverageAlbumScore { get; set; }
        public SubgenreBreakdown? SubgenreBreakdown { get; set; }
        public YearsActive? YearsActive { get; set; }
        public MoodBreakdown? MoodBreakdown { get; set; }

        // relationships
        public List<AlbumArtist> AlbumArtists { get; set; } = new List<AlbumArtist>();
        public Guid DiscoTransactionId { get; set; }
        public DiscoTransaction DiscoTransaction { get; set; }
    }
}
