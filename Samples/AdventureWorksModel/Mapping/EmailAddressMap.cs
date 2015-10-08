using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.Infrastructure;

namespace AdventureWorksModel {
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