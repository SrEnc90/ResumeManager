using MasterDetailCRUD.Data;
using MasterDetailCRUD.Models;
using Microsoft.AspNetCore.Mvc;

using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

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
        public ActionResult<List<Applicant>> Index()
        {
            List<Applicant> applicants;
            applicants = _context.Applicants.ToList();
            return View(applicants);
        }

        [HttpGet]
        public ActionResult Create()
        {
            Applicant applicant = new Applicant();

            applicant.Experiences.Add(new Experience() { ExperienceId = 1 });
            //applicant.Experiences.Add(new Experience() { ExperienceId = 2 });
            //applicant.Experiences.Add(new Experience() { ExperienceId = 3 });

            return View(applicant);
        }

        [HttpPost]
        public async Task<ActionResult> Create(Applicant applicant)
        {
            applicant.Experiences.RemoveAll(e => e.YearsWorked == 0); // Para que al agregar una nueva fila en la tabla experiencia y al no llenar nada me la elimine

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
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    applicant.ProfilePhoto.CopyTo(fileStream);
                }
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
    }
}
