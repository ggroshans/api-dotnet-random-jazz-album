using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Api.Domain;
using Api.Domain.Entities;

namespace Api.Data
{
    public class MusicDbContext : DbContext
    {
        public DbSet<Album> Albums { get; set; }
        public DbSet<Artist> Artists { get; set; }
        public DbSet<GenreType> GenreTypes { get; set; }
        public DbSet<Subgenre> Subgenres { get; set; }
        public DbSet<Mood> Moods { get; set; }
        public DbSet<JazzEraType> JazzEraTypes { get; set; }
        public DbSet<DiscoTransaction> DiscoTransactions { get; set; }
        public DbSet<AlbumArtist> AlbumArtists { get; set; }
        public DbSet<AlbumSubgenre> AlbumSubgenres { get; set; }
        public DbSet<AlbumMood> AlbumMoods { get; set; }
        public DbSet<AlbumGenre> AlbumGenres { get; set; }
        public DbSet<AlbumJazzEra> AlbumJazzEras { get; set; }

        public MusicDbContext(DbContextOptions<MusicDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GenreType>().HasData(
                new GenreType { Id = 1, Name = "Classical" },
                new GenreType { Id = 2, Name = "Jazz" },
                new GenreType { Id = 3, Name = "Blues" },
                new GenreType { Id = 4, Name = "Funk"},
                new GenreType { Id = 5, Name = "Rock" },
                new GenreType { Id = 6, Name = "Pop" },
                new GenreType { Id = 7, Name = "Country" },
                new GenreType { Id = 8, Name = "Folk" },
                new GenreType { Id = 9, Name = "Hip-Hop/Rap" },
                new GenreType { Id = 10, Name = "Electronic" },
                new GenreType { Id = 11, Name = "Reggae" },
                new GenreType { Id = 12, Name = "R&B/Soul" },
                new GenreType { Id = 13, Name = "Metal" },
                new GenreType { Id = 14, Name = "World Music" },
                new GenreType { Id = 15, Name = "Gospel" },
                new GenreType { Id = 16, Name = "Latin" }
            );

            modelBuilder.Entity<JazzEraType>().HasData(
                new JazzEraType {
                    Id = 1, Name = "Ragtime", StartYear = 1897, EndYear = 1918,
                    Description = "Ragtime was a syncopated piano-based style that blended African American rhythms with European classical traditions. Popularized by Scott Joplin, it laid the rhythmic foundation for early jazz and dance music of the early 20th century."
                },
                new JazzEraType {
                    Id = 2, Name = "Early Jazz / New Orleans Jazz", StartYear = 1910, EndYear = 1929,
                    Description = "Emerging in New Orleans, this style fused blues, brass band marches, and ragtime. Characterized by collective improvisation and polyphony, it spread through musicians like Louis Armstrong and Jelly Roll Morton, shaping the first true jazz idiom."
                },
                new JazzEraType {
                    Id = 3, Name = "Kansas City Jazz", StartYear = 1928, EndYear = 1942,
                    Description = "Known for riff-based arrangements, blues roots, and an emphasis on improvisation, Kansas City Jazz fostered a looser swing feel and extended jam sessions. Count Basie’s orchestra became the emblem of this vibrant, nightlife-driven regional style."
                },
                new JazzEraType {
                    Id = 4, Name = "Swing Era / Big Band Era", StartYear = 1930, EndYear = 1945,
                    Description = "The Swing Era brought jazz into mainstream popularity with large ensembles, danceable rhythms, and sophisticated arrangements. Leaders like Duke Ellington, Count Basie, and Benny Goodman defined this golden age of jazz and American dance culture."
                },
                new JazzEraType {
                    Id = 5, Name = "Gypsy Jazz (Jazz Manouche)", StartYear = 1934, EndYear = 1953,
                    Description = "Pioneered by guitarist Django Reinhardt and violinist Stéphane Grappelli, Gypsy Jazz combined swing rhythms with Romani folk traditions. Driven by guitar and violin, it created a distinct European jazz sound still celebrated in manouche circles today."
                },
                new JazzEraType {
                    Id = 6, Name = "Bebop", StartYear = 1945, EndYear = 1955,
                    Description = "Bebop shifted jazz from dance halls to art music, emphasizing speed, harmonic complexity, and virtuosic improvisation. Innovators like Charlie Parker and Dizzy Gillespie forged small-combo jazz that challenged listeners and redefined modern musicianship."
                },
                new JazzEraType {
                    Id = 7, Name = "Cool Jazz", StartYear = 1949, EndYear = 1959,
                    Description = "Cool Jazz contrasted bebop with a more subdued, lyrical sound, incorporating classical influences and rich textures. Figures like Miles Davis, Dave Brubeck, and Gerry Mulligan popularized its relaxed mood, particularly on the U.S. West Coast."
                },
                new JazzEraType {
                    Id = 8, Name = "Hard Bop", StartYear = 1954, EndYear = 1965,
                    Description = "Hard Bop revitalized jazz by infusing blues, gospel, and R&B elements into the bebop tradition. With powerful rhythms, soulful phrasing, and a return to roots, musicians like Art Blakey and Horace Silver created a deeply expressive and urban sound."
                },
                new JazzEraType {
                    Id = 9, Name = "Soul Jazz", StartYear = 1958, EndYear = 1969,
                    Description = "Emerging from hard bop, Soul Jazz highlighted groove, bluesy melodies, and gospel-inspired harmonies. Often driven by the Hammond organ, it fused jazz improvisation with R&B and funk rhythms, making the style accessible to broader popular audiences."
                },
                new JazzEraType {
                    Id = 10, Name = "Modal Jazz", StartYear = 1958, EndYear = 1967,
                    Description = "Modal Jazz broke from chord-heavy bebop by using musical modes as the harmonic basis, enabling freer improvisation. Pioneered by Miles Davis on 'Kind of Blue' and expanded by John Coltrane, this era profoundly influenced modern improvisational practice."
                },
                new JazzEraType {
                    Id = 11, Name = "Free Jazz / Avant-Garde", StartYear = 1960, EndYear = 1974,
                    Description = "Free Jazz rejected fixed forms and harmony, focusing on collective improvisation, extended techniques, and raw expression. Artists like Ornette Coleman, Cecil Taylor, and John Coltrane expanded jazz into avant-garde territory, reshaping its boundaries."
                },
                new JazzEraType {
                    Id = 12, Name = "Post-Bop", StartYear = 1964, EndYear = 1972,
                    Description = "Post-Bop synthesized elements of hard bop, modal jazz, and avant-garde experimentation. Characterized by harmonic freedom and sophisticated composition, it was championed by artists like Wayne Shorter, Herbie Hancock, and Miles Davis’s mid-60s quintet."
                },
                new JazzEraType {
                    Id = 13, Name = "Jazz Fusion", StartYear = 1969, EndYear = 1989,
                    Description = "Jazz Fusion merged improvisation with rock, funk, and electronic instruments. Bands like Weather Report and Mahavishnu Orchestra blended amplified sounds and global influences, creating one of jazz’s most commercially and stylistically diverse movements."
                },
                new JazzEraType {
                    Id = 14, Name = "Jazz-Funk", StartYear = 1972, EndYear = 1982,
                    Description = "Jazz-Funk emphasized syncopated bass lines, groove-heavy rhythms, and electric instrumentation. Combining jazz harmonies with funk danceability, it flourished in the 1970s through artists like Herbie Hancock and Roy Ayers, shaping later popular music."
                },
                new JazzEraType {
                    Id = 15, Name = "Latin Jazz (Afro-Cuban & Brazilian)", StartYear = 1947, EndYear = null,
                    Description = "Latin Jazz integrates jazz improvisation with Afro-Cuban rhythms like clave and Brazilian styles such as bossa nova. Popularized by Dizzy Gillespie, Tito Puente, and Antonio Carlos Jobim, it remains one of jazz’s most enduring and global traditions."
                },
                new JazzEraType {
                    Id = 16, Name = "Neo-Bop", StartYear = 1980, EndYear = 1999,
                    Description = "Neo-Bop revived the acoustic, hard bop sound with modern sensibilities during the 1980s and 90s. Spearheaded by Wynton Marsalis and the 'Young Lions,' it reaffirmed traditional improvisation and swing while resisting the era’s more commercial trends."
                },
                new JazzEraType {
                    Id = 17, Name = "Smooth Jazz", StartYear = 1982, EndYear = 1999,
                    Description = "Smooth Jazz blended jazz improvisation with pop and R&B sensibilities, producing a polished, radio-friendly sound. Artists like Kenny G, George Benson, and David Sanborn popularized the genre, making it commercially successful but critically divisive."
                },
                new JazzEraType {
                    Id = 18, Name = "Acid Jazz", StartYear = 1987, EndYear = 1997,
                    Description = "Acid Jazz fused jazz improvisation with funk, soul, hip-hop, and electronic dance grooves. Emerging from UK club culture, it produced artists like Jamiroquai and Brand New Heavies, influencing both the jazz revival and 1990s popular music landscapes."
                },
                new JazzEraType {
                    Id = 19, Name = "Contemporary Jazz / Modern Creative", StartYear = 2000, EndYear = null,
                    Description = "Contemporary Jazz reflects today’s global, cross-genre landscape, blending acoustic traditions with electronics, world music, and experimental approaches. It spans mainstream and avant-garde, highlighting jazz’s ongoing innovation in the 21st century."
                }
            );

            var jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);

