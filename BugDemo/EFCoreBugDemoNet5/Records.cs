using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace EFCoreBugDemoNet5
{
    public record BaseRecord
    {
        [Key]
        public int Id { get; set; }
    }

    public record SubRecord : BaseRecord
    {
        public string Name { get; set; }
    }

    public class RecordContext : DbContext {
        public DbSet<SubRecord> SubRecords { get; set; }
     
        public RecordContext()
        {
         
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=EFCoreBugDemoNet5;Integrated Security=True;");
            options.UseLazyLoadingProxies();
        }
    }
}
