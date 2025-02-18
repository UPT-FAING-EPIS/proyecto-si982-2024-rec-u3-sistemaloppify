using LoopifyFinal.Models;
using System.Linq;
using System.Web.Mvc;

namespace LoopifyFinal.Controllers
{
    public class CuentaController : Controller
    {
        private readonly LoopifyContext _db = new LoopifyContext();

        // Método auxiliar para obtener el ID del usuario logueado
        private int GetLoggedUserId()
        {
            return Session["UserId"] != null ? (int)Session["UserId"] : 0;
        }

        // Iniciar Sesión
        public ActionResult IniciarSesion()
        {
            return View();
        }

        [HttpPost]
        public ActionResult IniciarSesion(string correo, string password)
        {
            var usuario = _db.Usuarios.FirstOrDefault(u => u.Correo == correo && u.Password == password);

            if (usuario != null)
            {
                Session["UserId"] = usuario.Id; // Guardar el ID del usuario en la sesión
                Session["UserRole"] = usuario.Rol; // Guardar el Rol del usuario en la sesión

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
        public ActionResult CerrarSesion()
        {
            Session.Clear();
            return RedirectToAction("IniciarSesion");
        }

        // Registro de Usuario
        public ActionResult Registro()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Registro(string nombre, string correo, string password)
        {
            if (_db.Usuarios.Any(u => u.Correo == correo))
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

            _db.Usuarios.Add(nuevoUsuario);
            _db.SaveChanges();

            ViewBag.Mensaje = "Registro exitoso. Ahora puede iniciar sesión.";
            return RedirectToAction("IniciarSesion");
        }

        // Reestablecer Contraseña
        public ActionResult ReestablecerContrasena()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ReestablecerContrasena(string correo)
        {
            var usuario = _db.Usuarios.FirstOrDefault(u => u.Correo == correo);

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






//using LoopifyFinal.Models;
//using System.Linq;
//using System.Web.Mvc;

//namespace LoopifyFinal.Controllers
//{
//    public class CuentaController : Controller
//    {
//        private readonly LoopifyContext _db = new LoopifyContext();

//        // Iniciar Sesión
//        public ActionResult IniciarSesion()
//        {
//            return View();
//        }

//        [HttpPost]
//        public ActionResult IniciarSesion(string correo, string password)
//        {
//            using (var context = new LoopifyContext())
//            {
//                var usuario = context.Usuarios.FirstOrDefault(u => u.Correo == correo && u.Password == password);

//                if (usuario != null)
//                {
//                    Session["UserId"] = usuario.Id; // Guardar el ID del usuario en la sesión
//                    Session["UserRole"] = usuario.Rol; // Guardar el Rol del usuario en la sesión

//                    // Redirigir según el rol
//                    if (usuario.Rol == "Administrador")
//                        return RedirectToAction("Panel", "Administrador");
//                    else if (usuario.Rol == "Vendedor")
//                        return RedirectToAction("Panel", "Vendedor");
//                    else if (usuario.Rol == "Cliente")
//                        return RedirectToAction("Inicio", "Cliente");
//                }

//                ViewBag.Error = "Correo o contraseña incorrectos.";
//            }

//            return View();
//        }

//        // Cerrar Sesión
//        public ActionResult CerrarSesion()
//        {
//            Session.Clear();
//            return RedirectToAction("IniciarSesion");
//        }

//        // Registro de Usuario
//        public ActionResult Registro()
//        {
//            return View();
//        }

//        [HttpPost]
//        public ActionResult Registro(string nombre, string correo, string password)
//        {
//            if (_db.Usuarios.Any(u => u.Correo == correo))
//            {
//                ViewBag.Error = "El correo ya está registrado.";
//                return View();
//            }

//            var nuevoUsuario = new Usuario
//            {
//                Nombre = nombre,
//                Correo = correo,
//                Password = password,
//                Rol = "Cliente"
//            };

//            _db.Usuarios.Add(nuevoUsuario);
//            _db.SaveChanges();

//            ViewBag.Mensaje = "Registro exitoso. Ahora puede iniciar sesión.";
//            return RedirectToAction("IniciarSesion");
//        }

//        // Reestablecer Contraseña
//        public ActionResult ReestablecerContrasena()
//        {
//            return View();
//        }

//        [HttpPost]
//        public ActionResult ReestablecerContrasena(string correo)
//        {
//            var usuario = _db.Usuarios.FirstOrDefault(u => u.Correo == correo);

//            if (usuario != null)
//            {
//                // Simular envío de correo para reestablecer contraseña
//                ViewBag.Mensaje = $"Se ha enviado un correo a {correo} con instrucciones para reestablecer su contraseña.";
//            }
//            else
//            {
//                ViewBag.Error = "El correo no está registrado.";
//            }

//            return View();
//        }
//    }
//}
