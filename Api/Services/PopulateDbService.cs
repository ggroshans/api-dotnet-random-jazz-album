using Microsoft.EntityFrameworkCore;
using RandomAlbumApi.Data;
using RandomAlbumApi.Models;

namespace Api.Services
{
    public class PopulateDbService
    {
        private readonly MusicDbContext _db;

        public PopulateDbService(MusicDbContext db)
        {
            _db = db;
        }

        public async Task SeedAlbumAsync(List<AlbumDto> albumDtos)
        {
            foreach (var albumDto in albumDtos)
            {
                // -------- Overall Album --------
                var album = new Album
                {
                    Name = albumDto.Name,
                    ReleaseYear = albumDto.ReleaseYear,
                    TotalTracks = albumDto.TotalTracks,
                    ImageUrl = albumDto.ImageUrl,
                    SpotifyId = albumDto.SpotifyId,
                    Description = albumDto.Description, 
                    PopularTracks = albumDto.PopularTracks,
                    AlbumTheme = albumDto.AlbumTheme,
                };

                _db.Albums.Add(album);

                //  -------- Artist --------
                foreach (var artistDto in albumDto.Artists)
                {
                    var existingArtist = await _db.Artists.FirstOrDefaultAsync(a => a.Name.ToLower() == artistDto.Name.ToLower()); 

                    if (existingArtist == null)
                    {
                        existingArtist = new Artist
                        {
                            Name = artistDto.Name,
                            Type = artistDto.Type,
                            SpotifyId = artistDto.SpotifyId
                        };
                        _db.Artists.Add(existingArtist);
                    }
                    // ** Album Artist Junction Table **
                    if (!await _db.AlbumArtists.AnyAsync(aa => aa.ArtistId == artistDto.Id && aa.AlbumId == album.Id))
                    {
                        _db.AlbumArtists.Add(new AlbumArtist
                        {
                            Album = album,
                            Artist = existingArtist,
                        });
                    }
                }
                // ------- Genre -------
                var existingGenre = await _db.Genres.FirstOrDefaultAsync(g => g.Name.ToLower() == albumDto.Genre.ToLower());
                if (existingGenre == null)
                {
                    existingGenre = new Genre
                    {
                        Name = albumDto.Genre,
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
                    });     
                }

                // ------- Mood -------
                foreach (var moodDto in albumDto.Moods)
                {
                    var existingMood = await _db.Moods.FirstOrDefaultAsync(m => m.Name.ToLower() == moodDto.ToLower());
                    
                    if (existingMood == null)
                    {
                        existingMood = new Mood
                        {
                            Name = moodDto,
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
                            Mood = existingMood
                        });
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
                            Name = subgenreDto,
                            GenreId = existingGenre.Id,
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
                        });
                    }
                }
            }
            await _db.SaveChangesAsync();
        }
    }
}