            modelBuilder.Entity<Artist>(entity =>
            {
                entity.Property(a => a.SubgenreBreakdown)
                      .HasConversion(
                          v => JsonSerializer.Serialize(v, jsonOptions),
                          v => string.IsNullOrWhiteSpace(v) ? new SubgenreBreakdown() : JsonSerializer.Deserialize<SubgenreBreakdown>(v, jsonOptions)!)
                      .HasColumnType("TEXT");

                entity.Property(a => a.YearsActive)
                      .HasConversion(
                          v => JsonSerializer.Serialize(v, jsonOptions),
                          v => string.IsNullOrWhiteSpace(v) ? new YearsActive() : JsonSerializer.Deserialize<YearsActive>(v, jsonOptions)!)
                      .HasColumnType("TEXT");

                entity.Property(a => a.MoodBreakdown)
                      .HasConversion(
                          v => JsonSerializer.Serialize(v, jsonOptions),
                          v => string.IsNullOrWhiteSpace(v) ? new MoodBreakdown() : JsonSerializer.Deserialize<MoodBreakdown>(v, jsonOptions)!)
                      .HasColumnType("TEXT");
            });

            modelBuilder.Entity<Album>().ToTable("albums");
            modelBuilder.Entity<Artist>().ToTable("artists");
            modelBuilder.Entity<GenreType>().ToTable("genre_types");
            modelBuilder.Entity<Subgenre>().ToTable("subgenres");
            modelBuilder.Entity<Mood>().ToTable("moods");
            modelBuilder.Entity<JazzEraType>().ToTable("jazz_era_types");
            modelBuilder.Entity<DiscoTransaction>().ToTable("disco_transactions");
            modelBuilder.Entity<AlbumArtist>().ToTable("album_artists");
            modelBuilder.Entity<AlbumSubgenre>().ToTable("album_subgenres");
            modelBuilder.Entity<AlbumMood>().ToTable("album_moods");
            modelBuilder.Entity<AlbumGenre>().ToTable("album_genres");
            modelBuilder.Entity<AlbumJazzEra>().ToTable("album_jazz_eras");

