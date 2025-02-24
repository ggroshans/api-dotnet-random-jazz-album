namespace Api.Domain.Entities
{
    public class AlbumMood
    {
        // relationships
        public int AlbumId { get; set; }
        public Album Album { get; set; }
        public int MoodId { get; set; }
        public Mood Mood { get; set; }
        public Guid GptBatchUpdateId { get; set; }
        public GptBatchUpdate GptBatchUpdate { get; set; } 
    }
}
