namespace Api.Models.DTOs.InternalDTOs
{
    public class ArtistProcessingDto
    {
        //spotify fields
        public int Id { get; set; }
        public string Name { get; set; }
        public List<string>? Genres { get; set; }
        public string ImageUrl { get; set; }
        public int PopularityScore { get; set; }
        public string SpotifyId { get; set; }

        //gpt fields
        public string Biography { get; set; }
    }
}
