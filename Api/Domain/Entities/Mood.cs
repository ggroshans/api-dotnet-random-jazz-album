namespace Api.Domain.Entities
{
    public class Mood
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // relationships
        public List<AlbumMood> AlbumMoods { get; set; } = new List<AlbumMood>();
        public Guid DiscoTransactionId { get; set; }
        public DiscoTransaction DiscoTransaction { get; set; }
    }
}
