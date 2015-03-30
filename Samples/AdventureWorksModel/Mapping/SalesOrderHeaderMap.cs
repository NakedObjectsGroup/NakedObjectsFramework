using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AdventureWorksModel
{
    public class SalesOrderHeaderMap : EntityTypeConfiguration<SalesOrderHeader>
    {
        public SalesOrderHeaderMap()
        {
            // Primary Key
            this.HasKey(t => t.SalesOrderID);

            // Properties
            this.Property(t => t.SalesOrderNumber)
                .IsRequired()
                .HasMaxLength(25);

            this.Property(t => t.PurchaseOrderNumber)
                .HasMaxLength(25);

            this.Property(t => t.AccountNumber)
                .HasMaxLength(15);

            this.Property(t => t.CreditCardApprovalCode)
                .HasMaxLength(15);

            this.Property(t => t.Comment)
                .HasMaxLength(128);

            // Table & Column Mappings
            this.ToTable("SalesOrderHeader", "Sales");
            this.Property(t => t.SalesOrderID).HasColumnName("SalesOrderID");
            this.Property(t => t.RevisionNumber).HasColumnName("RevisionNumber");
            this.Property(t => t.OrderDate).HasColumnName("OrderDate");
            this.Property(t => t.DueDate).HasColumnName("DueDate");
            this.Property(t => t.ShipDate).HasColumnName("ShipDate");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.OnlineOrder).HasColumnName("OnlineOrderFlag");
            this.Property(t => t.SalesOrderNumber).HasColumnName("SalesOrderNumber");
            this.Property(t => t.PurchaseOrderNumber).HasColumnName("PurchaseOrderNumber");
            this.Property(t => t.AccountNumber).HasColumnName("AccountNumber");
            this.Property(t => t.CustomerID).HasColumnName("CustomerID");
            this.Property(t => t.ContactID).HasColumnName("ContactID");
            this.Property(t => t.SalesPersonID).HasColumnName("SalesPersonID");
            this.Property(t => t.SalesTerritoryID).HasColumnName("TerritoryID");
            this.Property(t => t.BillingAddressID).HasColumnName("BillToAddressID");
            this.Property(t => t.ShippingAddressID).HasColumnName("ShipToAddressID");
            this.Property(t => t.ShipMethodID).HasColumnName("ShipMethodID");
            this.Property(t => t.CreditCardID).HasColumnName("CreditCardID");
            this.Property(t => t.CreditCardApprovalCode).HasColumnName("CreditCardApprovalCode");
            this.Property(t => t.CurrencyRateID).HasColumnName("CurrencyRateID");
            this.Property(t => t.SubTotal).HasColumnName("SubTotal");
            this.Property(t => t.TaxAmt).HasColumnName("TaxAmt");
            this.Property(t => t.Freight).HasColumnName("Freight");
            this.Property(t => t.TotalDue).HasColumnName("TotalDue");
            this.Property(t => t.Comment).HasColumnName("Comment");
            this.Property(t => t.rowguid).HasColumnName("rowguid");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");

            // Relationships
            this.HasRequired(t => t.BillingAddress);
            this.HasRequired(t => t.ShippingAddress);
            this.HasRequired(t => t.Contact);
            this.HasRequired(t => t.ShipMethod);
            this.HasOptional(t => t.CreditCard);
            this.HasOptional(t => t.CurrencyRate);
            this.HasRequired(t => t.Customer);
            this.HasOptional(t => t.SalesPerson);
            this.HasOptional(t => t.SalesTerritory);

        }
    }
}
