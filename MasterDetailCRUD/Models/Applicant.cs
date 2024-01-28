using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MasterDetailCRUD.Models
{
    public class Applicant
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Este Campo es necesario")]
        [StringLength(150)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Este Campo es necesario")]
        [StringLength(10)]
        public string Gender { get; set; }

        [Required(ErrorMessage = "Este campo es necesario")]
        [Range(25, 55, ErrorMessage = "Actualmente, no temenos vacantes para tu edad")]
        [DisplayName("Edad en años")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Este campo es necesario")]
        [StringLength(50)]
        public string Qualification { get; set; }


        [Required(ErrorMessage = "Este campo es necesario")]
        [Range(1, 25, ErrorMessage = " Actualemnte, no tenemos vacantes para tu experiencia")]
        [DisplayName("Total de experiencia en años")]
        public int TotalExperiencie { get; set; }

        //public virtual List<Experience> Experiences { get; set; } = new List<Experience>(); //Detalle de experiencias
        public List<Experience> Experiences { get; set; }

        [FileExtensions(Extensions = "jpg, jpeg, png, gif", ErrorMessage = "No tiene el formato correcto")]
        public string PhotoUrl { get; set; }

        [Required(ErrorMessage = "Este campo es necesario")]
        [DisplayName("Elegir la foto de perfil")]
        [NotMapped]
        public IFormFile ProfilePhoto { get; set; }
    }
}