            modelBuilder.Entity<AlbumArtist>()
                .HasKey(aa => new { aa.AlbumId, aa.ArtistId });
            modelBuilder.Entity<AlbumArtist>()
                .HasOne(aa => aa.Album)
                .WithMany(a => a.AlbumArtists)
                .HasForeignKey(aa => aa.AlbumId);
            modelBuilder.Entity<AlbumArtist>()
                .HasOne(aa => aa.Artist)
                .WithMany(a => a.AlbumArtists)
                .HasForeignKey(aa => aa.ArtistId);

            modelBuilder.Entity<AlbumSubgenre>()
                .HasKey(aa => new { aa.AlbumId, aa.SubgenreId });
            modelBuilder.Entity<AlbumSubgenre>()
                .HasOne(asg => asg.Album)
                .WithMany(a => a.AlbumSubgenres)
                .HasForeignKey(asg => asg.AlbumId);
            modelBuilder.Entity<AlbumSubgenre>()
                .HasOne(asg => asg.Subgenre)
                .WithMany(s => s.AlbumSubgenres)
                .HasForeignKey(asg => asg.SubgenreId);

            modelBuilder.Entity<AlbumMood>()
                .HasKey(am => new { am.AlbumId, am.MoodId });
            modelBuilder.Entity<AlbumMood>()
                .HasOne(am => am.Album)
                .WithMany(a => a.AlbumMoods)
                .HasForeignKey(am => am.AlbumId);
            modelBuilder.Entity<AlbumMood>()
                .HasOne(am => am.Mood)
                .WithMany(m => m.AlbumMoods)
                .HasForeignKey(am => am.MoodId);

