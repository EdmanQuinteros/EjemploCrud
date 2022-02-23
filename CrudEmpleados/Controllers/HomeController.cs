using CrudEmpleados.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CrudEmpleados.Controllers
{
    public class HomeController : Controller
    {
        //creamps una variable privada de solo lectura //
        private readonly ApplicationDbContext _dbContext;

        private readonly IWebHostEnvironment _hostingEnvironment;

        public HomeController(ApplicationDbContext dbContext, IWebHostEnvironment hostingEnvironment)
        {
            _dbContext = dbContext;
            _hostingEnvironment = hostingEnvironment;
        }

        //hacemos sea asincrono para que ayude mas en el rendimiento de nuestra aplicacion//
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View(await _dbContext.Empleado.ToListAsync()); // para que enviar como lista y asincrono ya q asi emos empleado//
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Empleado empleado)
        {
            // validamos con el if si el modelo es valido
            if (ModelState.IsValid)
            {
                // para las imagenes
                string rutaPrincipal = _hostingEnvironment.WebRootPath;
                var archivos = HttpContext.Request.Form.Files;

                //nuevo articulo (Guid - permite guardar un archivo o variable con nombre muy grande //
                string nombreArchivo = Guid.NewGuid().ToString();
                var subidas = Path.Combine(rutaPrincipal, @"imagenes\empleados\");
                var extension = Path.GetExtension(archivos[0].FileName);

                using (var fileStreams = new FileStream(Path.Combine(subidas, nombreArchivo + extension), FileMode.Create))
                {
                    archivos[0].CopyTo(fileStreams);
                }

                empleado.Foto = @"\imagenes\empleados\" + nombreArchivo + extension;

                _dbContext.Empleado.Add(empleado);  //añademos los valores 
                await _dbContext.SaveChangesAsync(); //guardamos los valores
                return RedirectToAction(nameof(Index)); //redireccionamos al index
            }
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
