using MasterDetailCRUD.Data;
using MasterDetailCRUD.Models;
using Microsoft.AspNetCore.Mvc;

using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MasterDetailCRUD.Controllers
{
    public class ResumeController : Controller
    {
        private readonly MasterDetailDbContext _context;
        private readonly IWebHostEnvironment _webHost;

        public ResumeController(MasterDetailDbContext context, IWebHostEnvironment webHost)
        {
            this._context = context;
            this._webHost = webHost;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Applicant>>> Index()
        {
            List<Applicant> applicants;
            applicants = await _context.Applicants.ToListAsync();
            return View(applicants);
        }

        [HttpGet]
        public ActionResult Create()
        {
            Applicant applicant = new Applicant();

            applicant.Experiences = new List<Experience>() { new Experience() { ExperienceId = 1 } };

            //applicant.Experiences.Add(new Experience() { ExperienceId = 1 });
            //applicant.Experiences.Add(new Experience() { ExperienceId = 2 });
            //applicant.Experiences.Add(new Experience() { ExperienceId = 3 });

            ViewBag.Gender = GetGender();

            return View(applicant);
        }

        [HttpPost]
        public async Task<ActionResult> Create(Applicant applicant)
        {
            applicant.Experiences.RemoveAll(e => e.YearsWorked == 0); // Para que al agregar una nueva fila en la tabla experiencia y al no llenar nada me la elimine
            applicant.Experiences.RemoveAll(e => e.IsDeleted == true);//Para arreglar el problema del model binding

            /* Más corto arriba
            foreach (Experience experience in applicant.Experiences)
            {
                if(string.IsNullOrEmpty(experience.CompanyName) || experience.CompanyName.Length == 0)
                    applicant.Experiences.Remove(experience);
            }
            */

            string uniqueFileName = GetUploadFileName(applicant);
            applicant.PhotoUrl = uniqueFileName;

            _context.Add(applicant);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private string GetUploadFileName(Applicant applicant)
        {
            string uniqueFileName = null;
            if (applicant.ProfilePhoto != null)
            {
                string uploadsFolder = Path.Combine(_webHost.WebRootPath, "image");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + applicant.ProfilePhoto.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using var fileStream = new FileStream(filePath, FileMode.Create);
                applicant.ProfilePhoto.CopyTo(fileStream);
                
            }
            return uniqueFileName;
        }

        public async Task<ActionResult<Applicant>> Details(int id)
        {
            var existe = await _context.Applicants.AnyAsync(x => x.Id == id);
            if (!existe) return NotFound($"No existe Applicant con el id: {id}");

            //if (id == 0) return NotFound($"No existe Applicant con el id: {id}");

            Applicant applicant = await _context.Applicants
                    .Include(a => a.Experiences)
                    .FirstOrDefaultAsync(a => a.Id == id);

            return View(applicant);
        }

        [HttpGet]
        public async Task<ActionResult<Applicant>> Edit(int Id)
        {
            var existe = await _context.Applicants.AnyAsync(x => x.Id == Id);
            if (!existe) return NotFound($"No existe Applicant con el id: {Id}");

            Applicant applicant = await _context.Applicants
                .Include(a => a.Experiences)
                .FirstOrDefaultAsync(x => x.Id == Id);

            ViewBag.Gender = GetGender();

            if (applicant.Experiences.Count == 0)
            {
                applicant.Experiences = new List<Experience>() { new Experience() { ExperienceId = 1 } };
            }

            return View(applicant);
        }

        [HttpPost]
        public async Task<ActionResult<Applicant>> Edit(Applicant applicant)
        {

            List<Experience> experiences = await _context.Experiences.Where(x => x.ApplicationId == applicant.Id).ToListAsync();
            _context.RemoveRange(experiences);
            await _context.SaveChangesAsync();

            applicant.Experiences.RemoveAll(e => e.YearsWorked == 0);
            applicant.Experiences.RemoveAll(e => e.IsDeleted == true);//Para arreglar el problema del model binding

            if (applicant.ProfilePhoto != null)
            {
                string uniqueFileName = GetUploadFileName(applicant);
                applicant.PhotoUrl = uniqueFileName;
            }
            _context.Attach(applicant);
             
            _context.Entry(applicant).State=EntityState.Modified; //Es importante está línea de código para que nuevamente el ORM EF core haga seguimiento a applicant ya que en la línea 118 hemos colocado el SaveChangesAsync(), y necesitamos indicarle que vamos hacer más cambios
            await _context.Experiences.AddRangeAsync(applicant.Experiences);

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }


        [HttpGet]
        public async Task<ActionResult> Delete(int id)
        {
            var existe = await _context.Applicants.AnyAsync(x => x.Id == id);
            if (!existe) return NotFound($"No existe el Applicant con id: {id}");

            Applicant applicant = await _context.Applicants.Include(x => x.Experiences).FirstOrDefaultAsync(x => x.Id == id);

            return View(applicant);
        }

        [HttpPost]
        [ActionName("Delete")]
        public async Task<ActionResult> DeletePost(int id)
        {
            var existe = await _context.Applicants.AnyAsync(x => x.Id == id);
            if (!existe) return NotFound($"No existe el Applicant con id: {id}");

            _context.Remove(new Applicant() { Id = id });
            await _context.SaveChangesAsync();

            return RedirectToAction("index");
        }

        private List<SelectListItem> GetGender()
        {

            List<SelectListItem> selGender = new List<SelectListItem>();

            //Al colocar como Value = null, no valida en el formulario que el <option> del select tenga un valor correcto, debo colocarlo cómo vacío (entre " ")
            //var selItem = new SelectListItem() { Value = null, Text = "Select Gender" };
            var selItem = new SelectListItem() { Value = "", Text = "Select Gender" };

            selGender.Insert(0, selItem);

            selItem = new SelectListItem()
            {
                Value = "Male",
                Text = "Male"
            };
            selGender.Add(selItem);

            selItem = new SelectListItem()
            {
                Value = "Female",
                Text = "Female"
            };
            selGender.Add(selItem);

            return selGender;

        }
    }
}
