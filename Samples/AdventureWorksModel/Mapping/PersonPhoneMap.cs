using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.Infrastructure;

namespace AdventureWorksModel {
    public class PersonPhoneMap : EntityTypeConfiguration<PersonPhone> {
        public PersonPhoneMap() {
            // Primary Key
            HasKey(t => new { t.BusinessEntityID, t.PhoneNumber, t.PhoneNumberTypeID});

            // Table & Column Mappings
            this.ToTable("PersonPhone", "Person");
        }
    }
}