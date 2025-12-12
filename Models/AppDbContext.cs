using Microsoft.EntityFrameworkCore;


namespace TravelFriend

{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Amigo> Amigos { get; set; }
        public DbSet<Viaje> Viajes { get; set; }
        public DbSet<ParticipanteViaje> ParticipanteViajes { get; set; }
        public DbSet<Gasto> Gastos { get; set; }
        public DbSet<GastoDetalle> GastosDetalles { get; set; }
        public DbSet<Notificacion> Notificaciones { get; set; }
        public DbSet<Liquidacion> Liquidaciones { get; set; }


        //Esto configura las relaciones y restricciones entre las entidades, eliminando cascadas ya que daba error al actualizar la base de datos.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Amigo>()
                .HasOne(a => a.Usuario)        // quien envía la amistad
                .WithMany(u => u.Amigos)
                .HasForeignKey(a => a.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Amigo>()
                .HasOne(a => a.Amiguito)       // quien recibe la amistad
                .WithMany()
                .HasForeignKey(a => a.AmigoId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Liquidacion>()
                .HasOne(l => l.Deudor)          // Usuario que debe
                .WithMany(u => u.LiquidacionesDeudor)
                .HasForeignKey(l => l.DeudorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Liquidacion>()
                .HasOne(l => l.Acreedor)        // Usuario que recibe
                .WithMany(u => u.LiquidacionesAcreedor)
                .HasForeignKey(l => l.AcreedorId)
                .OnDelete(DeleteBehavior.Restrict);


            // GASTOS
            modelBuilder.Entity<Gasto>()
                .HasOne(g => g.Viaje)
                .WithMany(v => v.Gastos)
                .HasForeignKey(g => g.ViajeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Gasto>()
                .HasOne(g => g.Usuario)
                .WithMany(u => u.Gastos)
                .HasForeignKey(g => g.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<ParticipanteViaje>()
                .HasOne(p => p.Usuario)
                .WithMany(u => u.Participaciones)
                .HasForeignKey(p => p.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ParticipanteViaje>()
                .HasOne(p => p.Viaje)
                .WithMany(v => v.Participantes)
                .HasForeignKey(p => p.ViajeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }


}
