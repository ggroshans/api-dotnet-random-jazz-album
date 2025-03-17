namespace Api.Domain.Entities
{
    public class Artist
    {
        //spotify fields
        public int Id { get; set; }
        public string Name { get; set; }
        public List<string>? Genres { get; set; }
        public string? ImageUrl { get; set; }
        public int PopularityScore { get; set; }
        public int? NormalizedScore { get; set; }
        public string SpotifyId { get; set; }

        //gpt fields
        public string Biography { get; set; }

        //relationships
        public List<AlbumArtist> AlbumArtists { get; set; } = new List<AlbumArtist>();
        public Guid DiscoTransactionId { get; set; }
        public DiscoTransaction DiscoTransaction { get; set; }
    }
}
