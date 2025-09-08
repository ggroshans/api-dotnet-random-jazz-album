using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

using Api.Controllers;
using Api.Data;
using Api.Models.DTOs.RequestDtos;       // CreateDiscographyRequest
using Api.Models.DTOs.InternalDTOs;      // AlbumProcessingDto
using Api.Domain.Entities;               // DiscoTransaction
using Api.Services.Contracts;            // ISpotifyApiService, IGptApiService, IPopulateDbService

namespace Api.Tests.Controllers
{
    public class AdminControllerTests
    {
        private static MusicDbContext NewInMemoryDb()
        {
            var options = new DbContextOptionsBuilder<MusicDbContext>()
                .UseInMemoryDatabase($"AdminController_{System.Guid.NewGuid():N}")
                .Options;
            var db = new MusicDbContext(options);
            db.Database.EnsureCreated();
            return db;
        }

        private static AdminController MakeSut(
            MusicDbContext db,
            out Mock<ISpotifyApiService> spotify,
            out Mock<IGptApiService> gpt,
            out Mock<IPopulateDbService> populate)
        {
            spotify = new Mock<ISpotifyApiService>(MockBehavior.Strict);
            gpt = new Mock<IGptApiService>(MockBehavior.Strict);
            populate = new Mock<IPopulateDbService>(MockBehavior.Strict);

            return new AdminController(db, spotify.Object, gpt.Object, populate.Object);
        }

        [Fact]
        public async Task CreateDiscographyFromArtist_returns_400_when_name_missing_or_blank()
        {
            using var db = NewInMemoryDb();
            var sut = MakeSut(db, out _, out _, out _);

            (await sut.CreateDiscographyFromArtist(new CreateDiscographyRequest { ArtistName = null }))
                .Should().BeOfType<BadRequestObjectResult>();

            (await sut.CreateDiscographyFromArtist(new CreateDiscographyRequest { ArtistName = "   " }))
                .Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task CreateDiscographyFromArtist_returns_404_when_spotify_returns_no_albums()
        {
            using var db = NewInMemoryDb();
            var sut = MakeSut(db, out var spotify, out _, out _);

            spotify.Setup(s => s.GetSpotifyAlbums("Miles Davis"))
                   .ReturnsAsync(Enumerable.Empty<AlbumProcessingDto>());

            var result = await sut.CreateDiscographyFromArtist(new CreateDiscographyRequest { ArtistName = "Miles Davis" });

            result.Should().BeOfType<NotFoundObjectResult>();
            spotify.Verify(s => s.GetSpotifyAlbums("Miles Davis"), Times.Once);
        }

        [Fact]
        public async Task CreateDiscographyFromArtist_returns_400_when_gpt_batch_reports_error()
        {
            using var db = NewInMemoryDb();
            var sut = MakeSut(db, out var spotify, out var gpt, out _);

            var artist = "Miles Davis";
            var spotifyAlbums = new List<AlbumProcessingDto>
            {
                new AlbumProcessingDto { SpotifyId = "a1", Title = "Test A" }
            };

            spotify.Setup(s => s.GetSpotifyAlbums(artist))
                   .ReturnsAsync(spotifyAlbums);

            // When batchError is true, the controller just reads discoTransaction?.ErrorMessage,
            // so it's safe to return null for the transaction in this test.
            gpt.Setup(g => g.BatchProcessAlbums(spotifyAlbums, artist))
               .ReturnsAsync((null as DiscoTransaction, new List<AlbumProcessingDto>(), true));

            var result = await sut.CreateDiscographyFromArtist(new CreateDiscographyRequest { ArtistName = artist });

            result.Should().BeOfType<BadRequestObjectResult>();
            spotify.VerifyAll();
            gpt.VerifyAll();
        }

        [Fact]
        public async Task CreateDiscographyFromArtist_returns_200_and_calls_populate_on_success()
        {
            using var db = NewInMemoryDb();
            var sut = MakeSut(db, out var spotify, out var gpt, out var populate);

            var artist = "Miles Davis";
            var spotifyAlbums = new List<AlbumProcessingDto>
            {
                new AlbumProcessingDto { SpotifyId = "a1", Title = "Test A" }
            };

            var txn = new DiscoTransaction(); // assuming EF entity with parameterless ctor
            var processed = new List<AlbumProcessingDto>
            {
                new AlbumProcessingDto { SpotifyId = "p1", Title = "Processed A" }
            };

            spotify.Setup(s => s.GetSpotifyAlbums(artist))
                   .ReturnsAsync(spotifyAlbums);

            gpt.Setup(g => g.BatchProcessAlbums(spotifyAlbums, artist))
               .ReturnsAsync((txn, processed, false));

            populate.Setup(p => p.PopulateAlbumAsync(txn, processed))
                    .Returns(Task.CompletedTask);

            var result = await sut.CreateDiscographyFromArtist(new CreateDiscographyRequest { ArtistName = artist });

            result.Should().BeOfType<OkObjectResult>();
            spotify.VerifyAll();
            gpt.VerifyAll();
            populate.Verify(p => p.PopulateAlbumAsync(txn, processed), Times.Once);
        }
    }
}
