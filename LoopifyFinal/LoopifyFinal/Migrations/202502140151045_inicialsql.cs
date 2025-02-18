namespace LoopifyFinal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class inicialsql : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Categorias",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nombre = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DetallesPedidos",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PedidoId = c.Int(nullable: false),
                        ProductoId = c.Int(nullable: false),
                        Cantidad = c.Int(nullable: false),
                        Precio = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Pedidos", t => t.PedidoId, cascadeDelete: true)
                .ForeignKey("dbo.Productos", t => t.ProductoId, cascadeDelete: true)
                .Index(t => t.PedidoId)
                .Index(t => t.ProductoId);
            
            CreateTable(
                "dbo.Pedidos",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Fecha = c.DateTime(nullable: false),
                        UsuarioId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Usuarios", t => t.UsuarioId, cascadeDelete: true)
                .Index(t => t.UsuarioId);
            
            CreateTable(
                "dbo.Usuarios",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nombre = c.String(),
                        Correo = c.String(),
                        Password = c.String(),
                        Rol = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Productos",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nombre = c.String(),
                        PrecioOriginal = c.Double(nullable: false),
                        PrecioDescuento = c.Double(nullable: false),
                        PorExpirar = c.Boolean(nullable: false),
                        FechaExpiracion = c.DateTime(),
                        Imagen = c.String(),
                        NegocioId = c.Int(nullable: false),
                        Descripcion = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Negocios", t => t.NegocioId, cascadeDelete: true)
                .Index(t => t.NegocioId);
            
            CreateTable(
                "dbo.Negocios",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nombre = c.String(),
                        Descripcion = c.String(),
                        Direccion = c.String(),
                        Ubicacion = c.String(),
                        Logo = c.String(),
                        Banner = c.String(),
                        UsuarioId = c.Int(nullable: false),
                        CategoriaId = c.Int(nullable: false),
                        Subcategoria1Id = c.Int(),
                        Subcategoria2Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Categorias", t => t.CategoriaId, cascadeDelete: true)
                .ForeignKey("dbo.Subcategorias", t => t.Subcategoria1Id)
                .ForeignKey("dbo.Subcategorias", t => t.Subcategoria2Id)
                .ForeignKey("dbo.Usuarios", t => t.UsuarioId)
                .Index(t => t.UsuarioId)
                .Index(t => t.CategoriaId)
                .Index(t => t.Subcategoria1Id)
                .Index(t => t.Subcategoria2Id);
            
            CreateTable(
                "dbo.Subcategorias",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nombre = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DetallesPedidos", "ProductoId", "dbo.Productos");
            DropForeignKey("dbo.Negocios", "UsuarioId", "dbo.Usuarios");
            DropForeignKey("dbo.Negocios", "Subcategoria2Id", "dbo.Subcategorias");
            DropForeignKey("dbo.Negocios", "Subcategoria1Id", "dbo.Subcategorias");
            DropForeignKey("dbo.Productos", "NegocioId", "dbo.Negocios");
            DropForeignKey("dbo.Negocios", "CategoriaId", "dbo.Categorias");
            DropForeignKey("dbo.Pedidos", "UsuarioId", "dbo.Usuarios");
            DropForeignKey("dbo.DetallesPedidos", "PedidoId", "dbo.Pedidos");
            DropIndex("dbo.Negocios", new[] { "Subcategoria2Id" });
            DropIndex("dbo.Negocios", new[] { "Subcategoria1Id" });
            DropIndex("dbo.Negocios", new[] { "CategoriaId" });
            DropIndex("dbo.Negocios", new[] { "UsuarioId" });
            DropIndex("dbo.Productos", new[] { "NegocioId" });
            DropIndex("dbo.Pedidos", new[] { "UsuarioId" });
            DropIndex("dbo.DetallesPedidos", new[] { "ProductoId" });
            DropIndex("dbo.DetallesPedidos", new[] { "PedidoId" });
            DropTable("dbo.Subcategorias");
            DropTable("dbo.Negocios");
            DropTable("dbo.Productos");
            DropTable("dbo.Usuarios");
            DropTable("dbo.Pedidos");
            DropTable("dbo.DetallesPedidos");
            DropTable("dbo.Categorias");
        }
    }
}
