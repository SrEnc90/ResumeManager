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
            ViewBag.Gender = GetGender();

            applicant.SoftwareExperiences = new List<SoftwareExperience>() { new SoftwareExperience { Id = 1 } };
            ViewBag.Rating = GetRating();
            ViewBag.Softwares = GetSoftware();

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
                    .Include(a => a.SoftwareExperiences)
                    .FirstOrDefaultAsync(a => a.Id == id);

            ViewBag.Rating = GetRating();
            ViewBag.Softwares = GetSoftware();

            return View(applicant);
        }

        [HttpGet]
        public async Task<ActionResult<Applicant>> Edit(int Id)
        {
            var existe = await _context.Applicants.AnyAsync(x => x.Id == Id);
            if (!existe) return NotFound($"No existe Applicant con el id: {Id}");

            Applicant applicant = await _context.Applicants
                .Include(a => a.Experiences)
                .Include(a=>a.SoftwareExperiences)
                .FirstOrDefaultAsync(x => x.Id == Id);

            ViewBag.Gender = GetGender();
            ViewBag.Rating = GetRating();
            ViewBag.Softwares = GetSoftware();

            if (applicant.Experiences.Count == 0)
            {
                applicant.Experiences = new List<Experience>() { new Experience() { ExperienceId = 1 } };
            }

            return View(applicant);
        }

        [HttpPost]
        public async Task<ActionResult<Applicant>> Edit(Applicant applicant)
        {
            //Lo que hago me traigo todo el listado de experiencias y software de ese applicant mediante id, de ahí las elimino para volver a insertarlas con los realizados por el usuario
            List<Experience> experiences = await _context.Experiences.Where(x => x.ApplicationId == applicant.Id).ToListAsync();
            _context.RemoveRange(experiences);
            await _context.SaveChangesAsync();

            List<SoftwareExperience> softwareExperiences = await _context.SoftwareExperiences.Where(d => d.ApplicantId == applicant.Id).ToListAsync();
            _context.RemoveRange(softwareExperiences);
            await _context.SaveChangesAsync();

            applicant.Experiences.RemoveAll(e => e.YearsWorked == 0);
            applicant.Experiences.RemoveAll(e => e.IsDeleted == true);//Para arreglar el problema del model binding
            applicant.SoftwareExperiences.RemoveAll(e => e.IsHidden == true);

            if (applicant.ProfilePhoto != null)
            {
                string uniqueFileName = GetUploadFileName(applicant);
                applicant.PhotoUrl = uniqueFileName;
            }
            _context.Attach(applicant);
            _context.Entry(applicant).State=EntityState.Modified; //Es importante está línea de código para que nuevamente el ORM EF core que haga seguimiento a applicant ya que en la línea 146 hemos colocado el SaveChangesAsync(), y necesitamos indicarle que vamos hacer más cambios
            
            await _context.Experiences.AddRangeAsync(applicant.Experiences);
            await _context.SoftwareExperiences.AddRangeAsync(applicant.SoftwareExperiences);

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }


        [HttpGet]
        public async Task<ActionResult> Delete(int id)
        {
            var existe = await _context.Applicants.AnyAsync(x => x.Id == id);
            if (!existe) return NotFound($"No existe el Applicant con id: {id}");

            Applicant applicant = await _context.Applicants.Include(x => x.Experiences).Include(e => e.SoftwareExperiences).FirstOrDefaultAsync(x => x.Id == id);

            ViewBag.Rating = GetRating();
            ViewBag.Softwares = GetSoftware();

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

        private List<SelectListItem> GetRating()
        {
            List<SelectListItem> selRating = new List<SelectListItem>();

            var selItem = new SelectListItem() { Value = "0", Text = "Select Rating" };
            selRating.Insert(0, selItem);

            for (int i = 0; i < 11; i++)
            {
                selItem = new SelectListItem()
                {
                    Value = i.ToString(),
                    Text = i.ToString()
                };
                selRating.Add(selItem);
            }
            return selRating;
        }

        private async Task<List<SelectListItem>> GetSoftware()
        {
            List<SelectListItem> selSoftware = await _context.Softwares
                .OrderBy(n => n.Name)
                .Select(n =>
                new SelectListItem()
                {
                    Value = n.Id.ToString(),
                    Text = n.Name
                }).ToListAsync();

            var selItem = new SelectListItem()
            {
                Value = null,
                Text = "Select Software"
            };

            selSoftware.Insert(0, selItem);

            return selSoftware;
        }
    }
}
