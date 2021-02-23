using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Template.Model.Types;

namespace Template.Model.Maps
{
    public class StudentMap : EntityTypeConfiguration<Student>
    {
        public StudentMap()
        {
            
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.FullName)
                .IsRequired()
                .HasMaxLength(50);

            Property(t => t.TimeStamp).IsConcurrencyToken()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed);
        }
    }
}
