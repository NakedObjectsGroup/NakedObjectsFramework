using System.Data.Entity.ModelConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdventureWorksModel {
    public class PhoneNumberTypeMap : EntityTypeConfiguration<PhoneNumberType> {
        public PhoneNumberTypeMap() {
            // Primary Key
            HasKey(t => t.PhoneNumberTypeID);

            // Table & Column Mappings
            this.ToTable("PhoneNumberType", "Person");
        }
    }

    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<PhoneNumberType> builder)
        {
            builder.HasKey(t => t.PhoneNumberTypeID);

            // Table & Column Mappings
            builder.ToTable("PhoneNumberType", "Person");
        }
    }
}