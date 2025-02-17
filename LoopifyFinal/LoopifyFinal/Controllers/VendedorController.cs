using LoopifyFinal.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LoopifyFinal.Controllers
{
    public class VendedorController : Controller
    {
        private readonly LoopifyContext _db = new LoopifyContext();

        
        public ActionResult Panel()
        {
            // Obtener el ID del usuario logueado desde la sesión
            int vendedorId = (int)Session["UserId"];

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
        public ActionResult CrearNegocio()
        {
            ViewBag.Categorias = _db.Categorias.ToList(); // Cargar las categorías
            ViewBag.Subcategorias = _db.Subcategorias.ToList(); // Cargar las subcategorías
            return View();
        }

        [HttpPost]
        public ActionResult CrearNegocio(string nombre, string descripcion, string direccion, string ubicacion, int categoriaId, int? subcategoria1Id, int? subcategoria2Id, HttpPostedFileBase logo, HttpPostedFileBase banner)
        {
            // Obtener el ID del usuario logueado desde la sesión
            int vendedorId = (int)Session["UserId"];

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
                string logoPath = SaveImage(logo, "Logos", 150, 150, true);
                nuevoNegocio.Logo = logoPath;
            }

            // Procesar banner
            if (banner != null)
            {
                string bannerPath = SaveImage(banner, "Banners", 1024, 300, false);
                nuevoNegocio.Banner = bannerPath;
            }

            _db.Negocios.Add(nuevoNegocio);
            _db.SaveChanges();

            TempData["Mensaje"] = $"¡Negocio '{nombre}' creado con éxito!";
            return RedirectToAction("Panel");
        }

        [HttpGet]
        public ActionResult EditarNegocio(int id)
        {
            var negocio = _db.Negocios.Find(id);
            if (negocio == null)
                return HttpNotFound();

            ViewBag.Categorias = _db.Categorias.ToList();
            ViewBag.Subcategorias = _db.Subcategorias.ToList();
            return View(negocio);
        }

        //[HttpPost]
        //public ActionResult EditarProducto(int id, string nombre, string descripcion, double precioOriginal, double? porcentajeDescuento, DateTime? fechaExpiracion, IEnumerable<HttpPostedFileBase> nuevasImagenes, string imagenesAEliminar)
        //{
        //    var producto = _db.Productos.FirstOrDefault(p => p.Id == id);
        //    if (producto == null)
        //    {
        //        TempData["Error"] = "El producto no existe.";
        //        return RedirectToAction("GestionarProductos");
        //    }

        //    // Actualizar datos básicos del producto
        //    producto.Nombre = nombre;
        //    producto.Descripcion = descripcion;
        //    producto.PrecioOriginal = precioOriginal;
        //    producto.PrecioDescuento = porcentajeDescuento.HasValue
        //        ? precioOriginal - (precioOriginal * (porcentajeDescuento.Value / 100))
        //        : precioOriginal;

        //    producto.FechaExpiracion = fechaExpiracion;
        //    producto.PorExpirar = fechaExpiracion.HasValue;

        //    // Manejo de imágenes existentes
        //    var rutasImagenes = string.IsNullOrWhiteSpace(producto.Imagen)
        //        ? new List<string>()
        //        : producto.Imagen.Split(',').ToList();

        //    // Eliminar imágenes marcadas
        //    if (!string.IsNullOrEmpty(imagenesAEliminar))
        //    {
        //        var imagenesEliminar = imagenesAEliminar.Split(',');
        //        foreach (var img in imagenesEliminar)
        //        {
        //            rutasImagenes.Remove(img);

        //            // Eliminar físicamente las imágenes del servidor
        //            var rutaFisica = Server.MapPath(img);
        //            if (System.IO.File.Exists(rutaFisica))
        //            {
        //                System.IO.File.Delete(rutaFisica);
        //            }
        //        }
        //    }

        //    // Agregar nuevas imágenes
        //    if (nuevasImagenes != null)
        //    {
        //        foreach (var nuevaImagen in nuevasImagenes)
        //        {
        //            if (nuevaImagen != null && nuevaImagen.ContentLength > 0)
        //            {
        //                string rutaImagen = SaveImage(nuevaImagen, "Productos", 300, 300, false); // Usa la misma lógica de añadir
        //                rutasImagenes.Add(rutaImagen);
        //            }
        //        }
        //    }

        //    // Actualizar el campo de imágenes concatenadas
        //    producto.Imagen = string.Join(",", rutasImagenes);

        //    _db.SaveChanges();

        //    TempData["Mensaje"] = $"¡Producto '{nombre}' actualizado con éxito!";
        //    return RedirectToAction("GestionarProductos", new { negocioId = producto.NegocioId });
        //}


        [HttpGet]
        public ActionResult EliminarNegocio(int id)
        {
            try
            {
                // Buscar el negocio por ID
                var negocio = _db.Negocios.FirstOrDefault(n => n.Id == id);
                if (negocio == null)
                {
                    TempData["Error"] = "El negocio no existe.";
                    return RedirectToAction("Panel");
                }

                // Eliminar todos los productos asociados al negocio
                var productos = _db.Productos.Where(p => p.NegocioId == id).ToList();
                if (productos.Any())
                {
                    _db.Productos.RemoveRange(productos);
                }

                // Eliminar el negocio
                _db.Negocios.Remove(negocio);
                _db.SaveChanges();

                TempData["Mensaje"] = "Negocio y sus productos eliminados con éxito.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al eliminar el negocio: {ex.Message}";
            }

            return RedirectToAction("Panel");
        }


        public ActionResult GestionarProductos(int? negocioId)
        {
            System.Diagnostics.Debug.WriteLine("Negocio ID recibido: " + negocioId);

            // Cargar listas de categorías y subcategorías
            var categorias = _db.Categorias.ToList();
            var subcategorias = _db.Subcategorias.ToList();
            ViewBag.Categorias = categorias;
            ViewBag.Subcategorias = subcategorias;

            // Crear diccionarios para acceso rápido en la vista
            ViewBag.CategoriasDict = categorias.ToDictionary(c => c.Id, c => c.Nombre);
            ViewBag.SubcategoriasDict = subcategorias.ToDictionary(sc => sc.Id, sc => sc.Nombre);


            // Si no se seleccionó un negocio o el ID es inválido
            if (!negocioId.HasValue || negocioId == 0)
            {
                ViewBag.Negocios = _db.Negocios.ToList();
                ViewBag.NegocioSeleccionado = null;
                return View(new List<Producto>());
            }

            // Obtener el negocio seleccionado
            var negocio = _db.Negocios.FirstOrDefault(n => n.Id == negocioId);
            if (negocio == null)
            {
                TempData["Error"] = "El negocio seleccionado no existe.";
                return RedirectToAction("Panel");
            }

            // Cargar los productos del negocio seleccionado
            ViewBag.NegocioSeleccionado = negocio;
            ViewBag.Productos = _db.Productos.Where(p => p.NegocioId == negocioId).ToList();
            ViewBag.Negocios = _db.Negocios.ToList();

            return View(ViewBag.Productos);
        }

        [HttpPost]
        public ActionResult EditarProducto(int id, string nombre, string descripcion, double precioOriginal, double? porcentajeDescuento, DateTime? fechaExpiracion, IEnumerable<HttpPostedFileBase> nuevasImagenes, string[] imagenesAEliminar)
        {
            var producto = _db.Productos.FirstOrDefault(p => p.Id == id);
            if (producto == null)
            {
                TempData["Error"] = "El producto no existe.";
                return RedirectToAction("GestionarProductos");
            }

            // Actualizar datos del producto
            producto.Nombre = nombre;
            producto.Descripcion = descripcion;
            producto.PrecioOriginal = precioOriginal;
            producto.PrecioDescuento = porcentajeDescuento.HasValue
                ? precioOriginal - (precioOriginal * (porcentajeDescuento.Value / 100))
                : precioOriginal;
            producto.FechaExpiracion = fechaExpiracion;
            producto.PorExpirar = fechaExpiracion.HasValue;

            // Manejo de imágenes
            var rutasImagenes = string.IsNullOrWhiteSpace(producto.Imagen)
                ? new List<string>()
                : producto.Imagen.Split(',').ToList();

            // Eliminar imágenes marcadas
            if (imagenesAEliminar != null && imagenesAEliminar.Length > 0)
            {
                foreach (var imagen in imagenesAEliminar)
                {
                    rutasImagenes.Remove(imagen);
                    var rutaFisica = Server.MapPath(imagen);
                    if (System.IO.File.Exists(rutaFisica))
                    {
                        System.IO.File.Delete(rutaFisica);
                    }
                }
            }

            // Agregar nuevas imágenes
            if (nuevasImagenes != null)
            {
                foreach (var nuevaImagen in nuevasImagenes)
                {
                    if (nuevaImagen != null && nuevaImagen.ContentLength > 0)
                    {
                        string rutaImagen = SaveImage(nuevaImagen, "Productos", 300, 300, false);
                        rutasImagenes.Add(rutaImagen);
                    }
                }
            }

            // Actualizar la lista de imágenes
            producto.Imagen = string.Join(",", rutasImagenes);

            _db.SaveChanges();

            TempData["Mensaje"] = $"¡Producto '{nombre}' actualizado con éxito!";
            return RedirectToAction("GestionarProductos", new { negocioId = producto.NegocioId });
        }




        [HttpPost]
        public ActionResult AgregarProducto(int? negocioId, string nombre, double precioOriginal, double? porcentajeDescuento, DateTime? fechaExpiracion, string descripcion, string imagenesSeleccionadas)
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
                    var filePath = Path.Combine(Server.MapPath("~/Uploads/Productos"), fileName);

                    // Decodificar la imagen base64 y guardarla
                    var bytes = Convert.FromBase64String(base64Image.Split(',')[1]);
                    System.IO.File.WriteAllBytes(filePath, bytes);

                    rutasImagenes.Add($"/Uploads/Productos/{fileName}");
                }

                nuevoProducto.Imagen = string.Join(",", rutasImagenes);
            }

            _db.Productos.Add(nuevoProducto);
            _db.SaveChanges();

            TempData["Mensaje"] = $"¡Producto '{nombre}' agregado con éxito al negocio '{negocio.Nombre}'!";
            return RedirectToAction("GestionarProductos", new { negocioId = negocioId.Value });
        }

        [HttpGet]
        public ActionResult EliminarProducto(int id)
        {
            Producto producto = null;

            try
            {
                // Buscar el producto por ID
                producto = _db.Productos.FirstOrDefault(p => p.Id == id);
                if (producto == null)
                {
                    TempData["Error"] = "El producto no existe.";
                    return RedirectToAction("GestionarProductos");
                }

                // Eliminar el producto de la base de datos
                _db.Productos.Remove(producto);
                _db.SaveChanges();

                TempData["Mensaje"] = "Producto eliminado con éxito.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al eliminar el producto: {ex.Message}";
            }

            // Redirigir a la vista de gestión de productos, usando el NegocioId del producto eliminado
            return RedirectToAction("GestionarProductos", new { negocioId = producto?.NegocioId });
        }

        private string SaveImage(HttpPostedFileBase file, string folder, int width, int height, bool isCircular)
        {
            string filePath = $"/Uploads/{folder}/{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            string serverPath = Server.MapPath(filePath);

            // Crea la carpeta si no existe
            string directoryPath = Path.GetDirectoryName(serverPath);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            // Procesa la imagen
            byte[] fileBytes;
            using (var memoryStream = new MemoryStream())
            {
                file.InputStream.CopyTo(memoryStream);
                fileBytes = memoryStream.ToArray();
            }

            using (var image = System.Drawing.Image.FromStream(new MemoryStream(fileBytes)))
            {
                using (var resizedImage = ResizeImage(image, width, height, isCircular))
                {
                    resizedImage.Save(serverPath);
                }
            }

            return filePath;
        }


        private System.Drawing.Image ResizeImage(System.Drawing.Image originalImage, int width, int height, bool isCircular)
        {
            var resized = new System.Drawing.Bitmap(width, height);

            using (var graphics = System.Drawing.Graphics.FromImage(resized))
            {
                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                var destRect = new System.Drawing.Rectangle(0, 0, width, height);

                graphics.DrawImage(originalImage, destRect);

                if (isCircular)
                {
                    var mask = new System.Drawing.Bitmap(width, height);
                    using (var g = System.Drawing.Graphics.FromImage(mask))
                    {
                        g.Clear(System.Drawing.Color.Transparent);
                        using (var brush = new System.Drawing.SolidBrush(System.Drawing.Color.White))
                        {
                            g.FillEllipse(brush, destRect);
                        }
                    }

                    resized.MakeTransparent(System.Drawing.Color.Black);
                }
            }

            return resized;
        }
    }
}
