using Microsoft.EntityFrameworkCore;
using LoopifyFinal.Models;

namespace LoopifyFinal.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Negocio> Negocios { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Subcategoria> Subcategorias { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<DetallePedido> DetallesPedidos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuración de relaciones
            modelBuilder.Entity<Negocio>()
                .HasOne(n => n.Usuario)
                .WithMany(u => u.Negocios)
                .HasForeignKey(n => n.UsuarioId);

            modelBuilder.Entity<Negocio>()
                .HasOne(n => n.Categoria)
                .WithMany()
                .HasForeignKey(n => n.CategoriaId);

            modelBuilder.Entity<Negocio>()
                .HasOne(n => n.Subcategoria1)
                .WithMany()
                .HasForeignKey(n => n.Subcategoria1Id)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Negocio>()
                .HasOne(n => n.Subcategoria2)
                .WithMany()
                .HasForeignKey(n => n.Subcategoria2Id)
                .OnDelete(DeleteBehavior.SetNull);

            base.OnModelCreating(modelBuilder);
        }
    }
}





// using Microsoft.EntityFrameworkCore;
// using LoopifyFinal.Models;

// namespace LoopifyFinal.Data
// {
//     public class LoopifyContext : DbContext
//     {
//         public LoopifyContext(DbContextOptions<LoopifyContext> options) : base(options) { }

//         public DbSet<Usuario> Usuarios { get; set; }
//         public DbSet<Negocio> Negocios { get; set; }
//         public DbSet<Producto> Productos { get; set; }
//         public DbSet<Categoria> Categorias { get; set; }
//         public DbSet<Subcategoria> Subcategorias { get; set; }
//         public DbSet<Pedido> Pedidos { get; set; }
//         public DbSet<DetallePedido> DetallesPedidos { get; set; }

//         protected override void OnModelCreating(ModelBuilder modelBuilder)
//         {
//             // Configurar los nombres de las tablas manualmente
//             modelBuilder.Entity<Usuario>().ToTable("Usuarios");
//             modelBuilder.Entity<Negocio>().ToTable("Negocios");
//             modelBuilder.Entity<Producto>().ToTable("Productos");
//             modelBuilder.Entity<Categoria>().ToTable("Categorias");
//             modelBuilder.Entity<Subcategoria>().ToTable("Subcategorias");
//             modelBuilder.Entity<Pedido>().ToTable("Pedidos");
//             modelBuilder.Entity<DetallePedido>().ToTable("DetallesPedidos");

//             // Configuración de la relación entre Negocio y Usuario
//             modelBuilder.Entity<Negocio>()
//                 .HasOne(n => n.Usuario)  // Un Negocio requiere un Usuario
//                 .WithMany()               // Un Usuario puede tener muchos Negocios (sin navegación explícita)
//                 .HasForeignKey(n => n.UsuarioId)
//                 .OnDelete(DeleteBehavior.Restrict);  // Desactiva la eliminación en cascada para evitar ciclos

//             base.OnModelCreating(modelBuilder);  // Llamar al método base (no es necesario en EF Core, pero es recomendable)
//         }
//     }
// }
