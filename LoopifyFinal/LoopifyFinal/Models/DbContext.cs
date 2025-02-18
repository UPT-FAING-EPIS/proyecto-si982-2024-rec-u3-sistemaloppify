using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace LoopifyFinal.Models
{
    public class LoopifyContext : DbContext
    {
        public LoopifyContext() : base("LoopifyConnectionString") { }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Negocio> Negocios { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Subcategoria> Subcategorias { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<DetallePedido> DetallesPedidos { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Configurar los nombres de las tablas manualmente
            modelBuilder.Entity<Usuario>().ToTable("Usuarios");
            modelBuilder.Entity<Negocio>().ToTable("Negocios");
            modelBuilder.Entity<Producto>().ToTable("Productos");
            modelBuilder.Entity<Categoria>().ToTable("Categorias");
            modelBuilder.Entity<Subcategoria>().ToTable("Subcategorias");
            modelBuilder.Entity<Pedido>().ToTable("Pedidos");
            modelBuilder.Entity<DetallePedido>().ToTable("DetallesPedidos");

            // Configuración de la relación entre Negocio y Usuario
            modelBuilder.Entity<Negocio>()
                .HasRequired(n => n.Usuario)  // Un Negocio requiere un Usuario
                .WithMany()                   // Un Usuario puede tener muchos Negocios (sin navegación explícita)
                .HasForeignKey(n => n.UsuarioId)
                .WillCascadeOnDelete(false);  // Desactiva la eliminación en cascada para evitar ciclos

            base.OnModelCreating(modelBuilder); // Llamar al método base
        }
    }
}








//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Data.Entity;

//namespace LoopifyFinal.Models
//{
//    public class LoopifyContext : DbContext
//    {
//        public LoopifyContext() : base("LoopifyConnectionString") { }

//        public DbSet<Usuario> Usuarios { get; set; }
//        public DbSet<Negocio> Negocios { get; set; }
//        public DbSet<Producto> Productos { get; set; }
//        public DbSet<Categoria> Categorias { get; set; } // Agregar
//        public DbSet<Subcategoria> Subcategorias { get; set; }
//        public DbSet<Pedido> Pedidos { get; set; }
//        public DbSet<DetallePedido> DetallesPedidos { get; set; }

//        protected override void OnModelCreating(DbModelBuilder modelBuilder)
//        {
//            // Configurar los nombres de las tablas manualmente
//            modelBuilder.Entity<Usuario>().ToTable("Usuarios");
//            modelBuilder.Entity<Negocio>().ToTable("Negocios");
//            modelBuilder.Entity<Producto>().ToTable("Productos");
//            modelBuilder.Entity<Categoria>().ToTable("Categorias");
//            modelBuilder.Entity<Subcategoria>().ToTable("Subcategorias");
//            modelBuilder.Entity<Pedido>().ToTable("Pedidos");
//            modelBuilder.Entity<DetallePedido>().ToTable("DetallesPedidos");

//            base.OnModelCreating(modelBuilder); // Llamar al método base
//        }
//    }
//}
