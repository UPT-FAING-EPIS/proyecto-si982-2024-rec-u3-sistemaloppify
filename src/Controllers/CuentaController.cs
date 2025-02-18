using LoopifyFinal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace Loopify.Controllers
{
    public class CuentaController : Controller
    {
        private readonly LoopifyContext _context;

        public CuentaController(LoopifyContext context)
        {
            _context = context;
        }

        // Método auxiliar para obtener el ID del usuario logueado
        private int GetLoggedUserId()
        {
            return HttpContext.Session.GetInt32("UserId") ?? 0;
        }

        // Iniciar Sesión
        public IActionResult IniciarSesion()
        {
            return View();
        }

        [HttpPost]
        public IActionResult IniciarSesion(string correo, string password)
        {
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Correo == correo && u.Password == password);

            if (usuario != null)
            {
                HttpContext.Session.SetInt32("UserId", usuario.Id); // Guardar el ID del usuario en la sesión
                HttpContext.Session.SetString("UserRole", usuario.Rol); // Guardar el Rol del usuario en la sesión

                // Redirigir según el rol
                if (usuario.Rol == "Administrador")
                    return RedirectToAction("Panel", "Administrador");
                else if (usuario.Rol == "Vendedor")
                    return RedirectToAction("Panel", "Vendedor");
                else if (usuario.Rol == "Cliente")
                    return RedirectToAction("Inicio", "Cliente");
            }

            ViewBag.Error = "Correo o contraseña incorrectos.";
            return View();
        }

        // Cerrar Sesión
        public IActionResult CerrarSesion()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("IniciarSesion");
        }

        // Registro de Usuario
        public IActionResult Registro()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Registro(string nombre, string correo, string password)
        {
            if (_context.Usuarios.Any(u => u.Correo == correo))
            {
                ViewBag.Error = "El correo ya está registrado.";
                return View();
            }

            var nuevoUsuario = new Usuario
            {
                Nombre = nombre,
                Correo = correo,
                Password = password,
                Rol = "Cliente"
            };

            _context.Usuarios.Add(nuevoUsuario);
            _context.SaveChanges();

            ViewBag.Mensaje = "Registro exitoso. Ahora puede iniciar sesión.";
            return RedirectToAction("IniciarSesion");
        }

        // Reestablecer Contraseña
        public IActionResult ReestablecerContrasena()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ReestablecerContrasena(string correo)
        {
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Correo == correo);

            if (usuario != null)
            {
                ViewBag.Mensaje = $"Se ha enviado un correo a {correo} con instrucciones para reestablecer su contraseña.";
            }
            else
            {
                ViewBag.Error = "El correo no está registrado.";
            }

            return View();
        }
    }
}
