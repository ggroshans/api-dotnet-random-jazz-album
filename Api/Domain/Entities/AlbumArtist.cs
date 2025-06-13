namespace Api.Domain.Entities
{
    public class AlbumArtist
    {
        // relationships 
        public int AlbumId { get; set; }
        public Album Album { get; set; }
        public int ArtistId { get; set; }
        public Artist Artist { get; set; }
        public int? OriginalAlbumOrder { get; set; } 
        public Guid DiscoTransactionId { get; set; }
        public DiscoTransaction DiscoTransaction { get; set; }
    }
}
