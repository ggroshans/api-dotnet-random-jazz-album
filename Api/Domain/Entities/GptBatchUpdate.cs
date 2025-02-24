namespace Api.Domain.Entities
{
    public class GptBatchUpdate
    {
        public Guid Id { get; set; }
        public DateTime CallStamp { get; set; }
        public string RequestDetails { get; set; }
        public int ResponseStatusCode { get; set; }
        public string? ErrorMessage { get; set; }

        // relationships

        public List<Album> Albums { get; set; } = new List<Album>();
        public List<Artist> Artists { get; set; } = new List<Artist>();
        public List<Genre> Genres { get; set; } = new List<Genre>();
        public List<Subgenre> Subgenres { get; set; } = new List<Subgenre>();
        public List<Mood> Moods { get; set; } = new List<Mood>();
        public List<AlbumArtist> AlbumArtists { get; set; } = new List<AlbumArtist>();
        public List<AlbumGenre> AlbumGenres { get; set; } = new List<AlbumGenre>();
        public List<AlbumMood> AlbumMoods { get; set; } = new List<AlbumMood>();
        public List<AlbumSubgenre> AlbumSubgenres { get; set; } = new List<AlbumSubgenre>();
    }
}
