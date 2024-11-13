using Microsoft.EntityFrameworkCore;
using RandomAlbumApi.Models;

namespace RandomAlbumApi.Data
{
    public class MusicDbContext : DbContext
    {
        
        public DbSet<Album> Albums { get; set; }
        public DbSet<Artist> Artists { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Subgenre> Subgenres { get; set; }
        public DbSet<Mood> Moods { get; set; }  

        // Junction tables for many-to-many
        public DbSet<AlbumArtist> AlbumArtists { get; set; }
        public DbSet<AlbumSubgenre> AlbumSubgenres { get; set; }
        public DbSet<AlbumMood> AlbumMoods { get; set; }

    public MusicDbContext (DbContextOptions<MusicDbContext> options) : base(options)
    {
    }    

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
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

            // Album + Genre (many to many)
            modelBuilder.Entity<AlbumGenre>()
                .HasKey(ag => new { ag.AlbumId, ag.GenreId });

            modelBuilder.Entity<AlbumGenre>()
                .HasOne(ag => ag.Album)
                .WithMany(ag => ag.AlbumGenres)
                .HasForeignKey(ag => ag.AlbumId);

            modelBuilder.Entity<AlbumGenre>()
                .HasOne(ag => ag.Genre)
                .WithMany(ag => ag.AlbumGenres)
                .HasForeignKey(ag => ag.GenreId);

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

            // Genre + Subgenre (one to many)

            modelBuilder.Entity<Subgenre>()
                .HasOne(s => s.Genre)
                .WithMany(g => g.Subgenres)
                .HasForeignKey(s => s.GenreId);
        }
    }    
}
