using LoopifyFinal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Loopify.Controllers
{
    public class ClienteController : Controller
    {
        private readonly LoopifyContext _context;

        public ClienteController(LoopifyContext context)
        {
            _context = context;
        }

        public IActionResult Inicio()
        {
            var negocios = _context.Negocios.Include(n => n.Productos).ToList();
            return View(negocios);
        }
    }
}
