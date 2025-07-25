namespace Api.Models.DTOs.ResponseDTOs
{
    public class ArtistResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Biography { get; set; }
        public List<string> Genres { get; set; }
        public string ImageUrl { get; set; }
        public int? PopularityRating { get; set; }
        public string? Instrument { get; set; }
        public int? AlbumCount { get; set; }
        public int? AverageAlbumScore { get; set; }
        public string? DebutYear { get; set; }


        //computed fields
        public List<AlbumResponseDto> NoteableAlbums { get; set; } = new List<AlbumResponseDto>();
    }
}
