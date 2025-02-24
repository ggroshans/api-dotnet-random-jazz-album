namespace Api.Domain.Entities
{
    public class Mood
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<AlbumMood> AlbumMoods { get; set; } = new List<AlbumMood>();
    }
}
