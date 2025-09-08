
using System.ComponentModel.DataAnnotations;

namespace Api.Models.DTOs.RequestDtos
{
    public class CreateDiscographyRequest
    {
        [Required] public string ArtistName { get; set; } = default!;
    }
}