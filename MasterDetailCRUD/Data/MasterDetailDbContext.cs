using MasterDetailCRUD.Models;
using Microsoft.EntityFrameworkCore;

namespace MasterDetailCRUD.Data
{
    public class MasterDetailDbContext : DbContext
    {
        public  DbSet<Applicant> Applicants { get; set; }
        public  DbSet<Experience> Experiences { get; set; }

        public DbSet<Softwares> Softwares { get; set; }

        public DbSet<SoftwareExperience> SoftwareExperiences { get; set; }

        /***************************Api Afluente o También llamado Afluent Api ****************************/
        //Colocar override onmodel + tab para que se autocomplete
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Experience>().HasOne(x => x.Applicant); // .HasRequired(x => x.Applicant) -> Está deprecado
            
            base.OnModelCreating(modelBuilder);
        }

        public MasterDetailDbContext(DbContextOptions<MasterDetailDbContext> options) : base(options) 
        {
        }

    }
}
