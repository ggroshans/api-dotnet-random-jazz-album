using Microsoft.AspNetCore.Mvc;
using RandomAlbumAPI.Models;

namespace RandomAlbumAPI.Controllers
{

    [Route("api/Album")]
    [ApiController]
    public class AlbumsController : ControllerBase
    {
        
        private readonly ILogger<AlbumsController> _logger;

        public AlbumsController(ILogger<AlbumsController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public IActionResult CreateAlbum([FromBody] AlbumRequest albumRequest)
        {
            _logger.LogInformation("FIRED: CreateAlbum endpoint was called");
            _logger.LogInformation($"Artist Name: {albumRequest.ArtistName}, Album Name: {albumRequest.AlbumName}");
            return Ok("Worked");
        }
         

    }
}
