namespace Api.Models.DTOs.InternalDTOs
{
    public class MoodProcessingDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Valence { get; set; }
        public int Arousal { get; set; }
    }
}
