namespace LoopifyFinal.Models
{
    public class Producto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public double PrecioOriginal { get; set; }
        public double PrecioDescuento { get; set; }
        public bool PorExpirar { get; set; } // Indica si el producto está próximo a expirar
        public DateTime? FechaExpiracion { get; set; } // Fecha de expiración opcional
        public string Imagen { get; set; } // Ruta de la imagen del producto
        public int NegocioId { get; set; }
        public Negocio Negocio { get; set; }
        public string Descripcion { get; set; }
    }
}
