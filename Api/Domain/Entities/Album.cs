namespace Api.Domain.Entities
{
    public class Album
    {
        //spotify fields
        public int Id { get; set; }
        public string Title { get; set; }
        public string ReleaseYear { get; set; }
        public int TotalTracks { get; set; }
        public string ImageUrl { get; set; }
        public int PopularityScore { get; set; }
        public double? PercentileScore { get; set; }
        public string Label { get; set; }
        public string SpotifyId { get; set; }

        //gpt fields
        public string? Description { get; set; }
        public string? AlbumTheme { get; set; }
        public List<string>? PopularTracks { get; set; }


        // relationships
        public List<AlbumArtist> AlbumArtists { get; set; } = new List<AlbumArtist>();
        public List<AlbumSubgenre>? AlbumSubgenres { get; set; } = new List<AlbumSubgenre>();
        public List<AlbumMood>? AlbumMoods { get; set; } = new List<AlbumMood>();
        public int GenreId { get; set; }
        public Genre Genre { get; set; }
        public Guid DiscoTransactionId { get; set; }
        public DiscoTransaction DiscoTransaction { get; set; }
    }
}
