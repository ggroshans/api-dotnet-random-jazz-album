using Microsoft.EntityFrameworkCore;
using Api.Data;
using Api.Services.ApiServices;
using Api.Services.ApiServices.Spotify;
using Api.Domain.Entities;
using Api.Utilities;
using Newtonsoft.Json;
using Api.Models.DTOs.InternalDTOs;

namespace Api.Services
{
    public class PopulateDbService
    {
        private readonly MusicDbContext _db;
        private readonly SpotifyApiService _spotifyApiService;
        private readonly GptApiService _gptApiService;
        private readonly StreamingLinksService _streamingLinksService;

        public PopulateDbService(MusicDbContext db, SpotifyApiService spotifyApiService, GptApiService gptApiService, StreamingLinksService streamingLinksService )
        {
            _db = db;
            _spotifyApiService = spotifyApiService;
            _gptApiService = gptApiService;
            _streamingLinksService = streamingLinksService;
        }

        public async Task PopulateAlbumAsync(DiscoTransaction discoTransaction, List<AlbumProcessingDto> albumDtos)
        {

            RequestDetails requestDetails = JsonConvert.DeserializeObject<RequestDetails>(discoTransaction.RequestDetails) ?? new RequestDetails();
            

            foreach (var albumDto in albumDtos)
            {
               
                // -------- Overall Album --------
                var dbAlbums = await _db.Albums.ToListAsync();
                var existingAlbum = dbAlbums.FirstOrDefault(a => StringUtils.NormalizeName(a.Title) == StringUtils.NormalizeName(albumDto.Title));

                if (existingAlbum == null)
                {

                    var streamingLinks = await _streamingLinksService.GetLinks(albumDto.SpotifyId);
                    //var existingJazzEraType = await _db.JazzEraTypes.FirstOrDefaultAsync(j => (j.Name == albumDto.JazzEra.Name) && (j.Id == albumDto.JazzEra.Id));

                    requestDetails.NewAlbumCount += 1;
                    existingAlbum = new Album
                    {
                        Title = StringUtils.CapitalizeAndFormat(albumDto.Title),
                        ReleaseYear = albumDto.ReleaseYear,
                        TotalTracks = albumDto.TotalTracks,
                        ImageUrl = albumDto.ImageUrl,
                        SpotifyId = albumDto.SpotifyId,
                        Description = StringUtils.CapitalizeSentences(albumDto.Description), 
                        AlbumTheme = StringUtils.CapitalizeSentences(albumDto.AlbumTheme),
                        Label = StringUtils.CapitalizeAndFormat(albumDto.Label),
                        PopularityScore = albumDto.PopularityScore,
                        JazzEraTypeId = null,
                        DiscoTransactionId = discoTransaction.Id,
                        YoutubeId = streamingLinks.TryGetValue("YOUTUBE_PLAYLIST", out var youtubeId) ? youtubeId : null,
                        AppleMusicId = streamingLinks.TryGetValue("ITUNES_ALBUM", out var appleMusicId) ? appleMusicId : null,
                        AmazonMusicId = streamingLinks.TryGetValue("AMAZON_ALBUM", out var amazonMusicId) ? amazonMusicId : null,
                        PandoraId = streamingLinks.TryGetValue("PANDORA_ALBUM", out var soundCloudId) ? soundCloudId : null,
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
                        requestDetails.NewArtistCount += 1;
                        var populatedArtist = await GetArtistDetailsAsync(artistDto);
                        existingArtist = new Artist
                        {
                            Name = StringUtils.CapitalizeAndFormat(artistDto.Name),
                            Biography = StringUtils.CapitalizeSentences(populatedArtist.Biography),
                            Instrument = StringUtils.CapitalizeAndFormat(populatedArtist.Instrument),
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
                    var existingMood = dbMoods.FirstOrDefault(m => StringUtils.NormalizeName(m.Name) == StringUtils.NormalizeName(moodDto.Name));
                    
                    if (existingMood == null)
                    {
                        requestDetails.NewMoodCount += 1;
                        existingMood = new Mood
                        {
                            Name = StringUtils.CapitalizeAndFormat(moodDto.Name),
                            EmotionScore = moodDto.Valence,
                            EnergyScore = moodDto.Arousal,
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

                // ------- Genres -------
                foreach (var genreDto in albumDto.Genres)
                {
                    // ------- Subgenres -------
                    foreach (var subgenreDto in genreDto.Subgenres)
                    {
                        var dbSubgenres = await _db.Subgenres.ToListAsync();
                        var existingSubgenre = dbSubgenres.FirstOrDefault(sg => StringUtils.NormalizeName(sg.Name) == StringUtils.NormalizeName(subgenreDto));

                        if (existingSubgenre == null)
                        {
                            requestDetails.NewSubgenreCount += 1;
                            existingSubgenre = new Subgenre
                            {
                                Name = StringUtils.CapitalizeAndFormat(subgenreDto),
                                GenreTypeId = genreDto.Id, 
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

                    //**Album Genre Junction Table * *
                    if (!await _db.AlbumGenres.AnyAsync(ag => (ag.GenreTypeId == genreDto.Id && ag.AlbumId == existingAlbum.Id)))
                    {
                        var dbGenreTypes = _db.GenreTypes.ToList();
                        var existingGenreType = dbGenreTypes.FirstOrDefault(gt => gt.Id == genreDto.Id);

                        _db.AlbumGenres.Add(new AlbumGenre
                        {
                            Album = existingAlbum,
                            GenreType = existingGenreType,
                            DiscoTransactionId = discoTransaction.Id
                        });
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
