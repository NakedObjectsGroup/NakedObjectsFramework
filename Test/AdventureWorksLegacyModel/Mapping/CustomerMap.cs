using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using AdventureWorksLegacyModel.Sales;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdventureWorksLegacyModel.Mapping
{
    public class CustomerMap : EntityTypeConfiguration<Customer>
    {
        public CustomerMap()
        {
            // Primary Key
            HasKey(t => t.CustomerID);

            //Ignores
            Ignore(t => t.CustomerType);

            // Properties
            Property(t => t.AccountNumber)
                .IsRequired()
                .HasMaxLength(10)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed);

            Property(t => t.StoreID).IsOptional();
            Property(t => t.PersonID).IsOptional();
            Property(t => t.SalesTerritoryID).IsOptional();

            // Table & Column Mappings
            ToTable("Customer", "Sales");
            Property(t => t.CustomerID).HasColumnName("CustomerID");
            Property(t => t.SalesTerritoryID).HasColumnName("TerritoryID");
            Property(t => t.StoreID).HasColumnName("StoreID");
            Property(t => t.PersonID).HasColumnName("PersonID");
            Property(t => t.AccountNumber).HasColumnName("AccountNumber");
            Property(t => t.CustomerRowguid).HasColumnName("rowguid");
          Property(t => t.CustomerModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);

          // Relationships
          HasOptional(t => t.SalesTerritory).WithMany().HasForeignKey(t => t.SalesTerritoryID);
          HasOptional(t => t.Store).WithMany().HasForeignKey(t => t.StoreID);
          HasOptional(t => t.Person).WithMany().HasForeignKey(t => t.PersonID);
        }
    }

    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<Customer> builder)
        {
            builder.HasKey(t => t.CustomerID);

            builder.Ignore(t => t.CustomerType);

            // Properties
            builder.Property(t => t.AccountNumber)
                   .IsRequired()
                   .HasMaxLength(10)
                   .ValueGeneratedOnAddOrUpdate();

            builder.Property(t => t.StoreID);//.IsOptional();
            builder.Property(t => t.PersonID);//.IsOptional();
            builder.Property(t => t.SalesTerritoryID);//.IsOptional();

            // Table & Column Mappings
            builder.ToTable("Customer", "Sales");
            builder.Property(t => t.CustomerID).HasColumnName("CustomerID");
            builder.Property(t => t.SalesTerritoryID).HasColumnName("TerritoryID");
            builder.Property(t => t.StoreID).HasColumnName("StoreID");
            builder.Property(t => t.PersonID).HasColumnName("PersonID");
            builder.Property(t => t.AccountNumber).HasColumnName("AccountNumber");
            builder.Property(t => t.CustomerRowguid).HasColumnName("rowguid");
            builder.Property(t => t.CustomerModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);

            // Relationships
            builder.HasOne(t => t.SalesTerritory).WithMany().HasForeignKey(t => t.SalesTerritoryID);
            builder.HasOne(t => t.Store).WithMany().HasForeignKey(t => t.StoreID);
            builder.HasOne(t => t.Person).WithMany().HasForeignKey(t => t.PersonID);
        }
    }
}
