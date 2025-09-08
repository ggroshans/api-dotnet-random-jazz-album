using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

// 🔧 Adjust these to your actual namespaces
using Api.Controllers;
using Api.Data;              // MusicDbContext
using Api.Domain.Entities;
using Api.Models.DTOs.ResponseDTOs;          // Album entity (or your DTO + mapping if controller returns DTOs)

namespace Api.Tests.Controllers
{
    public class AlbumControllerTests
    {
        private static MusicDbContext NewInMemoryDb(string? name = null)
        {
            var dbName = name ?? Guid.NewGuid().ToString("N");
            var opts = new DbContextOptionsBuilder<MusicDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;

            var ctx = new MusicDbContext(opts);
            ctx.Database.EnsureCreated();
            return ctx;
        }

        private static AlbumController NewController(MusicDbContext db)
        {
            // We pass NullLogger and null! for services the tested action doesn’t use.
            // This sidesteps Moq trying to construct concrete services like GptApiService.
            return new AlbumController(
                logger: null,
                openAIService: null!,
                spotifyApiService: null!,
                populateDbService: null!,
                dbContext: db
            );
        }

        [Fact]
        public async Task GetAlbumById_returns_NotFound_when_absent()
        {
            using var db = NewInMemoryDb();
            var sut = NewController(db);

            var result = await sut.GetAlbumById(999);

            result.Should().BeOfType<NotFoundResult>();
        }
        [Fact]
        public async Task GetAlbumById_returns_Ok_with_dto_when_present()
        {
            using var db = NewInMemoryDb();

            var album = new Album
            {
                Id = 42,
                Title = "Kind of Blue",
                Description = "Classic modal jazz album.",
                ImageUrl = "image-url.com",
                SpotifyId = "088234",
                ReleaseDate = "1959-08-17",
                ReleaseDatePrecision = "Year",
                SortableDate = 19590817,
                TotalTracks = 5,
                IsOriginalRelease = true,
                PopularityRating = 99,
                AverageEmotionalTone = 89,
                AverageEnergyLevel = 43,
                Label = "Columbia"
            };
            var artist = new Artist { Id = 1001, Name = "Miles Davis", Genres = new List<string> { "Jazz", "Blues" }, ImageUrl = "miles.jpg", SpotifyId = "23423324", PopularityRating = 100 };
            var aa = new AlbumArtist { Album = album, Artist = artist, AlbumId = album.Id, ArtistId = artist.Id, OriginalAlbumOrder = 1 };

            db.Albums.Add(album);
            db.Artists.Add(artist);
            db.AlbumArtists.Add(aa);
            await db.SaveChangesAsync();

            var sut = NewController(db);

            var result = await sut.GetAlbumById(42);

            var ok = result.Should().BeOfType<OkObjectResult>().Subject;

            // ✅ Assert DTO, not entity
            ok.Value.Should().BeOfType<AlbumResponseDto>();

            var dto = (AlbumResponseDto)ok.Value!;

            dto.Id.Should().Be(42);
            dto.Title.Should().Be("Kind of Blue");
            dto.ImageUrl.Should().Be("image-url.com");
            dto.SpotifyId.Should().Be("088234");
            dto.OriginalAlbumOrder.Should().Be(1);
            dto.Artists.Should().HaveCount(1);
            dto.Artists![0].Name.Should().Be("Miles Davis");

            // Collections exist, may be empty
            dto.Genres.Should().NotBeNull();
            dto.Subgenres.Should().NotBeNull();
            dto.Moods.Should().NotBeNull();
            dto.JazzEras.Should().NotBeNull();
        }


        [Fact]
        public async Task GetRandomAlbum_returns_Ok_with_some_album_when_exists()
        {
            using var db = NewInMemoryDb();
            db.Albums.AddRange(
                new Album { Id = 1, Title = "Blue Train", ReleaseDate = "1957", ReleaseDatePrecision = "Year", SpotifyId = "90823424", ImageUrl = "image.url.com" },
                new Album { Id = 2, Title = "A Love Supreme", ReleaseDate = "1965-06-12", ReleaseDatePrecision = "Year", SpotifyId = "90823424", ImageUrl = "image.url.com" }
            );
            await db.SaveChangesAsync();

            var sut = NewController(db);

            var result = await sut.GetRandomAlbum();

            var ok = result.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().NotBeNull();
        }

        [Fact]
        public async Task GetRandomAlbum_returns_NotFound_when_no_albums()
        {
            using var db = NewInMemoryDb();
            var sut = NewController(db);

            var result = await sut.GetRandomAlbum();

            result.Should().BeOfType<NotFoundResult>();
        }
        
        [Fact]
        public async Task GetAlbums_returns_empty_page_when_out_of_range()
        {
            using var db = NewInMemoryDb();
            var sut = new AlbumController(null!, null!, null!, null!, db);

            var result = await sut.GetAlbums(query: null, pageNumber: 99, pageSize: 10);
            var ok = Assert.IsType<OkObjectResult>(result);
            var page = Assert.IsAssignableFrom<PaginatedResponseDto<AlbumResponseDto>>(ok.Value);

            page.Items.Should().BeEmpty();
            page.HasNextPage.Should().BeFalse();
        }
    }
}
