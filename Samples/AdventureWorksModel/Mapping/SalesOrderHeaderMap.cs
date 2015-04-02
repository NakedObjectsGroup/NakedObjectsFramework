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

            //Ignores
            this.Ignore(t => t.AddItemsFromCart);

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
            this.HasRequired(t => t.BillingAddress).WithMany().HasForeignKey(t => t.BillingAddressID);
            this.HasRequired(t => t.ShippingAddress).WithMany().HasForeignKey(t => t.ShippingAddressID);
            this.HasRequired(t => t.Contact).WithMany().HasForeignKey(t => t.ContactID);
            this.HasRequired(t => t.ShipMethod).WithMany().HasForeignKey(t => t.ShipMethodID);
            this.HasOptional(t => t.CreditCard).WithMany().HasForeignKey(t => t.CreditCardID);
            this.HasOptional(t => t.CurrencyRate).WithMany().HasForeignKey(t => t.CurrencyRateID);
            this.HasRequired(t => t.Customer).WithMany().HasForeignKey(t => t.CustomerID);
            this.HasOptional(t => t.SalesPerson).WithMany().HasForeignKey(t => t.SalesPersonID);
            this.HasOptional(t => t.SalesTerritory).WithMany().HasForeignKey(t => t.SalesTerritoryID);

        }
    }
}
