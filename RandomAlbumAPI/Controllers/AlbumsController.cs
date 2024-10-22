using Microsoft.AspNetCore.Mvc;
using RandomAlbumAPI.Models;
using RandomAlbumAPI.Services;

namespace RandomAlbumAPI.Controllers
{

    [Route("api/Album")]
    [ApiController]
    public class AlbumsController : ControllerBase
    {
        
        private readonly ILogger<AlbumsController> _logger;
        public readonly OpenAIService _openAIservice;

        public AlbumsController(ILogger<AlbumsController> logger, OpenAIService openAIService)
        {
            _logger = logger;
            _openAIservice = openAIService;
        }

        [HttpPost]
        public IActionResult CreateAlbum([FromBody] AlbumRequest albumRequest)
        {
            _logger.LogInformation("FIRED: CreateAlbum endpoint was called");
            _logger.LogInformation($"Artist Name: {albumRequest.ArtistName}, Album Name: {albumRequest.AlbumName}");
            var chatCompletionObject = _openAIservice.GetAlbumDetailAsync(albumRequest.ArtistName, albumRequest.AlbumName).Result;
            var response = chatCompletionObject.Content[0].Text;
            return Ok(response);

        }
         

    }
}
