using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LoopifyFinal.Models
{
    public class Pedido
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }
        public ICollection<DetallePedido> Detalles { get; set; }
    }

}