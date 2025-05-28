namespace Api.Models.DTOs.InternalDTOs
{
    public class MoodProcessingDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Valence { get; set; }
        public double Arousal { get; set; }
    }
}
