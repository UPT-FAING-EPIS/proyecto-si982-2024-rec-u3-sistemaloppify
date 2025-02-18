namespace LoopifyFinal.Models
{
    public class Negocio
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Direccion { get; set; }
        public string Ubicacion { get; set; } // Coordenadas o enlace a ubicación
        public string Logo { get; set; } // URL o byte array para el logo
        public string Banner { get; set; } // URL o byte array para el banner
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }
        public int CategoriaId { get; set; } // Campo obligatorio
        public Categoria Categoria { get; set; }

        public int? Subcategoria1Id { get; set; } // Primera subcategoría opcional
        public Subcategoria Subcategoria1 { get; set; }

        public int? Subcategoria2Id { get; set; } // Segunda subcategoría opcional
        public Subcategoria Subcategoria2 { get; set; }
        public ICollection<Producto> Productos { get; set; }
    }
}
