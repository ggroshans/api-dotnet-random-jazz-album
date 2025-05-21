namespace Api.Models.DTOs.ResponseDTOs
{
    public class AlbumResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Theme { get; set; }
        public string ImageUrl { get; set; }
        public string Label { get; set; }
        public int PopularityScore { get; set; }
        public double? PercentileScore { get; set; }
        public List<string> PopularTracks { get; set; }
        public string ReleaseYear { get; set; }
        public int TotalTracks { get; set; }
        public string SpotifyId { get; set; }
        public string YoutubeId { get; set; }
        public string AppleMusicId { get; set; }
        public string AmazonMusicId { get; set; }
        public string PandoraId { get; set; }
        public List<ArtistResponseDto> Artists { get; set; }
        public List<GenreResponseDto> Genre { get; set; }
        public List<SubgenreResponseDto> Subgenres { get; set; }
        public List<MoodResponseDto> Moods { get; set; }
    }
}
