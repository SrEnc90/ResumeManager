using System.ComponentModel.DataAnnotations;

namespace MasterDetailCRUD.Models
{
    public class Softwares
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Name { get; set; } = "";
    }
}
