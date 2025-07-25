using Microsoft.EntityFrameworkCore;
using Api.Utilities;
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

        // Junction tables for many-to-many
        public DbSet<AlbumArtist> AlbumArtists { get; set; }
        public DbSet<AlbumSubgenre> AlbumSubgenres { get; set; }
        public DbSet<AlbumMood> AlbumMoods { get; set; }
        public DbSet<AlbumGenre> AlbumGenres { get; set; }
        public DbSet<AlbumJazzEra> AlbumJazzEras { get; set; }


        public MusicDbContext (DbContextOptions<MusicDbContext> options) : base(options)
    {
    }    

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

            // seeding initial data for GenreType and JazzEraType
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
                // Foundational
                new JazzEraType { Id = 1, Name = "Ragtime", StartYear = 1897, EndYear = 1918 },
                new JazzEraType { Id = 2, Name = "Early Jazz / New Orleans Jazz", StartYear = 1910, EndYear = 1929 },

                // The Swing Era & Its Variants
                new JazzEraType { Id = 3, Name = "Kansas City Jazz", StartYear = 1928, EndYear = 1942 },
                new JazzEraType { Id = 4, Name = "Swing Era / Big Band Era", StartYear = 1930, EndYear = 1945 },
                new JazzEraType { Id = 5, Name = "Gypsy Jazz (Jazz Manouche)", StartYear = 1934, EndYear = 1953 },

                // The Modern Jazz Revolution
                new JazzEraType { Id = 6, Name = "Bebop", StartYear = 1945, EndYear = 1955 },
                new JazzEraType { Id = 7, Name = "Cool Jazz", StartYear = 1949, EndYear = 1959 },
                new JazzEraType { Id = 8, Name = "Hard Bop", StartYear = 1954, EndYear = 1965 },
                new JazzEraType { Id = 9, Name = "Soul Jazz", StartYear = 1958, EndYear = 1969 },
                new JazzEraType { Id = 10, Name = "Modal Jazz", StartYear = 1958, EndYear = 1967 },

                // New Directions and Fusions
                new JazzEraType { Id = 11, Name = "Free Jazz / Avant-Garde", StartYear = 1960, EndYear = 1974 },
                new JazzEraType { Id = 12, Name = "Post-Bop", StartYear = 1964, EndYear = 1972 },
                new JazzEraType { Id = 13, Name = "Jazz Fusion", StartYear = 1969, EndYear = 1989 },
                new JazzEraType { Id = 14, Name = "Jazz-Funk", StartYear = 1972, EndYear = 1982 },

                // Contemporary and Parallel Streams
                new JazzEraType { Id = 15, Name = "Latin Jazz (Afro-Cuban & Brazilian)", StartYear = 1947, EndYear = null }, // Ongoing
                new JazzEraType { Id = 16, Name = "Neo-Bop", StartYear = 1980, EndYear = 1999 },
                new JazzEraType { Id = 17, Name = "Smooth Jazz", StartYear = 1982, EndYear = 1999 },
                new JazzEraType { Id = 18, Name = "Acid Jazz", StartYear = 1987, EndYear = 1997 },
                new JazzEraType { Id = 19, Name = "Contemporary Jazz / Modern Creative", StartYear = 2000, EndYear = null }
            );

            // configuring jsonb columns for Artist entity
            modelBuilder.Entity<Artist>(entity =>
            {
                entity.Property(a => a.SubgenreBreakdown)
                      .HasColumnType("jsonb"); 

                entity.Property(a => a.YearsActive)
                      .HasColumnType("jsonb");

                entity.Property(a => a.MoodBreakdown)
                      .HasColumnType("jsonb");
            });

            // formatting all table and column names to snake_case
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                // Set table name to snake_case
                entity.SetTableName(StringUtils.ConvertToSnakeCase(entity.GetTableName()));

                // Set column names to snake_case
                foreach (var property in entity.GetProperties())
                {
                    property.SetColumnName(StringUtils.ConvertToSnakeCase(property.Name));
                }

                // Handle keys and foreign keys
                foreach (var key in entity.GetKeys())
                {
                    key.SetName(StringUtils.ConvertToSnakeCase(key.GetName()));
                }

                foreach (var foreignKey in entity.GetForeignKeys())
                {
                    foreignKey.SetConstraintName(StringUtils.ConvertToSnakeCase(foreignKey.GetConstraintName()));
                }

                foreach (var index in entity.GetIndexes())
                {
                    index.SetDatabaseName(StringUtils.ConvertToSnakeCase(index.GetDatabaseName()));
                }
            }

            // mapping table names to lowercase
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


            // **CONFIGURING RELATIONSHIPS*

            // Album + Artist (many to many)
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

            // Album + Subgenre (many to many)
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

            // Album + Mood (many to many)
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

            // Album + GenreType (many to many)
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

            // Album + JazzEraType (many to many)
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

            // GenreType + Subgenre (one to many)
            modelBuilder.Entity<Subgenre>()
                .HasOne(s => s.GenreType)
                .WithMany(g => g.Subgenres)
                .HasForeignKey(s => s.GenreTypeId);


            // ***
            // DiscoTransaction Relationships
            // ***

            // Album + DiscoTransaction (one to many)
            modelBuilder.Entity<Album>()
                .HasOne(a => a.DiscoTransaction)
                .WithMany(dt => dt.Albums)
                .HasForeignKey(a => a.DiscoTransactionId);

            // Artist + DiscoTransaction (one to many)
            modelBuilder.Entity<Artist>()
                .HasOne(a => a.DiscoTransaction)
                .WithMany(dt => dt.Artists)
                .HasForeignKey(a => a.DiscoTransactionId);

            // Mood + DiscoTransaction (one to many)
            modelBuilder.Entity<Mood>()
                .HasOne(m => m.DiscoTransaction)
                .WithMany(dt => dt.Moods)
                .HasForeignKey(a => a.DiscoTransactionId);

            // Subgenre + DiscoTransaction (one to many)
            modelBuilder.Entity<Subgenre>()
                .HasOne(s => s.DiscoTransaction)
                .WithMany(dt => dt.Subgenres)
                .HasForeignKey(a => a.DiscoTransactionId);

            // AlbumArtist + DiscoTransaction (one to many)
            modelBuilder.Entity<AlbumArtist>()
                .HasOne(aa => aa.DiscoTransaction)
                .WithMany(dt => dt.AlbumArtists)
                .HasForeignKey(aa => aa.DiscoTransactionId);

            // AlbumMood + DiscoTransaction (one to many)
            modelBuilder.Entity<AlbumMood>()
                .HasOne(am => am.DiscoTransaction)
                .WithMany(dt => dt.AlbumMoods)
                .HasForeignKey(am => am.DiscoTransactionId);

            // AlbumSubgenre + DiscoTransaction (one to many)
            modelBuilder.Entity<AlbumSubgenre>()
                .HasOne(asg => asg.DiscoTransaction)
                .WithMany(dt => dt.AlbumSubgenres)
                .HasForeignKey(asg => asg.DiscoTransactionId);
        }
    }    
}
