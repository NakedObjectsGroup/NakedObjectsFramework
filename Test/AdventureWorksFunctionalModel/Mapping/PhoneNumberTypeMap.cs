using System.Data.Entity.ModelConfiguration;
using AW.Types;

namespace AW.Mapping {
    public class PhoneNumberTypeMap : EntityTypeConfiguration<PhoneNumberType> {
        public PhoneNumberTypeMap() {
            // Primary Key
            HasKey(t => t.PhoneNumberTypeID);

            // Table & Column Mappings
            this.ToTable("PhoneNumberType", "Person");
        }
    }
}