using System.Data.Entity.ModelConfiguration;

namespace AdventureWorksModel
{
    public class IndividualMap : EntityTypeConfiguration<Individual>
    {
        public IndividualMap()
        {
            // Table & Column Mappings
            ToTable("Individual", "Sales");
            Property(t => t.ContactID).HasColumnName("ContactID");
            Property(t => t.Demographics).HasColumnName("Demographics");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");

            // Relationships
            HasRequired(t => t.Contact);
        }
    }
}
