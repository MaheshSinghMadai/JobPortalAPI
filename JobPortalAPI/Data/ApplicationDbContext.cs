using JobPortalAPI.Entity;
using Microsoft.EntityFrameworkCore;

namespace JobPortalAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }

        public DbSet<Candidate> Candidates { get; set; }
    }
}
