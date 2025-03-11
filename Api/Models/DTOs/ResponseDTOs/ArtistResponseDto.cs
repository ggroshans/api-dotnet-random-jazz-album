namespace Api.Models.DTOs.ResponseDTOs
{
    public class ArtistResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Biography { get; set; }
        public List<string> Genres { get; set; }
        public string ImageUrl { get; set; }
        public int PopularityScore { get; set; }
    }
}
