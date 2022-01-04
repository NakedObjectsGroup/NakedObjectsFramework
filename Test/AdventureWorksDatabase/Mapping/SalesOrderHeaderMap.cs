using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdventureWorksModel
{
    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<SalesOrderHeader> builder)
        {
            builder.HasKey(t => t.SalesOrderID);

            //Ignores
            //builder.Ignore(t => t.AddItemsFromCart);
            //Ignore(t => t.Status); //TODO is it necessary to ignore this?


            // Properties
            builder.Property(t => t.mappedSalesOrderNumber)
                .IsRequired()
                .HasMaxLength(25).ValueGeneratedOnAddOrUpdate();

            builder.Property(t => t.mappedPurchaseOrderNumber)
                .HasMaxLength(25);

            builder.Property(t => t.mappedAccountNumber)
                .HasMaxLength(15);

            builder.Property(t => t.mappedCreditCardApprovalCode)
                .HasMaxLength(15);

            builder.Property(t => t.mappedComment)
                .HasMaxLength(128);

            // Table & Column Mappings
            builder.ToTable("SalesOrderHeader", "Sales");
            builder.Property(t => t.SalesOrderID).HasColumnName("SalesOrderID");
            builder.Property(t => t.mappedRevisionNumber).HasColumnName("RevisionNumber");
            builder.Property(t => t.mappedOrderDate).HasColumnName("OrderDate");
            builder.Property(t => t.mappedDueDate).HasColumnName("DueDate");
            builder.Property(t => t.mappedShipDate).HasColumnName("ShipDate");
            //builder.Property(t => t.StatusByte).HasColumnName("Status");
            builder.Property(t => t.mappedOnlineOrder).HasColumnName("OnlineOrderFlag");
            builder.Property(t => t.mappedSalesOrderNumber).HasColumnName("SalesOrderNumber");
            builder.Property(t => t.mappedPurchaseOrderNumber).HasColumnName("PurchaseOrderNumber");
            builder.Property(t => t.mappedAccountNumber).HasColumnName("AccountNumber");
            builder.Property(t => t.CustomerID).HasColumnName("CustomerID");
            builder.Property(t => t.SalesPersonID).HasColumnName("SalesPersonID");
            builder.Property(t => t.SalesTerritoryID).HasColumnName("TerritoryID");
            builder.Property(t => t.BillingAddressID).HasColumnName("BillToAddressID");
            builder.Property(t => t.ShippingAddressID).HasColumnName("ShipToAddressID");
            builder.Property(t => t.ShipMethodID).HasColumnName("ShipMethodID");
            builder.Property(t => t.CreditCardID).HasColumnName("CreditCardID");
            builder.Property(t => t.mappedCreditCardApprovalCode).HasColumnName("CreditCardApprovalCode");
            builder.Property(t => t.CurrencyRateID).HasColumnName("CurrencyRateID");
            builder.Property(t => t.mappedSubTotal).HasColumnName("SubTotal");
            builder.Property(t => t.mappedTaxAmt).HasColumnName("TaxAmt");
            builder.Property(t => t.mappedFreight).HasColumnName("Freight");
            builder.Property(t => t.mappedTotalDue).HasColumnName("TotalDue").ValueGeneratedOnAddOrUpdate();
            builder.Property(t => t.mappedComment).HasColumnName("Comment");
            builder.Property(t => t.RowGuid).HasColumnName("rowguid");
            builder.Property(t => t.mappedModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);

            // Relationships
            builder.HasOne(t => t.BillingAddress).WithMany().HasForeignKey(t => t.BillingAddressID);
            builder.HasOne(t => t.ShippingAddress).WithMany().HasForeignKey(t => t.ShippingAddressID);
            builder.HasOne(t => t.ShipMethod).WithMany().HasForeignKey(t => t.ShipMethodID);
            builder.HasOne(t => t.CreditCard).WithMany().HasForeignKey(t => t.CreditCardID);
            builder.HasOne(t => t.CurrencyRate).WithMany().HasForeignKey(t => t.CurrencyRateID);
            builder.HasOne(t => t.Customer).WithMany().HasForeignKey(t => t.CustomerID);
            builder.HasOne(t => t.SalesPerson).WithMany().HasForeignKey(t => t.SalesPersonID);
            builder.HasOne(t => t.SalesTerritory).WithMany().HasForeignKey(t => t.SalesTerritoryID);
        }
    }
}
