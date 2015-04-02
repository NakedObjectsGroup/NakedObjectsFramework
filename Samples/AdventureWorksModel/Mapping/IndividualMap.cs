using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AdventureWorksModel
{
    public class IndividualMap : EntityTypeConfiguration<Individual>
    {
        public IndividualMap()
        {
            // Table & Column Mappings
            this.ToTable("Individual", "Sales");
            this.Property(t => t.ContactID).HasColumnName("ContactID");
            this.Property(t => t.Demographics).HasColumnName("Demographics");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");

            // Relationships
            this.HasRequired(t => t.Contact);
        }
    }
}
