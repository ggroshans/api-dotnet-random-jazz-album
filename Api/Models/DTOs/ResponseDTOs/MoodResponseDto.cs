namespace Api.Models.DTOs.ResponseDTOs
{
    public class MoodResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? EmotionalTone { get; set; }
        public int? EnergyLevel { get; set; }
    }
}
