using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LoopifyFinal.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Password { get; set; }
        public string Rol { get; set; } // Puede ser "Administrador", "Vendedor", o "Cliente"
    }

}