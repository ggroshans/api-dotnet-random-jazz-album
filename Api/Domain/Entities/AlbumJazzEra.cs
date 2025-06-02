namespace Api.Domain.Entities
{
    public class AlbumJazzEra
    {
        public int AlbumId { get; set; }
        public virtual Album Album { get; set; }
        public int JazzEraTypeId { get; set; }
        public JazzEraType JazzEraType { get; set; }
    }
}
