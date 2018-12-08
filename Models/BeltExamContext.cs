using Microsoft.EntityFrameworkCore;
 
namespace BeltExam.Models
{
    public class BeltExamContext : DbContext
    {
        public DbSet<User> Users {get;set;}
        public DbSet<Activity> Activities {get;set;}
        public DbSet<Participant> Participants {get;set;}
        // base() calls the parent class' constructor passing the "options" parameter along
        public BeltExamContext(DbContextOptions<BeltExamContext> options) : base(options) { }
    }
}