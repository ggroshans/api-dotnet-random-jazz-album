namespace Api.Domain.Entities
{
    public class AlbumGenre
    {
        // relationships
        public int AlbumId { get; set; }
        public Album Album { get; set; }
        public int GenreId { get; set; }
        public Genre Genre { get; set; }
        public Guid GptBatchUpdateId { get; set; }
        public GptBatchUpdate GptBatchUpdate { get; set; }
    }
}
