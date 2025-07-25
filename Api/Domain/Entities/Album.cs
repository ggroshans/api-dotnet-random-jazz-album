namespace Api.Domain.Entities
{
    public class Album
    {
        //spotify api fields
        public int Id { get; set; }
        public string Title { get; set; }
        public string ReleaseDate { get; set; }
        public string ReleaseDatePrecision { get; set; }
        public int? TotalTracks { get; set; }
        public string ImageUrl { get; set; }
        public int SpotifyPopularity { get; set; }
        public string? Label { get; set; }
        public string? AdditionalArtists { get; set; } 

        // gpt api fields
        public string? Description { get; set; }
        public bool? IsOriginalRelease { get; set; }

        // songlink api fields
        public string SpotifyId { get; set; } // Technically retrieved from spotify api
        public string? YoutubeId { get; set; }
        public string? AppleMusicId { get; set; }
        public string? AmazonMusicId { get; set; }
        public string? PandoraId { get; set; }

        // computed fields 
        public int? SortableDate { get; set; } 
        public int? PopularityRating { get; set; }
        public int? AverageEmotionalTone { get; set; }
        public int? AverageEnergyLevel { get; set; }

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
