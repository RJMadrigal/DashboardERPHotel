using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using web_hoteldemo.Data;
using web_hoteldemo.Models;
using web_hoteldemo.Models.DB;
using web_hoteldemo.Services;

namespace web_hoteldemo.Controllers
{
    
    public class InventarioController : Controller

    {
        private readonly db_adminHotelContext _context;
    

        public InventarioController(db_adminHotelContext context)
        {
            _context = context;
  
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> IndexAdmin()
        {
            return _context.Inventario != null ?
                        View(await _context.Inventario.ToListAsync()) :
                        Problem("Entity set 'db_adminHotelContext.Inventario'  is null.");
        }

        [HttpGet]
        public JsonResult ListarInventario()
        {
            List<Inventario> oLista = new List<Inventario>();
            try
            {
                // Llama al método Listar
                oLista = InventarioData.Instancia.Listar().OrderBy(i => i.inventarioID).ToList();

                // Agrega un mensaje de registro para verificar si se cargaron los datos correctamente
                //System.Console.WriteLine("Se cargaron " + oLista.Count + " productos correctamente.");


            }
            catch (Exception ex)
            {
                // Si ocurre un error, registra el mensaje de error
                System.Console.WriteLine("Error al cargar datos de inventario: " + ex.Message);
            }

            // Devuelve los datos como JSON
            return Json(new { data = oLista });
        }


        public IActionResult RegistrarProducto()
        {
            return View();
        }


        //public IActionResult EditView(int inventarioID)
        //{
        //    Inventario inventario = _context.Inventario.Where(i => i.inventarioID == inventarioID).FirstOrDefault();
        //    if (inventario == null)
        //    {
        //        return NotFound(); // O manejo de error apropiado
        //    }
        //    return View(inventario);
        //}


        // Acción para obtener la información del producto seleccionado
        [HttpGet]
        public IActionResult GetProductInfo(string producto)
        {
            var productoSeleccionado = _context.Inventario.FirstOrDefault(p => p.producto == producto);
            return Json(productoSeleccionado);
        }


        // GET: Usuarios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Inventario == null)
            {
                return NotFound();
            }

            var inventario = await _context.Inventario
                .FirstOrDefaultAsync(i => i.inventarioID == id);
            if (inventario == null)
            {
                return NotFound();
            }

            return View(inventario);
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Inventario == null)
            {
                return Problem("Entity set 'db_adminHotelContext.Inventario'  is null.");
            }
            var inventario = await _context.Inventario.FindAsync(id);
            if (inventario != null)
            {
                _context.Inventario.Remove(inventario);
                // Actualizar los registros en bitacora con el usuarioID del usuario eliminado a null y establecer el comentario
                string comentario = $"Inventario eliminado - ID: {id}";
              //  await _context.Database.ExecuteSqlInterpolatedAsync($"UPDATE bitacora SET usuarioID = NULL, comentario = {comentario} WHERE usuarioID = {id}");

            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(RegistrarProducto));
        }

        public IActionResult Editar()
        {
            ViewBag.Productos = _context.Inventario.Select(p => p.producto).ToList();

            return View();  
        }
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit( [Bind("inventarioID,producto,cantidadDisponible")] Inventario inventario)
        //{
            

        //    if (ModelState.IsValid)
        //    {
                
                
        //            _context.Update(inventario);
        //            await _context.SaveChangesAsync();
        //            return RedirectToAction(nameof(RegistrarProducto));    
                
               
        //    }
        //    return View(inventario);
        //}


        // GET: Usuarios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Inventario == null)
            {
                return NotFound();
            }

            var inventario = await _context.Inventario.FindAsync(id);
            if (inventario == null)
            {
                return NotFound();
            }
            return View(inventario);
        }

        // POST: Usuarios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("inventarioID,producto,cantidadDisponible")] Inventario inventario)
        {
            if (id != inventario.inventarioID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    
                    _context.Update(inventario);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InventarioExists(inventario.inventarioID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(RegistrarProducto));
            }
            return View(inventario);
        }





        private bool InventarioExists(int id)
        {
            return (_context.Inventario?.Any(i => i.inventarioID == id)).GetValueOrDefault();
        }


        //[HttpPost]
        //public IActionResult Edit(Inventario inventario)
        //{
        //    _context.Inventario.Update(inventario);
        //    _context.SaveChanges();

        //    return RedirectToAction("RegistrarProducto", "Inventario");
        //}



        public IActionResult Create()
        {
            return View();
        }


       


        [HttpPost]
        public IActionResult Create(Inventario inventario)
        {
            Console.WriteLine("InventarioID: " + inventario.inventarioID);
            Console.WriteLine("Producto: " + inventario.producto);
            Console.WriteLine("Cantidad Disponible: " + inventario.cantidadDisponible);

            if (ModelState.IsValid)
            {
                _context.Inventario.Add(inventario);
                _context.SaveChanges();
                return RedirectToAction("RegistrarProducto", "Inventario");
            }
            return View(inventario);
        }

    }
}
