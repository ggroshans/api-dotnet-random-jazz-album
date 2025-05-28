namespace Api.Domain.Entities
{
    public class Subgenre
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // relationships
        public int GenreTypeId { get; set; }
        public GenreType GenreType { get; set; }
        public List<AlbumSubgenre> AlbumSubgenres { get; set; } = new List<AlbumSubgenre>();
        public Guid DiscoTransactionId { get; set; }
        public DiscoTransaction DiscoTransaction { get; set; }
    }
}
