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

        public async Task PopulateAlbumAsync(DiscoTransaction discoTransaction, List<AlbumDto> albumDtos)
        {
            foreach (var albumDto in albumDtos)
            {
                var dbAlbums = await _db.Albums.ToListAsync();
                var existingAlbum = dbAlbums.FirstOrDefault(a => StringUtils.NormalizeName(a.Name) == StringUtils.NormalizeName(albumDto.Name));

                if (existingAlbum == null)
                {
                    // -------- Overall Album --------
                    existingAlbum = new Album
                    {
                        Name = StringUtils.CapitalizeAndFormat(albumDto.Name),
                        ReleaseYear = albumDto.ReleaseYear,
                        TotalTracks = albumDto.TotalTracks,
                        ImageUrl = albumDto.ImageUrl,
                        SpotifyId = albumDto.SpotifyId,
                        Description = albumDto.Description, 
                        PopularTracks = albumDto.PopularTracks,
                        AlbumTheme = albumDto.AlbumTheme,
                        DiscoTransactionId = discoTransaction.Id,
                    };
                    _db.Albums.Add(existingAlbum);
                    _db.SaveChanges();
                }

                //  -------- Artist --------
                foreach (var artistDto in albumDto.Artists)
                {
                    var dbArtists = await _db.Artists.ToListAsync();
                    var existingArtist = dbArtists.FirstOrDefault(a => StringUtils.NormalizeName(a.Name) == StringUtils.NormalizeName(artistDto.Name)); 

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
                            DiscoTransactionId = discoTransaction.Id,
                        };
                        _db.Artists.Add(existingArtist);
                        _db.SaveChanges();
                        
                    }
                    // ** Album Artist Junction Table **
                    if (!await _db.AlbumArtists.AnyAsync(aa => aa.ArtistId == existingArtist.Id && aa.AlbumId == existingAlbum.Id))
                    {
                        _db.AlbumArtists.Add(new AlbumArtist
                        {
                            Album = existingAlbum,
                            Artist = existingArtist,
                            DiscoTransactionId = discoTransaction.Id,
                        });
                        _db.SaveChanges();
                    }
                }
                // ------- Genre -------
                var dbGenres = await _db.Genres.ToListAsync();
                var existingGenre = dbGenres.FirstOrDefault(g => StringUtils.NormalizeName(g.Name) == StringUtils.NormalizeName(albumDto.Genre));
                if (existingGenre == null)
                {
                    existingGenre = new Genre
                    {
                        Name = StringUtils.CapitalizeAndFormat(albumDto.Genre),
                        DiscoTransactionId = discoTransaction.Id,
                    };

                    _db.Genres.Add(existingGenre);
                    _db.SaveChanges();
                }
                // ** Album Genre Junction Table **
                if (!await _db.AlbumGenres.AnyAsync(ag => ag.GenreId == existingGenre.Id && ag.AlbumId == existingAlbum.Id))
                {
                    _db.AlbumGenres.Add(new AlbumGenre
                    { 
                        Album = existingAlbum,
                        Genre = existingGenre,
                        DiscoTransactionId = discoTransaction.Id,
                    });
                    _db.SaveChanges();
                }

                // ------- Mood -------
                foreach (var moodDto in albumDto.Moods)
                {
                    var dbMoods = await _db.Moods.ToListAsync();
                    var existingMood = dbMoods.FirstOrDefault(m => StringUtils.NormalizeName(m.Name) == StringUtils.NormalizeName(moodDto));
                    
                    if (existingMood == null)
                    {
                        existingMood = new Mood
                        {
                            Name = StringUtils.CapitalizeAndFormat(moodDto),
                            DiscoTransactionId = discoTransaction.Id,
                        };

                        _db.Moods.Add(existingMood);
                        _db.SaveChanges();
                    }

                // ** Album Mood Junction Table **
                    if (!await _db.AlbumMoods.AnyAsync(am => am.MoodId == existingMood.Id && am.AlbumId == existingAlbum.Id))
                    {
                        _db.AlbumMoods.Add(new AlbumMood
                        {
                            Album = existingAlbum,
                            Mood = existingMood,
                            DiscoTransactionId = discoTransaction.Id,
                        });
                        _db.SaveChanges();
                    }
                }

                // ------- Subgenres -------
                foreach (var subgenreDto in albumDto.Subgenres)
                {
                    var dbSubgenres = await _db.Subgenres.ToListAsync();
                    var existingSubgenre = dbSubgenres.FirstOrDefault(sg => StringUtils.NormalizeName(sg.Name) == StringUtils.NormalizeName(subgenreDto));

                    if (existingSubgenre == null)
                    {
                        existingSubgenre = new Subgenre
                        {
                            Name = StringUtils.CapitalizeAndFormat(subgenreDto),
                            GenreId = existingGenre.Id,
                            DiscoTransactionId = discoTransaction.Id,
                        };
                        _db.Subgenres.Add(existingSubgenre);
                        _db.SaveChanges();
                    }

                // ** Album Subgenre Junction Table **
                    if (!await _db.AlbumSubgenres.AnyAsync(asg => asg.SubgenreId == existingSubgenre.Id && asg.AlbumId == existingAlbum.Id))
                    {
                        _db.AlbumSubgenres.Add(new AlbumSubgenre
                        {
                            Album = existingAlbum,
                            Subgenre = existingSubgenre,
                            DiscoTransactionId = discoTransaction.Id,
                        });
                        _db.SaveChanges();
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
