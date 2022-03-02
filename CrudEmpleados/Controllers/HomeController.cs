using CrudEmpleados.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
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

                if (archivos != null && archivos.Count > 0) //solo si selecciona una imagen
                {
                    //nuevo articulo (Guid - permite guardar un archivo o variable con nombre muy grande //
                    string nombreArchivo = Guid.NewGuid().ToString();
                    var subidas = Path.Combine(rutaPrincipal, @"imagenes\empleados");
                    var extension = Path.GetExtension(archivos[0].FileName);



                    using (var fileStreams = new FileStream(Path.Combine(subidas, nombreArchivo + extension), FileMode.Create))
                    {
                        archivos[0].CopyTo(fileStreams);
                    }

                    empleado.Foto = @"\imagenes\empleados\" + nombreArchivo + extension; 
                    
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "No se a seleccionado ninguna imagen");
                    empleado.Foto = "";
                }

                _dbContext.Empleado.Add(empleado);  //añademos los valores 
                await _dbContext.SaveChangesAsync(); //guardamos los valores
                TempData["mensaje"] = "Empleado creado con éxitos.";
                return RedirectToAction(nameof(Index)); //redireccionamos al index
            }
            return View();
        }


        [HttpGet]
        public IActionResult Edit(int? id) // pasamos el id del empleado que editaremos//
        {
            //validamos que el id no sea nulo//
            if (id == null)
            {
                //si es nulo el id renornamos no encontrado//
                return NotFound();
            }

            var empleado = _dbContext.Empleado.Find(id);

            //ahora validamos si el empleado es nulo, sino lo es no entra al if//
            if (empleado == null)
            {
                return NotFound();
            }

            return View(empleado); //le pasamos los datos del empleado para mostrarlos//
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Empleado empleado)
        {
            // validamos con el if si el modelo es valido
            if (ModelState.IsValid)
            {
                // para las imagenes
                string rutaPrincipal = _hostingEnvironment.WebRootPath;
                var archivos = HttpContext.Request.Form.Files;

                var fotoDesdeDb = await _dbContext.Empleado.AsNoTracking().FirstAsync(e =>e.Id == empleado.Id);
                
                //este if sirve para x si crearon empleado sin foto al estar nulo permita tener un valor y asi poder editar ya que la base no deja
               /* if (fotoDesdeDb.Foto == null)
                {
                    fotoDesdeDb.Foto = "";
                }*/

                if (archivos.Count > 0)
                {
                    

                    //editamos  (Guid - permite guardar un archivo o variable con nombre muy grande //
                    string nombreArchivo = Guid.NewGuid().ToString();
                    var subidas = Path.Combine(rutaPrincipal, @"imagenes\empleados");
                    var nuevaExtension = Path.GetExtension(archivos[0].FileName);
                    var rutaImagen = Path.Combine(rutaPrincipal, fotoDesdeDb.Foto.TrimStart('\\'));

                    //validamos si la imagen existe, borramos el archivo para luego sustituir x nuevo//
                    if (System.IO.File.Exists(rutaImagen))
                    {
                        System.IO.File.Delete(rutaImagen);
                    }

                    //subimos nuevamente el archivo
                    using (var fileStreams = new FileStream(Path.Combine(subidas, nombreArchivo + nuevaExtension), FileMode.Create))
                    {
                        archivos[0].CopyTo(fileStreams);
                    }

                    empleado.Foto = @"\imagenes\empleados\" + nombreArchivo + nuevaExtension;

                    //_dbContext.Entry(empleado).State = EntityState.Modified;
                    _dbContext.Update(empleado);
                    await _dbContext.SaveChangesAsync();
                    TempData["mensaje"] = "Modificado con exito.";
                    return RedirectToAction(nameof(Index));
                }
                //aqui cuando la imagen existe y no se reemplaza//
                //debe conservar la que ya existe
                else
                {
                    empleado.Foto = fotoDesdeDb.Foto;
                }


                //_dbContext.Entry(empleado).State = EntityState.Modified; //otra forma de update
                _dbContext.Update(empleado);  //añademos los valores 
                await _dbContext.SaveChangesAsync(); //guardamos los valores
                TempData["mensaje"] = "Modificado con exito."; //mensaje
                return RedirectToAction(nameof(Index)); //redireccionamos al index
            }
            return View(empleado); 
        }


        [HttpGet]
        public IActionResult Details(int? id) // pasamos el id del empleado que mostrarlo//
        {
            //validamos que el id no sea nulo//
            if (id == null)
            {
                //si es nulo el id renornamos no encontrado//
                return NotFound();
            }

            var empleado = _dbContext.Empleado.Find(id);

            //ahora validamos si el empleado es nulo, sino lo es no entra al if//
            if (empleado == null)
            {
                return NotFound();
            }

            return View(empleado); //le pasamos los datos del empleado para mostrarlos//
        }

        [HttpGet]
        public IActionResult Delete(int? id) // pasamos el id del empleado que editaremos//
        {
            //validamos que el id no sea nulo//
            if (id == null)
            {
                //si es nulo el id renornamos no encontrado//
                return NotFound();
            }

            var empleado = _dbContext.Empleado.Find(id);

            //ahora validamos si el empleado es nulo, sino lo es no entra al if//
            if (empleado == null)
            {
                return NotFound();
            }
            
            return View(empleado); //le pasamos los datos del empleado para mostrarlos//
        }


        [HttpDelete]
        [HttpPost, ActionName("Delete")] //indicamos que aunq se llame diferente es un Iaction delete//
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRegistro(int? id) //le cambiamos el nombre porq pasamos los mismos valor id y no se puede repetir//
        {
            string rutaDirectorioPrincipal = _hostingEnvironment.WebRootPath;
            var empleadoDesdeDb = await _dbContext.Empleado.FindAsync(id); //que nos busque el usuario por el id

            //este if sirve para x si el empleado esta sin foto y pueda tener un valor y asi poder borrar y que la db no deja
            /*if (empleadoDesdeDb.Foto == null)
            {
                empleadoDesdeDb.Foto = "";
            }*/

            //optener la ruta guardada en la base de datos//
            var rutaImagen = Path.Combine(rutaDirectorioPrincipal, empleadoDesdeDb.Foto.TrimStart('\\')); //trimstar elimina  los '\' que se guardan en la base//

            if (System.IO.File.Exists(rutaImagen)) //si exite imagen que borre
            {
                System.IO.File.Delete(rutaImagen);
            }

            if (empleadoDesdeDb == null)  //si es nulo nos retorna la misma vista
            {
                return View();
            }
          
            _dbContext.Empleado.Remove(empleadoDesdeDb);  //removemos los valores 
            await _dbContext.SaveChangesAsync(); //guardamos los valores
            TempData["mensaje"] = "Borrado con éxitos.";

            return RedirectToAction(nameof(Index)); //redireccionamos al index                
            
        }


       
    }
}
