namespace DataModel
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity;
    using System.Data.Entity.ModelConfiguration.Conventions;
    using Entities;

    public class AudioDbContext : DbContext
    {
        public AudioDbContext() : base("name=audio-db")
        {
        }

        public static AudioDbContext Create()
        {
            return new AudioDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Properties<Guid>()
                .Where(x => x.Name == nameof(IEntity.Id))
                .Configure(
                    x =>
                    {
                        x.IsKey();
                        x.HasDatabaseGeneratedOption(
                            DatabaseGeneratedOption.None);
                    });

            modelBuilder.Entity<AudioItem>()
                .HasMany(x => x.Categories)
                .WithMany(x => x.AudioItems)
                .Map(x =>
                    x.ToTable("AudioCategories")
                        .MapLeftKey("AudioItemId")
                        .MapRightKey("CategoryId"));

            modelBuilder.Entity<AudioItem>()
                .HasMany(x => x.Artists)
                .WithMany(x => x.AudioItems)
                .Map(x =>
                    x.ToTable("AudioArtists")
                        .MapLeftKey("AudioItemId")
                        .MapRightKey("ArtistId"));

            modelBuilder.Entity<Artist>()
                .HasMany(x => x.Groups)
                .WithMany(x => x.Members)
                .Map(
                    x => x.ToTable("ArtistsArtistGroups")
                        .MapLeftKey("ArtistId")
                        .MapRightKey("ArtistGroupId"));
        }

        public DbSet<AudioItem> Audio { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Artist> Artists { get; set; }
        public DbSet<ArtistGroup> ArtistGroups { get; set; }
    }
}