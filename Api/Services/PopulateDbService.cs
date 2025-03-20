using Microsoft.EntityFrameworkCore;
using Api.Data;
using Api.Services.ApiServices;
using Api.Services.ApiServices.Spotify;
using Api.Domain.Entities;
using Api.Utilities;
using Newtonsoft.Json;
using Api.Models;
using Api.Models.DTOs.InternalDTOs;

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

        public async Task PopulateAlbumAsync(DiscoTransaction discoTransaction, List<AlbumProcessingDto> albumDtos)
        {

            RequestDetails requestDetails = JsonConvert.DeserializeObject<RequestDetails>(discoTransaction.RequestDetails) ?? new RequestDetails();
            

            foreach (var albumDto in albumDtos)
            {
                
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


                // -------- Overall Album --------
                var dbAlbums = await _db.Albums.ToListAsync();
                var existingAlbum = dbAlbums.FirstOrDefault(a => StringUtils.NormalizeName(a.Title) == StringUtils.NormalizeName(albumDto.Title));

                if (existingAlbum == null)
                {
                    requestDetails.AlbumCount += 1;
                    existingAlbum = new Album
                    {
                        Title = StringUtils.CapitalizeAndFormat(albumDto.Title),
                        ReleaseYear = albumDto.ReleaseYear,
                        TotalTracks = albumDto.TotalTracks,
                        ImageUrl = albumDto.ImageUrl,
                        SpotifyId = albumDto.SpotifyId,
                        Description = StringUtils.CapitalizeSentences(albumDto.Description), 
                        PopularTracks = albumDto.PopularTracks,
                        AlbumTheme = StringUtils.CapitalizeSentences(albumDto.AlbumTheme),
                        Label = StringUtils.CapitalizeAndFormat(albumDto.Label),
                        PopularityScore = albumDto.PopularityScore,
                        GenreId = existingGenre.Id,
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
                        requestDetails.ArtistCount += 1;
                        var populatedArtist = await GetArtistDetailsAsync(artistDto);
                        existingArtist = new Artist
                        {
                            Name = StringUtils.CapitalizeAndFormat(artistDto.Name),
                            Biography = StringUtils.CapitalizeSentences(populatedArtist.Biography),
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


                // ------- Mood -------
                foreach (var moodDto in albumDto.Moods)
                {
                    var dbMoods = await _db.Moods.ToListAsync();
                    var existingMood = dbMoods.FirstOrDefault(m => StringUtils.NormalizeName(m.Name) == StringUtils.NormalizeName(moodDto));
                    
                    if (existingMood == null)
                    {
                        requestDetails.MoodCount += 1;
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
                        requestDetails.SubgenreCount += 1;
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

            var existingDiscoTransaction = _db.DiscoTransactions.Find(discoTransaction.Id);

            if (existingDiscoTransaction != null)
            {
                existingDiscoTransaction.RequestDetails = JsonConvert.SerializeObject(requestDetails);
                _db.SaveChanges();
            }
        }

        public async Task<ArtistProcessingDto> GetArtistDetailsAsync(ArtistProcessingDto artist)
        {
            var spotifyArtist = await _spotifyApiService.GetSpotifyArtist(artist);
            var gptArtist = await _gptApiService.GetGptArtistDetails(spotifyArtist);
            return gptArtist;
        }
    }
}
