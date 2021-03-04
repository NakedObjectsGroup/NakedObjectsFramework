using System.Data.Entity;

namespace SchoolRecords.Model
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(string dbName) : base(dbName)
        {
            Database.SetInitializer(new Initializer());
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<TeachingSet> Sets { get; set; }
        public DbSet<SubjectReport> SubjectReports { get; set; }
    }
}