            modelBuilder.Entity<AlbumGenre>()
                .HasKey(ag => new { ag.AlbumId, ag.GenreTypeId });
            modelBuilder.Entity<AlbumGenre>()
                .HasOne(ag => ag.GenreType)
                .WithMany(g => g.AlbumGenres)
                .HasForeignKey(a => a.GenreTypeId);
            modelBuilder.Entity<AlbumGenre>()
                .HasOne(ag => ag.Album)
                .WithMany(a => a.AlbumGenres)
                .HasForeignKey(a => a.AlbumId);

            modelBuilder.Entity<AlbumJazzEra>()
                .HasKey(aje => new { aje.AlbumId, aje.JazzEraTypeId });
            modelBuilder.Entity<AlbumJazzEra>()
                .HasOne(aje => aje.Album)
                .WithMany(a => a.AlbumJazzEras)
                .HasForeignKey(aje => aje.AlbumId);
            modelBuilder.Entity<AlbumJazzEra>()
                .HasOne(aje => aje.JazzEraType)
                .WithMany(j => j.AlbumJazzEras)
                .HasForeignKey(aje => aje.JazzEraTypeId);

            modelBuilder.Entity<Subgenre>()
                .HasOne(s => s.GenreType)
                .WithMany(g => g.Subgenres)
                .HasForeignKey(s => s.GenreTypeId);

            modelBuilder.Entity<Album>()
                .HasOne(a => a.DiscoTransaction)
                .WithMany(dt => dt.Albums)
                .HasForeignKey(a => a.DiscoTransactionId);
            modelBuilder.Entity<Artist>()
                .HasOne(a => a.DiscoTransaction)
                .WithMany(dt => dt.Artists)
                .HasForeignKey(a => a.DiscoTransactionId);
            modelBuilder.Entity<Mood>()
                .HasOne(m => m.DiscoTransaction)
                .WithMany(dt => dt.Moods)
                .HasForeignKey(a => a.DiscoTransactionId);
            modelBuilder.Entity<Subgenre>()
                .HasOne(s => s.DiscoTransaction)
                .WithMany(dt => dt.Subgenres)
                .HasForeignKey(a => a.DiscoTransactionId);
            modelBuilder.Entity<AlbumArtist>()
                .HasOne(aa => aa.DiscoTransaction)
                .WithMany(dt => dt.AlbumArtists)
                .HasForeignKey(aa => aa.DiscoTransactionId);
            modelBuilder.Entity<AlbumMood>()
                .HasOne(am => am.DiscoTransaction)
                .WithMany(dt => dt.AlbumMoods)
                .HasForeignKey(am => am.DiscoTransactionId);
            modelBuilder.Entity<AlbumSubgenre>()
                .HasOne(asg => asg.DiscoTransaction)
                .WithMany(dt => dt.AlbumSubgenres)
                .HasForeignKey(asg => asg.DiscoTransactionId);
        }
    }
}
