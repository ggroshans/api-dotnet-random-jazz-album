namespace Api.Domain.Entities
{
    public class Genre
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // relationships
        public List<Subgenre> Subgenres { get; set; } = new List<Subgenre>();
        public List<Album> Albums { get; set; } = new List<Album>();
        public Guid DiscoTransactionId { get; set; }
        public DiscoTransaction DiscoTransaction { get; set; }
    }
}
