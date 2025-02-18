using LoopifyFinal.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace LoopifyFinal.Controllers
{
    public class AdministradorController : Controller
    {
        private readonly LoopifyContext _db = new LoopifyContext();

        public ActionResult Panel()
        {
            var vendedores = _db.Usuarios.Where(u => u.Rol == "Vendedor").ToList();
            return View(vendedores);
        }

        [HttpGet]
        public ActionResult CrearVendedor()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CrearVendedor(string nombreVendedor, string correo, string password)
        {
            var nuevoVendedor = new Usuario
            {
                Nombre = nombreVendedor,
                Correo = correo,
                Password = password,
                Rol = "Vendedor"
            };

            _db.Usuarios.Add(nuevoVendedor);
            _db.SaveChanges();

            TempData["Mensaje"] = $"¡Cuenta del vendedor '{nombreVendedor}' creada con éxito!";
            return RedirectToAction("Panel");
        }

        [HttpGet]
        public ActionResult EditarVendedor(int id)
        {
            var vendedor = _db.Usuarios.FirstOrDefault(u => u.Id == id && u.Rol == "Vendedor");
            if (vendedor == null)
            {
                return HttpNotFound();
            }
            return View(vendedor);
        }

        [HttpPost]
        public ActionResult EditarVendedor(Usuario vendedor)
        {
            var vendedorExistente = _db.Usuarios.FirstOrDefault(u => u.Id == vendedor.Id && u.Rol == "Vendedor");
            if (vendedorExistente != null)
            {
                vendedorExistente.Nombre = vendedor.Nombre;
                vendedorExistente.Correo = vendedor.Correo;
                vendedorExistente.Password = vendedor.Password;
                _db.SaveChanges();
            }
            return RedirectToAction("Panel");
        }

        [HttpGet]
        public ActionResult DetallesVendedor(int id)
        {
            var vendedor = _db.Usuarios.FirstOrDefault(u => u.Id == id && u.Rol == "Vendedor");
            if (vendedor == null)
            {
                return HttpNotFound();
            }
            return View(vendedor);
        }

        [HttpPost]
        public ActionResult EliminarVendedor(int id)
        {
            var vendedor = _db.Usuarios.FirstOrDefault(u => u.Id == id && u.Rol == "Vendedor");
            if (vendedor != null)
            {
                _db.Usuarios.Remove(vendedor);
                _db.SaveChanges();
            }
            return RedirectToAction("Panel");
        }

        public ActionResult GestionarNegocios()
        {
            var negocios = _db.Negocios.ToList();
            return View(negocios);
        }

        public ActionResult GestionarCategorias()
        {
            var categorias = _db.Categorias.ToList();
            return View(categorias);
        }

        [HttpPost]
        public ActionResult CrearCategoria(string nombre)
        {
            if (!string.IsNullOrEmpty(nombre))
            {
                var nuevaCategoria = new Categoria { Nombre = nombre };
                _db.Categorias.Add(nuevaCategoria);
                _db.SaveChanges();
                TempData["Mensaje"] = "¡Categoría creada con éxito!";
            }
            else
            {
                TempData["Error"] = "El nombre de la categoría no puede estar vacío.";
            }

            return RedirectToAction("GestionarCategorias");
        }

        [HttpPost]
        public ActionResult EditarCategoria(int id, string nombre)
        {
            var categoria = _db.Categorias.FirstOrDefault(c => c.Id == id);

            if (categoria != null && !string.IsNullOrEmpty(nombre))
            {
                categoria.Nombre = nombre;
                _db.SaveChanges();
                TempData["Mensaje"] = "¡Categoría actualizada con éxito!";
            }
            else
            {
                TempData["Error"] = "Error al editar la categoría. Asegúrate de que el nombre no esté vacío.";
            }

            return RedirectToAction("GestionarCategorias");
        }

        [HttpPost]
        public ActionResult EliminarCategoria(int id)
        {
            var categoria = _db.Categorias.FirstOrDefault(c => c.Id == id);

            if (categoria != null)
            {
                _db.Categorias.Remove(categoria);
                _db.SaveChanges();
                TempData["Mensaje"] = "¡Categoría eliminada con éxito!";
            }
            else
            {
                TempData["Error"] = "Error al eliminar la categoría.";
            }

            return RedirectToAction("GestionarCategorias");
        }

        public ActionResult GestionarSubcategorias()
        {
            var subcategorias = _db.Subcategorias.ToList();
            return View(subcategorias);
        }

        [HttpPost]
        public ActionResult CrearSubcategoria(string nombre)
        {
            if (!string.IsNullOrEmpty(nombre))
            {
                var nuevaSubcategoria = new Subcategoria { Nombre = nombre };
                _db.Subcategorias.Add(nuevaSubcategoria);
                _db.SaveChanges();
                TempData["Mensaje"] = "¡Subcategoría creada con éxito!";
            }
            else
            {
                TempData["Error"] = "El nombre de la subcategoría no puede estar vacío.";
            }

            return RedirectToAction("GestionarSubcategorias");
        }

        [HttpPost]
        public ActionResult EditarSubcategoria(int id, string nombre)
        {
            var subcategoria = _db.Subcategorias.FirstOrDefault(sc => sc.Id == id);

            if (subcategoria != null && !string.IsNullOrEmpty(nombre))
            {
                subcategoria.Nombre = nombre;
                _db.SaveChanges();
                TempData["Mensaje"] = "¡Subcategoría actualizada con éxito!";
            }
            else
            {
                TempData["Error"] = "Error al editar la subcategoría. Asegúrate de que el nombre no esté vacío.";
            }

            return RedirectToAction("GestionarSubcategorias");
        }

        [HttpPost]
        public ActionResult EliminarSubcategoria(int id)
        {
            var subcategoria = _db.Subcategorias.FirstOrDefault(sc => sc.Id == id);

            if (subcategoria != null)
            {
                _db.Subcategorias.Remove(subcategoria);
                _db.SaveChanges();
                TempData["Mensaje"] = "¡Subcategoría eliminada con éxito!";
            }
            else
            {
                TempData["Error"] = "Error al eliminar la subcategoría.";
            }

            return RedirectToAction("GestionarSubcategorias");
        }

        public ActionResult ResumenVentas()
        {
            var resumenVentas = _db.Negocios
                .Select(n => new
                {
                    Negocio = n.Nombre,
                    TotalVentas = n.Productos
                        .Join(_db.Pedidos.SelectMany(p => p.Detalles),
                              producto => producto.Id,
                              detalle => detalle.ProductoId,
                              (producto, detalle) => detalle)
                        .Sum(d => d.Cantidad * d.Precio)
                })
                .ToList();

            return View(resumenVentas);
        }

        public ActionResult GestionarPublicidad()
        {
            return View();
        }
    }
}
