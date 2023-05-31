using Microsoft.EntityFrameworkCore;

namespace BikeappAPI.Models
{
    public partial class BikeappContext : DbContext
    {
        public BikeappContext()
        {
        }

        public BikeappContext(DbContextOptions<BikeappContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Journey> Journey { get; set; }
        public virtual DbSet<Station> Station { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Journey>(entity =>
            {
                entity.Property(e => e.JourneyId)
                    .ValueGeneratedNever()
                    .HasColumnName("JourneyID");

                entity.Property(e => e.DepartureDate).HasColumnType("datetime");

                entity.Property(e => e.DepartureStationId).HasColumnName("DepartureStationID");

                entity.Property(e => e.DepartureStationName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ReturnDate).HasColumnType("datetime");

                entity.Property(e => e.ReturnStationId).HasColumnName("ReturnStationID");

                entity.Property(e => e.ReturnStationName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Station>(entity =>
            {
                entity.Property(e => e.FId)
                   .ValueGeneratedNever()
                   .HasColumnName("FId");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");

                entity.Property(e => e.Adress)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Kaupunki)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Namn)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Nimi)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Operaattor)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Osoite)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Stad)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}