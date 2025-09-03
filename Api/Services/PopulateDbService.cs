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

                    requestDetails.NewAlbumCount += 1;
                    existingAlbum = new Album
                    {
                        Title = StringUtils.CapitalizeAndFormat(albumDto.Title),
                        ReleaseDate = albumDto.ReleaseDate,
                        ReleaseDatePrecision = albumDto.ReleaseDatePrecision,
                        SortableDate = albumDto.SortableDate,
                        TotalTracks = albumDto.TotalTracks,
                        ImageUrl = albumDto.ImageUrl,
                        SpotifyId = albumDto.SpotifyId,
                        Description = StringUtils.CapitalizeSentences(albumDto.Description), 
                        Label = StringUtils.CapitalizeAndFormat(albumDto.Label),
                        SpotifyPopularity = albumDto.SpotifyPopularity,
                        IsOriginalRelease = albumDto.IsOriginalRelease,
                        DiscoTransactionId = discoTransaction.Id,
                        YoutubeId = streamingLinks.TryGetValue("YOUTUBE_PLAYLIST", out var youtubeId) ? youtubeId : null,
                        AppleMusicId = streamingLinks.TryGetValue("ITUNES_ALBUM", out var appleMusicId) ? appleMusicId : null,
                        AmazonMusicId = streamingLinks.TryGetValue("AMAZON_ALBUM", out var amazonMusicId) ? amazonMusicId : null,
                        PandoraId = streamingLinks.TryGetValue("PANDORA_ALBUM", out var soundCloudId) ? soundCloudId : null,
                    };

                    var additionalArtists = albumDto.Artists.Where(a => StringUtils.NormalizeName(a.Name) != StringUtils.NormalizeName(requestDetails.PrimaryArtistName)).Select(x => x.Name).ToList();
                    existingAlbum.AdditionalArtists = additionalArtists.Count > 0 ? existingAlbum.AdditionalArtists + String.Join(", ", additionalArtists) : "";

                    _db.Albums.Add(existingAlbum);
                    _db.SaveChanges();

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
                                EmotionalTone = moodDto.Valence,
                                EnergyLevel = moodDto.Arousal,
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
                                AlbumId = existingAlbum.Id,
                                MoodId = existingMood.Id,
                                DiscoTransactionId = discoTransaction.Id,
                            });
                            _db.SaveChanges();
                        }
                    }

                    // ------- Genres -------
                    foreach (var genreDto in albumDto.Genres)
                    {               
                        var matchingGenre = _db.GenreTypes.FirstOrDefault(gt => gt.Name == genreDto.Name);

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
                                    GenreTypeId = matchingGenre.Id,
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
                                    AlbumId = existingAlbum.Id,
                                    SubgenreId = existingSubgenre.Id,
                                    DiscoTransactionId = discoTransaction.Id,
                                });
                                _db.SaveChanges();
                            }
                        }

                        // ** Album Genre Junction Table **

                        if (!await _db.AlbumGenres.AnyAsync(ag => ag.AlbumId == existingAlbum.Id && ag.GenreTypeId == matchingGenre.Id))
                        {
                            var dbGenreTypes = _db.GenreTypes.ToList();

                            _db.AlbumGenres.Add(new AlbumGenre
                            {
                                AlbumId = existingAlbum.Id,
                                GenreTypeId = matchingGenre.Id,
                                DiscoTransactionId = discoTransaction.Id
                            });
                        }
                    }

                    // ** Album Genre Junction Table **
                    foreach (var jazzEraDto in albumDto.JazzEras)
                    {
                        var jazzEraTypeMatch = _db.JazzEraTypes.FirstOrDefault(jet => jet.Id == jazzEraDto);

                        if (jazzEraTypeMatch != null && !await _db.AlbumJazzEras.AnyAsync(aje => aje.AlbumId == existingAlbum.Id && aje.JazzEraTypeId == jazzEraTypeMatch.Id))
                        {
                            _db.AlbumJazzEras.Add(new AlbumJazzEra
                            {
                                AlbumId = existingAlbum.Id,
                                JazzEraTypeId = jazzEraTypeMatch.Id,
                            });
                        }
                    }
                }


                //  -------- Artist --------
                    
                var dbArtists = await _db.Artists.ToListAsync();
                var existingArtist = dbArtists.FirstOrDefault(a => StringUtils.NormalizeName(a.Name) == StringUtils.NormalizeName(requestDetails.PrimaryArtistName));
              
                if (existingArtist == null)
                {
                    requestDetails.NewArtistCount += 1;
                    var artistMatch = albumDto.Artists.FirstOrDefault(a => StringUtils.NormalizeName(a.Name) == StringUtils.NormalizeName(requestDetails.PrimaryArtistName));
                    var populatedArtist = await GetArtistDetailsAsync(artistMatch);
                    existingArtist = new Artist
                    {
                        Name = StringUtils.CapitalizeAndFormat(artistMatch.Name),
                        Biography = StringUtils.CapitalizeSentences(populatedArtist.Biography),
                        BirthYear = populatedArtist.BirthYear,
                        DeathYear = populatedArtist.DeathYear == "null" ? null : populatedArtist.DeathYear,
                        Instrument = StringUtils.CapitalizeAndFormat(populatedArtist.Instrument),
                        Genres = populatedArtist.Genres,
                        ImageUrl = populatedArtist.ImageUrl,
                        SpotifyPopularity = populatedArtist.SpotifyPopularity,
                        RelatedArtists = populatedArtist.RelatedArtists,
                        Influences = populatedArtist.Influences,
                        SpotifyId = artistMatch.SpotifyId,
                        DiscoTransactionId = discoTransaction.Id,
                    };
                    _db.Artists.Add(existingArtist);
                }

                _db.SaveChanges();

                // ** Album Artist Junction Table **
                if (!await _db.AlbumArtists.AnyAsync(aa => aa.ArtistId == existingArtist.Id && aa.AlbumId == existingAlbum.Id))
                {
                    _db.AlbumArtists.Add(new AlbumArtist
                    {
                        AlbumId = existingAlbum.Id,
                        ArtistId = existingArtist.Id,
                        DiscoTransactionId = discoTransaction.Id,
                    });
                    _db.SaveChanges();
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
