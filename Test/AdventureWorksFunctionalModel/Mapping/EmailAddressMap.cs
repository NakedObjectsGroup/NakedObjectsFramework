using System.Data.Entity.ModelConfiguration;
using AW.Types;

namespace AW.Mapping {
    public class EmailAddressMap : EntityTypeConfiguration<EmailAddress> {
        public EmailAddressMap() {
            // Primary Key
            HasKey(t => new { t.BusinessEntityID, t.EmailAddressID});

            // Table & Column Mappings
            this.ToTable("EmailAddress", "Person");
            Property(t => t.EmailAddress1).HasColumnName("EmailAddress");
        }
    }
}