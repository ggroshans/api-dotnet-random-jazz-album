namespace Api.Domain.Entities
{
    public class AlbumGenre
    {
        public int AlbumId { get; set; }
        public Album Album { get; set; }
        public int GenreTypeId { get; set; }
        public GenreType GenreType { get; set; }
        public Guid DiscoTransactionId { get; set; }
        public DiscoTransaction DiscoTransaction { get; set; }
    }
}
