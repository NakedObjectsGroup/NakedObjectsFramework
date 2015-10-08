using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.Infrastructure;

namespace AdventureWorksModel {
    public class PhoneNumberTypeMap : EntityTypeConfiguration<PhoneNumberType> {
        public PhoneNumberTypeMap() {
            // Primary Key
            HasKey(t => t.PhoneNumberTypeID);

            // Table & Column Mappings
            this.ToTable("PhoneNumberType", "Person");
        }
    }
}