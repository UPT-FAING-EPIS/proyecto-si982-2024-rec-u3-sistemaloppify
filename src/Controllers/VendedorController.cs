using LoopifyFinal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LoopifyFinal.Controllers
{
    public class VendedorController : Controller
    {
        private readonly LoopifyContext _db;
        private readonly IWebHostEnvironment _env;

        public VendedorController(LoopifyContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        public IActionResult Panel()
        {
            // Obtener el ID del usuario logueado desde la sesión
            int vendedorId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));

            // Cargar negocios del vendedor
            var negocios = _db.Negocios.Where(n => n.UsuarioId == vendedorId).OrderBy(n => n.Id).ToList();

            // Cargar categorías y subcategorías para el modal
            ViewBag.Categorias = _db.Categorias.ToList();
            ViewBag.Subcategorias = _db.Subcategorias.ToList();

            // Diccionarios para facilitar la búsqueda en el panel
            ViewBag.CategoriasDict = _db.Categorias.ToDictionary(c => c.Id, c => c.Nombre);
            ViewBag.SubcategoriasDict = _db.Subcategorias.ToDictionary(sc => sc.Id, sc => sc.Nombre);

            return View(negocios);
        }

        [HttpGet]
        public IActionResult CrearNegocio()
        {
            ViewBag.Categorias = _db.Categorias.ToList(); // Cargar las categorías
            ViewBag.Subcategorias = _db.Subcategorias.ToList(); // Cargar las subcategorías
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CrearNegocio(string nombre, string descripcion, string direccion, string ubicacion, int categoriaId, int? subcategoria1Id, int? subcategoria2Id, IFormFile logo, IFormFile banner)
        {
            // Obtener el ID del usuario logueado desde la sesión
            int vendedorId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));

            var nuevoNegocio = new Negocio
            {
                Nombre = nombre,
                Descripcion = descripcion,
                Direccion = direccion,
                Ubicacion = ubicacion,
                UsuarioId = vendedorId,
                CategoriaId = categoriaId,
                Subcategoria1Id = subcategoria1Id,
                Subcategoria2Id = subcategoria2Id
            };

            // Procesar logo
            if (logo != null)
            {
                string logoPath = await SaveImage(logo, "Logos", 150, 150, true);
                nuevoNegocio.Logo = logoPath;
            }

            // Procesar banner
            if (banner != null)
            {
                string bannerPath = await SaveImage(banner, "Banners", 1024, 300, false);
                nuevoNegocio.Banner = bannerPath;
            }

            _db.Negocios.Add(nuevoNegocio);
            await _db.SaveChangesAsync();

            TempData["Mensaje"] = $"¡Negocio '{nombre}' creado con éxito!";
            return RedirectToAction("Panel");
        }

        [HttpGet]
        public IActionResult EditarNegocio(int id)
        {
            var negocio = _db.Negocios.Find(id);
            if (negocio == null)
                return NotFound();

            ViewBag.Categorias = _db.Categorias.ToList();
            ViewBag.Subcategorias = _db.Subcategorias.ToList();
            return View(negocio);
        }

        [HttpPost]
        public async Task<IActionResult> EliminarNegocio(int id)
        {
            try
            {
                var negocio = _db.Negocios.FirstOrDefault(n => n.Id == id);
                if (negocio == null)
                {
                    TempData["Error"] = "El negocio no existe.";
                    return RedirectToAction("Panel");
                }

                var productos = _db.Productos.Where(p => p.NegocioId == id).ToList();
                if (productos.Any())
                {
                    _db.Productos.RemoveRange(productos);
                }

                _db.Negocios.Remove(negocio);
                await _db.SaveChangesAsync();

                TempData["Mensaje"] = "Negocio y sus productos eliminados con éxito.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al eliminar el negocio: {ex.Message}";
            }

            return RedirectToAction("Panel");
        }

        public IActionResult GestionarProductos(int? negocioId)
        {
            var categorias = _db.Categorias.ToList();
            var subcategorias = _db.Subcategorias.ToList();
            ViewBag.Categorias = categorias;
            ViewBag.Subcategorias = subcategorias;

            if (!negocioId.HasValue || negocioId == 0)
            {
                ViewBag.Negocios = _db.Negocios.ToList();
                ViewBag.NegocioSeleccionado = null;
                return View(new List<Producto>());
            }

            var negocio = _db.Negocios.FirstOrDefault(n => n.Id == negocioId);
            if (negocio == null)
            {
                TempData["Error"] = "El negocio seleccionado no existe.";
                return RedirectToAction("Panel");
            }

            ViewBag.NegocioSeleccionado = negocio;
            ViewBag.Productos = _db.Productos.Where(p => p.NegocioId == negocioId).ToList();
            ViewBag.Negocios = _db.Negocios.ToList();

            return View(ViewBag.Productos);
        }

        [HttpPost]
        public async Task<IActionResult> AgregarProducto(int? negocioId, string nombre, double precioOriginal, double? porcentajeDescuento, DateTime? fechaExpiracion, string descripcion, string imagenesSeleccionadas)
        {
            if (!negocioId.HasValue || negocioId.Value == 0)
            {
                TempData["Error"] = "Por favor, seleccione un negocio antes de agregar un producto.";
                return RedirectToAction("GestionarProductos");
            }

            var negocio = _db.Negocios.FirstOrDefault(n => n.Id == negocioId);
            if (negocio == null)
            {
                TempData["Error"] = "El negocio seleccionado no existe.";
                return RedirectToAction("GestionarProductos");
            }

            double precioDescuento = porcentajeDescuento.HasValue
                ? precioOriginal - (precioOriginal * (porcentajeDescuento.Value / 100))
                : precioOriginal;

            var nuevoProducto = new Producto
            {
                Nombre = nombre,
                PrecioOriginal = precioOriginal,
                PrecioDescuento = precioDescuento,
                PorExpirar = fechaExpiracion.HasValue,
                FechaExpiracion = fechaExpiracion,
                Descripcion = descripcion,
                NegocioId = negocioId.Value,
                Imagen = null // Esto se actualizará más adelante
            };

            if (!string.IsNullOrEmpty(imagenesSeleccionadas))
            {
                var imagenes = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(imagenesSeleccionadas);
                var rutasImagenes = new List<string>();

                foreach (var base64Image in imagenes)
                {
                    var fileName = $"{Guid.NewGuid()}.jpg";
                    var filePath = Path.Combine(_env.WebRootPath, "Uploads/Productos", fileName);

                    var bytes = Convert.FromBase64String(base64Image.Split(',')[1]);
                    await System.IO.File.WriteAllBytesAsync(filePath, bytes);

                    rutasImagenes.Add($"/Uploads/Productos/{fileName}");
                }

                nuevoProducto.Imagen = string.Join(",", rutasImagenes);
            }

            _db.Productos.Add(nuevoProducto);
            await _db.SaveChangesAsync();

            TempData["Mensaje"] = $"¡Producto '{nombre}' agregado con éxito al negocio '{negocio.Nombre}'!";
            return RedirectToAction("GestionarProductos", new { negocioId = negocioId.Value });
        }

        private async Task<string> SaveImage(IFormFile file, string folder, int width, int height, bool isCircular)
        {
            var filePath = $"/Uploads/{folder}/{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var serverPath = Path.Combine(_env.WebRootPath, filePath);

            var directoryPath = Path.GetDirectoryName(serverPath);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            using (var fileStream = new FileStream(serverPath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            // Procesa la imagen (esto requiere implementar el método ResizeImage en ASP.NET Core también)
            return filePath;
        }
    }
}
