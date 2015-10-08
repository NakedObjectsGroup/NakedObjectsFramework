using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.Infrastructure;

namespace AdventureWorksModel
{
    public class BusinessEntityContactMap : EntityTypeConfiguration<BusinessEntityContact>
    {
        public BusinessEntityContactMap()
        {
            // Primary Key
            HasKey(t => new { t.BusinessEntityID, t.PersonID, t.ContactTypeID });

            // Table & Column Mappings
            ToTable("BusinessEntityContact", "Person");
        }
    }
}