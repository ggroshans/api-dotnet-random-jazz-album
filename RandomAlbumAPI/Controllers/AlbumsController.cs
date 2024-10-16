using Microsoft.AspNetCore.Mvc;
using RandomAlbumAPI.Models;

namespace RandomAlbumAPI.Controllers
{

    [Route("api/Album")]
    [ApiController]
    public class AlbumsController : ControllerBase
    {

        public IActionResult CreateAlbum([FromBody] AlbumRequest albumRequest)
        {
            Console.WriteLine("FIRED");
            return Ok();
        }
         

    }
}
