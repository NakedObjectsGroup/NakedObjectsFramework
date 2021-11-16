using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace EFCoreBugDemoNet6
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
            options.UseSqlServer(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=EFCoreBugDemoNet6;Integrated Security=True;");
            options.UseLazyLoadingProxies();
        }
    }
}
