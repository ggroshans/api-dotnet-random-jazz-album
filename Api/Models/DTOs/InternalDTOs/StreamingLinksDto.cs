namespace Api.Models.DTOs.InternalDTOs
{
    public class StreamingLinksDto
    {
        public int AlbumId { get; set; }

        public string? SpotifyUrl { get; set; }
        public string? YoutubeUrl { get; set; }
        public string? AppleMusicUrl { get; set; }
        public string? AmazonMusicUrl { get; set; }
        public string? PandoraUrl { get; set; }
    }
}