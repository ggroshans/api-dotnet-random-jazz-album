using Microsoft.EntityFrameworkCore;
using Api.Data;
using Api.Services.ApiServices;
using Api.Services.ApiServices.Spotify;
using Api.DTOs;
using Api.Domain.Entities;
using Api.Utilities;

namespace Api.Services
{
    public class PopulateDbService
    {
        private readonly MusicDbContext _db;
        private readonly SpotifyApiService _spotifyApiService;
        private readonly GptApiService _gptApiService;

        public PopulateDbService(MusicDbContext db, SpotifyApiService spotifyApiService, GptApiService gptApiService)
        {
            _db = db;
            _spotifyApiService = spotifyApiService;
            _gptApiService = gptApiService;
        }

        public async Task PopulateAlbumAsync(Guid discoTransactionId, List<AlbumDto> albumDtos)
        {
            foreach (var albumDto in albumDtos)
            {
                // -------- Overall Album --------
                var album = new Album
                {
                    Name = StringUtils.CapitalizeAndFormat(albumDto.Name),
                    ReleaseYear = albumDto.ReleaseYear,
                    TotalTracks = albumDto.TotalTracks,
                    ImageUrl = albumDto.ImageUrl,
                    SpotifyId = albumDto.SpotifyId,
                    Description = albumDto.Description, 
                    PopularTracks = albumDto.PopularTracks,
                    AlbumTheme = albumDto.AlbumTheme,
                    DiscoTransactionId = discoTransactionId,
                };

                _db.Albums.Add(album);
                await _db.SaveChangesAsync();

                //  -------- Artist --------
                foreach (var artistDto in albumDto.Artists)
                {
                    var existingArtist = await _db.Artists.FirstOrDefaultAsync(a => a.Name.ToLower() == artistDto.Name.ToLower()); 

                    if (existingArtist == null)
                    {
                        var populatedArtist = await GetArtistDetailsAsync(artistDto);
                        existingArtist = new Artist
                        {
                            Name = StringUtils.CapitalizeAndFormat(artistDto.Name),
                            Biography = populatedArtist.Biography,
                            Genres = populatedArtist.Genres,
                            ImageUrl = populatedArtist.ImageUrl,
                            PopularityScore = populatedArtist.PopularityScore,
                            SpotifyId = artistDto.SpotifyId,
                            DiscoTransactionId = discoTransactionId,
                        };
                        _db.Artists.Add(existingArtist);
                        await _db.SaveChangesAsync();
                        
                    }
                    // ** Album Artist Junction Table **
                    if (!await _db.AlbumArtists.AnyAsync(aa => aa.ArtistId == artistDto.Id && aa.AlbumId == album.Id))
                    {
                        _db.AlbumArtists.Add(new AlbumArtist
                        {
                            Album = album,
                            Artist = existingArtist,
                            DiscoTransactionId = discoTransactionId,
                        });
                        await _db.SaveChangesAsync();
                    }
                }
                // ------- Genre -------
                var existingGenre = await _db.Genres.FirstOrDefaultAsync(g => g.Name.ToLower() == albumDto.Genre.ToLower());
                if (existingGenre == null)
                {
                    existingGenre = new Genre
                    {
                        Name = StringUtils.CapitalizeAndFormat(albumDto.Genre),
                        DiscoTransactionId = discoTransactionId,
                    };

                    _db.Genres.Add(existingGenre);
                    await _db.SaveChangesAsync();
                }
                // ** Album Genre Junction Table **
                if (!await _db.AlbumGenres.AnyAsync(ag => ag.GenreId == existingGenre.Id && ag.AlbumId == album.Id))
                {
                    _db.AlbumGenres.Add(new AlbumGenre
                    { 
                        Album = album,
                        Genre = existingGenre,
                        DiscoTransactionId = discoTransactionId,
                    });
                    await _db.SaveChangesAsync();
                }

                // ------- Mood -------
                foreach (var moodDto in albumDto.Moods)
                {
                    var existingMood = await _db.Moods.FirstOrDefaultAsync(m => m.Name.ToLower() == moodDto.ToLower());
                    
                    if (existingMood == null)
                    {
                        existingMood = new Mood
                        {
                            Name = StringUtils.CapitalizeAndFormat(moodDto),
                            DiscoTransactionId = discoTransactionId,
                        };

                        _db.Moods.Add(existingMood);
                        await _db.SaveChangesAsync();
                    }

                // ** Album Mood Junction Table **
                    if (!await _db.AlbumMoods.AnyAsync(am => am.MoodId == existingMood.Id && am.AlbumId == album.Id))
                    {
                        _db.AlbumMoods.Add(new AlbumMood
                        {
                            Album = album,
                            Mood = existingMood,
                            DiscoTransactionId = discoTransactionId,
                        });
                        await _db.SaveChangesAsync();
                    }
                }

                // ------- Subgenres -------
                foreach (var subgenreDto in albumDto.Subgenres)
                {
                    var existingSubgenre = _db.Subgenres.FirstOrDefault(sg => sg.Name.ToLower() == subgenreDto.ToLower());


                    if (existingSubgenre == null)
                    {
                        existingSubgenre = new Subgenre
                        {
                            Name = StringUtils.CapitalizeAndFormat(subgenreDto),
                            GenreId = existingGenre.Id,
                            DiscoTransactionId = discoTransactionId,
                        };
                        _db.Subgenres.Add(existingSubgenre);
                        await _db.SaveChangesAsync();
                    }

                // ** Album Subgenre Junction Table **
                    if (!await _db.AlbumSubgenres.AnyAsync(asg => asg.SubgenreId == existingSubgenre.Id && asg.AlbumId == album.Id))
                    {
                        _db.AlbumSubgenres.Add(new AlbumSubgenre
                        {
                            Album = album,
                            Subgenre = existingSubgenre,
                            DiscoTransactionId = discoTransactionId,
                        });
                        await _db.SaveChangesAsync();
                    }
                }
            }
        }

        public async Task<ArtistDto> GetArtistDetailsAsync(ArtistDto artist)
        {
            var spotifyArtist = await _spotifyApiService.GetSpotifyArtist(artist);
            var gptArtist = await _gptApiService.GetGptArtistDetails(spotifyArtist);
            return gptArtist;
        }
    }
}
