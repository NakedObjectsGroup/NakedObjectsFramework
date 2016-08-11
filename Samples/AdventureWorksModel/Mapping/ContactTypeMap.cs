using System.Data.Entity.ModelConfiguration;

namespace AdventureWorksModel
{
    public class ContactTypeMap : EntityTypeConfiguration<ContactType>
    {
        public ContactTypeMap()
        {
            // Primary Key
            HasKey(t => t.ContactTypeID);

            // Properties
            Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            ToTable("ContactType", "Person");
            Property(t => t.ContactTypeID).HasColumnName("ContactTypeID");
            Property(t => t.Name).HasColumnName("Name");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);
        }
    }
}
