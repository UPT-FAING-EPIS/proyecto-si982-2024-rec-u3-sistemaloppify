using LoopifyFinal.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Loopify.Controllers
{
    public class ClienteController : Controller
    {
        private readonly LoopifyContext _db = new LoopifyContext();

        public ActionResult Inicio()
        {
            var negocios = _db.Negocios.Include("Productos").ToList();
            return View(negocios);
        }
    }
}
