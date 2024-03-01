using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using web_hoteldemo.Services;

namespace web_hoteldemo.Models.DB
{
    public partial class db_adminHotelContext : DbContext
    {
        public db_adminHotelContext()
        {
        }

        public db_adminHotelContext(DbContextOptions<db_adminHotelContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Usuario> Usuarios { get; set; } = null!;
        public virtual DbSet<Usuario> Usuario { get; set; } = null!;
        public virtual DbSet<Bitacora> Bitacora { get; set; } = null!;






        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {

            }
        }

        public List<Bitacora> InsertarBitacora(int usuarioID, DateTime fecha, TimeSpan horaEntrada,TimeSpan horaSalida)
        {
            var usuarioIDParameter = new SqlParameter("@usuarioID", usuarioID);
            var fechaParameter = new SqlParameter("@fecha", fecha);
            var horaEntradaParameter = new SqlParameter("@horaEntrada", horaEntrada);
            var horaSalidaParameter = new SqlParameter("@horaSalida", horaSalida);

            return this.Bitacora.FromSqlRaw("EXEC InsertarBitacora @usuarioID, @fecha, @horaEntrada","@horaSalida", usuarioIDParameter, fechaParameter, horaEntradaParameter, horaSalidaParameter).ToList();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("usuario");

                entity.Property(e => e.UsuarioId)
                    .ValueGeneratedNever()
                    .HasColumnName("usuarioID");

                entity.Property(e => e.Contraseña)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("contraseña");

                entity.Property(e => e.Estado).HasColumnName("estado");

                entity.Property(e => e.NombreUsuario)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("nombreUsuario");

                entity.Property(e => e.Rol)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("rol");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);



    }
}
