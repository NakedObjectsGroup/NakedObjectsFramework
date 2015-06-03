using System.Data.Entity.ModelConfiguration;

namespace AdventureWorksModel
{
    public class ContactMap : EntityTypeConfiguration<Contact>
    {
        public ContactMap()
        {
            // Primary Key
            HasKey(t => t.ContactID);

            //Ignores
            Ignore(t => t.InitialPassword);
            Ignore(t => t.ContactType);
            Ignore(t => t.Contactee);

            // Properties
            Property(t => t.Title)
                .HasMaxLength(8);

            Property(t => t.FirstName)
                .IsRequired()
                .HasMaxLength(50);

            Property(t => t.MiddleName)
                .HasMaxLength(50);

            Property(t => t.LastName)
                .IsRequired()
                .HasMaxLength(50);

            Property(t => t.Suffix)
                .HasMaxLength(10);

            Property(t => t.EmailAddress)
                .HasMaxLength(50);

            Property(t => t.Phone)
                .HasMaxLength(25);

            Property(t => t.PasswordHash)
                .IsRequired()
                .HasMaxLength(128);

            Property(t => t.PasswordSalt)
                .IsRequired()
                .HasMaxLength(10);

            // Table & Column Mappings
            ToTable("Contact", "Person");
            Property(t => t.ContactID).HasColumnName("ContactID");
            Property(t => t.NameStyle).HasColumnName("NameStyle");
            Property(t => t.Title).HasColumnName("Title");
            Property(t => t.FirstName).HasColumnName("FirstName");
            Property(t => t.MiddleName).HasColumnName("MiddleName");
            Property(t => t.LastName).HasColumnName("LastName");
            Property(t => t.Suffix).HasColumnName("Suffix");
            Property(t => t.EmailAddress).HasColumnName("EmailAddress");
            Property(t => t.EmailPromotion).HasColumnName("EmailPromotion");
            Property(t => t.Phone).HasColumnName("Phone");
            Property(t => t.PasswordHash).HasColumnName("PasswordHash");
            Property(t => t.PasswordSalt).HasColumnName("PasswordSalt");
            Property(t => t.AdditionalContactInfo).HasColumnName("AdditionalContactInfo");
            Property(t => t.rowguid).HasColumnName("rowguid");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");
        }
    }
}
