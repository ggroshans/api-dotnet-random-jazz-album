namespace Api.Domain.Entities
{
    public class GenreType
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // relationships
        public List<Subgenre> Subgenres { get; set; } = new List<Subgenre>();
        public List<AlbumGenre> AlbumGenres { get; set; } = new List<AlbumGenre>();
    }
}
