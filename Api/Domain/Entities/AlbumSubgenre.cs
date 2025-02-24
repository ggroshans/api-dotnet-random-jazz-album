namespace Api.Domain.Entities
{
    public class AlbumSubgenre
    {
        public int AlbumId { get; set; }
        public Album Album { get; set; }
        public int SubgenreId { get; set; }
        public Subgenre Subgenre { get; set; }
    }
}
