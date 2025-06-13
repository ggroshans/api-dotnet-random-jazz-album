namespace Api.Domain.Entities
{
    public class Album
    {
        //spotify api fields
        public int Id { get; set; }
        public string Title { get; set; }
        public string ReleaseYear { get; set; }
        public int? TotalTracks { get; set; }
        public string ImageUrl { get; set; }
        public int PopularityScore { get; set; }
        public string? Label { get; set; }

        // gpt api fields
        public string? Description { get; set; }
        public string? AlbumTheme { get; set; }
        public bool? IsOriginalRelease { get; set; }

        // songlink api fields
        public string SpotifyId { get; set; } // Technically retrieved from spotify api
        public string? YoutubeId { get; set; }
        public string? AppleMusicId { get; set; }
        public string? AmazonMusicId { get; set; }
        public string? PandoraId { get; set; }

        // computed fields
        public double? PercentileScore { get; set; }
        public double? AverageEmotionScore { get; set; }
        public double? AverageEnergyScore { get; set; }

        // relationships
        public List<AlbumArtist> AlbumArtists { get; set; } = new List<AlbumArtist>();
        public List<AlbumSubgenre>? AlbumSubgenres { get; set; } = new List<AlbumSubgenre>();
        public List<AlbumMood>? AlbumMoods { get; set; } = new List<AlbumMood>();
        public List<AlbumGenre>? AlbumGenres { get; set; } = new List<AlbumGenre>();
        public List<AlbumJazzEra>? AlbumJazzEras { get; set; }
        public Guid DiscoTransactionId { get; set; }
        public DiscoTransaction DiscoTransaction { get; set; }
    }
}
