namespace Api.Domain.Entities
{
    public class JazzEraType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? StartYear { get; set; }
        public int? EndYear { get; set; }

        // relationships
        public List<AlbumJazzEra> AlbumJazzEras { get; set; } = new List<AlbumJazzEra>();
    }
}
