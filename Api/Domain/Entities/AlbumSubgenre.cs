namespace Api.Domain.Entities
{
    public class AlbumSubgenre
    {
        // relationships
        public int AlbumId { get; set; }
        public Album Album { get; set; }
        public int SubgenreId { get; set; }
        public Subgenre Subgenre { get; set; }
        public Guid DiscoTransactionId { get; set; }
        public DiscoTransaction DiscoTransaction { get; set; }
    }
}
