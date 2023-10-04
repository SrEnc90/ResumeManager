using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MasterDetailCRUD.Models
{
    public class Experience
    {
        public Experience()
        {
        }

        [Key]
        public int ExperienceId { get; set; }

        [ForeignKey("Applicant")]
        public int ApplicationId { get; set; }
        public virtual Applicant Applicant { get; private set; } //Propiedad muy importante

        [Required(ErrorMessage = "Este campo es necesario")]
        public string CompanyName { get; set; }

        [Required(ErrorMessage = "Este campo es necesario")]
        public string Designation { get; set; }

        [Range(1,25, ErrorMessage = "Debe tener entre 1 a 25 años de experiencia")]
        [Required(ErrorMessage = "Este campo es necesario")]
        public int YearsWorked { get; set; }

    }
}
