using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace apiLibreria.Controllers
{
    public class LibrosController : ApiController
    {
        [HttpGet]
        public IHttpActionResult Get()
        {
            List<ViewModels.LibrosViewModel> list = new List<ViewModels.LibrosViewModel>();

            using (Models.LibreriaEntities db = new Models.LibreriaEntities())
            {
                list = (from l in db.Libros
                        select new ViewModels.LibrosViewModel
                        {
                            Id = l.Id,
                            ISBN = l.ISBN,
                            Titulo = l.Titulo,
                            Autor = l.Autor,
                            Temas = l.Temas,
                            Editorial = l.Editorial
                        }).ToList();
            }
            return Ok(list);
        }

        [HttpGet]
        public IHttpActionResult Get(int id)
        {
            ViewModels.LibrosViewModel libro = null;
            using (Models.LibreriaEntities db = new Models.LibreriaEntities())
            {
                libro = db.Libros.Where(l => l.Id == id)
                    .Select(l => new ViewModels.LibrosViewModel()
                    {
                        Id = l.Id,
                        ISBN = l.ISBN,
                        Titulo = l.Titulo,
                        Autor = l.Autor,
                        Temas = l.Temas,
                        Editorial = l.Editorial
                    }).FirstOrDefault<ViewModels.LibrosViewModel>();
            }
            if (libro == null)
                return NotFound();
            return Ok(libro);
        }

        [HttpPost]
        public IHttpActionResult Add(ViewModels.LibrosViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest("No es un modelo válido");
            using (Models.LibreriaEntities db = new Models.LibreriaEntities())
            {
                var oLibro = new Models.Libros();
                oLibro.ISBN = model.ISBN;
                oLibro.Titulo = model.Titulo;
                oLibro.Autor = model.Autor;
                oLibro.Temas = model.Temas;
                oLibro.Editorial = model.Editorial;
                db.Libros.Add(oLibro);
                db.SaveChanges();
            }
            return Ok("Registro agregado correctamente");
        }

        [HttpPut]
        public IHttpActionResult Put(ViewModels.LibrosViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest("No es un modelo válido");
            using (Models.LibreriaEntities db = new Models.LibreriaEntities())
            {
                var oLibro = db.Libros.Where(l => l.Id == model.Id)
                    .FirstOrDefault<Models.Libros>();
                if (oLibro != null)
                {
                    oLibro.ISBN = model.ISBN;
                    oLibro.Titulo = model.Titulo;
                    oLibro.Autor = model.Autor;
                    oLibro.Temas = model.Temas;
                    oLibro.Editorial = model.Editorial;
                    db.SaveChanges();
                }
                else
                    return NotFound();
            }
            return Ok();
        }

        [HttpDelete]
        public IHttpActionResult Delete(int id)
        {
            if (id <= 0)
                return BadRequest("No es un id de libro válido");
            using (Models.LibreriaEntities db = new Models.LibreriaEntities())
            {
                var libro = db.Libros.Where(l => l.Id == id)
                    .FirstOrDefault();
                db.Entry(libro).State = System.Data.Entity.EntityState.Deleted;
                db.SaveChanges();
            }
            return Ok();
        }
    }

}
