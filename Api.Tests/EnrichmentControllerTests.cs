using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

// Project usings
using Api.Controllers;
using Api.Data;
using Api.Domain.Entities;

namespace Api.Tests.Controllers
{
    public class EnrichmentControllerTests
    {
        private static MusicDbContext NewInMemoryDb()
        {
            var options = new DbContextOptionsBuilder<MusicDbContext>()
                .UseInMemoryDatabase($"Enrichment_{System.Guid.NewGuid():N}")
                .Options;
            var db = new MusicDbContext(options);
            db.Database.EnsureCreated();
            return db;
        }

        [Fact]
        public async Task BatchProcess_returns_200_and_success_message()
        {
            using var db = NewInMemoryDb();

            // Seed with one album and one artist so enrichment logic has something to work with
            db.Albums.Add(new Album
            {
                Title = "Test Album",
                ReleaseDate = "2020-01-01",
                ReleaseDatePrecision = "Day",
                ImageUrl = "img.png",
                SpotifyId = "sp123",
                SpotifyPopularity = 50
            });
            db.Artists.Add(new Artist
            {
                Name = "Test Artist",
                SpotifyId = "art123",
                SpotifyPopularity = 40
            });
            await db.SaveChangesAsync();

            var sut = new EnrichmentController(db);

            var result = await sut.BatchProcess();

            var ok = result.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeAssignableTo<object>(); // we just care that something comes back

            // Optionally check the dynamic response message
            var msg = ok.Value?.ToString();
            msg.Should().Contain("Enrichment batch completed");
        }

        [Fact]
        public async Task BatchProcess_returns_500_when_no_data_present()
        {
            using var db = NewInMemoryDb();
            var sut = new EnrichmentController(db);

            var result = await sut.BatchProcess();

            var error = result.Should().BeOfType<ObjectResult>().Subject;
            error.StatusCode.Should().Be(500);
        }
    }
}
