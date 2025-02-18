using LoopifyFinal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace LoopifyFinal.Controllers
{
    public class AdministradorController : Controller
    {
        private readonly LoopifyContext _context;

        public AdministradorController(LoopifyContext context)
        {
            _context = context;
        }

        public IActionResult Panel()
        {
            var vendedores = _context.Usuarios.Where(u => u.Rol == "Vendedor").ToList();
            return View(vendedores);
        }

        [HttpGet]
        public IActionResult CrearVendedor()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CrearVendedor(string nombreVendedor, string correo, string password)
        {
            var nuevoVendedor = new Usuario
            {
                Nombre = nombreVendedor,
                Correo = correo,
                Password = password,
                Rol = "Vendedor"
            };

            _context.Usuarios.Add(nuevoVendedor);
            _context.SaveChanges();

            TempData["Mensaje"] = $"¡Cuenta del vendedor '{nombreVendedor}' creada con éxito!";
            return RedirectToAction("Panel");
        }

        [HttpGet]
        public IActionResult EditarVendedor(int id)
        {
            var vendedor = _context.Usuarios.FirstOrDefault(u => u.Id == id && u.Rol == "Vendedor");
            if (vendedor == null)
            {
                return NotFound();
            }
            return View(vendedor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditarVendedor(Usuario vendedor)
        {
            var vendedorExistente = _context.Usuarios.FirstOrDefault(u => u.Id == vendedor.Id && u.Rol == "Vendedor");
            if (vendedorExistente != null)
            {
                vendedorExistente.Nombre = vendedor.Nombre;
                vendedorExistente.Correo = vendedor.Correo;
                vendedorExistente.Password = vendedor.Password;
                _context.SaveChanges();
            }
            return RedirectToAction("Panel");
        }

        [HttpGet]
        public IActionResult DetallesVendedor(int id)
        {
            var vendedor = _context.Usuarios.FirstOrDefault(u => u.Id == id && u.Rol == "Vendedor");
            if (vendedor == null)
            {
                return NotFound();
            }
            return View(vendedor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EliminarVendedor(int id)
        {
            var vendedor = _context.Usuarios.FirstOrDefault(u => u.Id == id && u.Rol == "Vendedor");
            if (vendedor != null)
            {
                _context.Usuarios.Remove(vendedor);
                _context.SaveChanges();
            }
            return RedirectToAction("Panel");
        }

        public IActionResult GestionarNegocios()
        {
            var negocios = _context.Negocios.ToList();
            return View(negocios);
        }

        public IActionResult GestionarCategorias()
        {
            var categorias = _context.Categorias.ToList();
            return View(categorias);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CrearCategoria(string nombre)
        {
            if (!string.IsNullOrEmpty(nombre))
            {
                var nuevaCategoria = new Categoria { Nombre = nombre };
                _context.Categorias.Add(nuevaCategoria);
                _context.SaveChanges();
                TempData["Mensaje"] = "¡Categoría creada con éxito!";
            }
            else
            {
                TempData["Error"] = "El nombre de la categoría no puede estar vacío.";
            }

            return RedirectToAction("GestionarCategorias");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditarCategoria(int id, string nombre)
        {
            var categoria = _context.Categorias.FirstOrDefault(c => c.Id == id);

            if (categoria != null && !string.IsNullOrEmpty(nombre))
            {
                categoria.Nombre = nombre;
                _context.SaveChanges();
                TempData["Mensaje"] = "¡Categoría actualizada con éxito!";
            }
            else
            {
                TempData["Error"] = "Error al editar la categoría. Asegúrate de que el nombre no esté vacío.";
            }

            return RedirectToAction("GestionarCategorias");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EliminarCategoria(int id)
        {
            var categoria = _context.Categorias.FirstOrDefault(c => c.Id == id);

            if (categoria != null)
            {
                _context.Categorias.Remove(categoria);
                _context.SaveChanges();
                TempData["Mensaje"] = "¡Categoría eliminada con éxito!";
            }
            else
            {
                TempData["Error"] = "Error al eliminar la categoría.";
            }

            return RedirectToAction("GestionarCategorias");
        }

        public IActionResult GestionarSubcategorias()
        {
            var subcategorias = _context.Subcategorias.ToList();
            return View(subcategorias);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CrearSubcategoria(string nombre)
        {
            if (!string.IsNullOrEmpty(nombre))
            {
                var nuevaSubcategoria = new Subcategoria { Nombre = nombre };
                _context.Subcategorias.Add(nuevaSubcategoria);
                _context.SaveChanges();
                TempData["Mensaje"] = "¡Subcategoría creada con éxito!";
            }
            else
            {
                TempData["Error"] = "El nombre de la subcategoría no puede estar vacío.";
            }

            return RedirectToAction("GestionarSubcategorias");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditarSubcategoria(int id, string nombre)
        {
            var subcategoria = _context.Subcategorias.FirstOrDefault(sc => sc.Id == id);

            if (subcategoria != null && !string.IsNullOrEmpty(nombre))
            {
                subcategoria.Nombre = nombre;
                _context.SaveChanges();
                TempData["Mensaje"] = "¡Subcategoría actualizada con éxito!";
            }
            else
            {
                TempData["Error"] = "Error al editar la subcategoría. Asegúrate de que el nombre no esté vacío.";
            }

            return RedirectToAction("GestionarSubcategorias");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EliminarSubcategoria(int id)
        {
            var subcategoria = _context.Subcategorias.FirstOrDefault(sc => sc.Id == id);

            if (subcategoria != null)
            {
                _context.Subcategorias.Remove(subcategoria);
                _context.SaveChanges();
                TempData["Mensaje"] = "¡Subcategoría eliminada con éxito!";
            }
            else
            {
                TempData["Error"] = "Error al eliminar la subcategoría.";
            }

            return RedirectToAction("GestionarSubcategorias");
        }

        public IActionResult ResumenVentas()
        {
            var resumenVentas = _context.Negocios
                .Select(n => new
                {
                    Negocio = n.Nombre,
                    TotalVentas = n.Productos
                        .Join(_context.Pedidos.SelectMany(p => p.Detalles),
                              producto => producto.Id,
                              detalle => detalle.ProductoId,
                              (producto, detalle) => detalle)
                        .Sum(d => d.Cantidad * d.Precio)
                })
                .ToList();

            return View(resumenVentas);
        }

        public IActionResult GestionarPublicidad()
        {
            return View();
        }
    }
}
