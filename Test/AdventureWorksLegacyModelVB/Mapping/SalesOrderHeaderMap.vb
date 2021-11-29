Imports System.ComponentModel.DataAnnotations.Schema
Imports System.Data.Entity.ModelConfiguration

Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders

Namespace AW.Mapping
	Public Class SalesOrderHeaderMap
		Inherits EntityTypeConfiguration(Of SalesOrderHeader)

		Public Sub New()
			' Primary Key
			HasKey(Function(t) t.SalesOrderID)

			'Ignores
			Ignore(Function(t) t.AddItemsFromCart)
			'Ignore(t => t.Status); //TODO is it necessary to ignore this?

			' Properties
			[Property](Function(t) t.SalesOrderNumber).IsRequired().HasMaxLength(25).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed)

			[Property](Function(t) t.PurchaseOrderNumber).HasMaxLength(25)

			[Property](Function(t) t.AccountNumber).HasMaxLength(15)

			[Property](Function(t) t.CreditCardApprovalCode).HasMaxLength(15)

			[Property](Function(t) t.Comment).HasMaxLength(128)

			' Table & Column Mappings
			ToTable("SalesOrderHeader", "Sales")
			[Property](Function(t) t.SalesOrderID).HasColumnName("SalesOrderID")
			[Property](Function(t) t.RevisionNumber).HasColumnName("RevisionNumber")
			[Property](Function(t) t.OrderDate).HasColumnName("OrderDate")
			[Property](Function(t) t.DueDate).HasColumnName("DueDate")
			[Property](Function(t) t.ShipDate).HasColumnName("ShipDate")
			[Property](Function(t) t.StatusByte).HasColumnName("Status")
			[Property](Function(t) t.OnlineOrder).HasColumnName("OnlineOrderFlag")
			[Property](Function(t) t.SalesOrderNumber).HasColumnName("SalesOrderNumber")
			[Property](Function(t) t.PurchaseOrderNumber).HasColumnName("PurchaseOrderNumber")
			[Property](Function(t) t.AccountNumber).HasColumnName("AccountNumber")
			[Property](Function(t) t.CustomerID).HasColumnName("CustomerID")
			[Property](Function(t) t.SalesPersonID).HasColumnName("SalesPersonID")
			[Property](Function(t) t.SalesTerritoryID).HasColumnName("TerritoryID")
			[Property](Function(t) t.BillingAddressID).HasColumnName("BillToAddressID")
			[Property](Function(t) t.ShippingAddressID).HasColumnName("ShipToAddressID")
			[Property](Function(t) t.ShipMethodID).HasColumnName("ShipMethodID")
			[Property](Function(t) t.CreditCardID).HasColumnName("CreditCardID")
			[Property](Function(t) t.CreditCardApprovalCode).HasColumnName("CreditCardApprovalCode")
			[Property](Function(t) t.CurrencyRateID).HasColumnName("CurrencyRateID")
			[Property](Function(t) t.SubTotal).HasColumnName("SubTotal")
			[Property](Function(t) t.TaxAmt).HasColumnName("TaxAmt")
			[Property](Function(t) t.Freight).HasColumnName("Freight")
			[Property](Function(t) t.TotalDue).HasColumnName("TotalDue").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed)

			[Property](Function(t) t.Comment).HasColumnName("Comment")
			[Property](Function(t) t.rowguid).HasColumnName("rowguid")
			[Property](Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();

			' Relationships
			HasRequired(Function(t) t.BillingAddress).WithMany().HasForeignKey(Function(t) t.BillingAddressID)
			HasRequired(Function(t) t.ShippingAddress).WithMany().HasForeignKey(Function(t) t.ShippingAddressID)
			HasRequired(Function(t) t.ShipMethod).WithMany().HasForeignKey(Function(t) t.ShipMethodID)
			HasOptional(Function(t) t.CreditCard).WithMany().HasForeignKey(Function(t) t.CreditCardID)
			HasOptional(Function(t) t.CurrencyRate).WithMany().HasForeignKey(Function(t) t.CurrencyRateID)
			HasRequired(Function(t) t.Customer).WithMany().HasForeignKey(Function(t) t.CustomerID)
			HasOptional(Function(t) t.SalesPerson).WithMany().HasForeignKey(Function(t) t.SalesPersonID)
			HasOptional(Function(t) t.SalesTerritory).WithMany().HasForeignKey(Function(t) t.SalesTerritoryID)
		End Sub
	End Class

	Public Module Mapper
		<System.Runtime.CompilerServices.Extension> _
		Public Sub Map(ByVal builder As EntityTypeBuilder(Of SalesOrderHeader))
			builder.HasKey(Function(t) t.SalesOrderID)

			'Ignores
			builder.Ignore(Function(t) t.AddItemsFromCart)
			'Ignore(t => t.Status); //TODO is it necessary to ignore this?

			' Properties
			builder.Property(Function(t) t.SalesOrderNumber).IsRequired().HasMaxLength(25).ValueGeneratedOnAddOrUpdate()

			builder.Property(Function(t) t.PurchaseOrderNumber).HasMaxLength(25)

			builder.Property(Function(t) t.AccountNumber).HasMaxLength(15)

			builder.Property(Function(t) t.CreditCardApprovalCode).HasMaxLength(15)

			builder.Property(Function(t) t.Comment).HasMaxLength(128)

			' Table & Column Mappings
			builder.ToTable("SalesOrderHeader", "Sales")
			builder.Property(Function(t) t.SalesOrderID).HasColumnName("SalesOrderID")
			builder.Property(Function(t) t.RevisionNumber).HasColumnName("RevisionNumber")
			builder.Property(Function(t) t.OrderDate).HasColumnName("OrderDate")
			builder.Property(Function(t) t.DueDate).HasColumnName("DueDate")
			builder.Property(Function(t) t.ShipDate).HasColumnName("ShipDate")
			builder.Property(Function(t) t.StatusByte).HasColumnName("Status")
			builder.Property(Function(t) t.OnlineOrder).HasColumnName("OnlineOrderFlag")
			builder.Property(Function(t) t.SalesOrderNumber).HasColumnName("SalesOrderNumber")
			builder.Property(Function(t) t.PurchaseOrderNumber).HasColumnName("PurchaseOrderNumber")
			builder.Property(Function(t) t.AccountNumber).HasColumnName("AccountNumber")
			builder.Property(Function(t) t.CustomerID).HasColumnName("CustomerID")
			builder.Property(Function(t) t.SalesPersonID).HasColumnName("SalesPersonID")
			builder.Property(Function(t) t.SalesTerritoryID).HasColumnName("TerritoryID")
			builder.Property(Function(t) t.BillingAddressID).HasColumnName("BillToAddressID")
			builder.Property(Function(t) t.ShippingAddressID).HasColumnName("ShipToAddressID")
			builder.Property(Function(t) t.ShipMethodID).HasColumnName("ShipMethodID")
			builder.Property(Function(t) t.CreditCardID).HasColumnName("CreditCardID")
			builder.Property(Function(t) t.CreditCardApprovalCode).HasColumnName("CreditCardApprovalCode")
			builder.Property(Function(t) t.CurrencyRateID).HasColumnName("CurrencyRateID")
			builder.Property(Function(t) t.SubTotal).HasColumnName("SubTotal")
			builder.Property(Function(t) t.TaxAmt).HasColumnName("TaxAmt")
			builder.Property(Function(t) t.Freight).HasColumnName("Freight")
			builder.Property(Function(t) t.TotalDue).HasColumnName("TotalDue").ValueGeneratedOnAddOrUpdate()
			builder.Property(Function(t) t.Comment).HasColumnName("Comment")
			builder.Property(Function(t) t.rowguid).HasColumnName("rowguid")
			builder.Property(Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();

			' Relationships
			builder.HasOne(Function(t) t.BillingAddress).WithMany().HasForeignKey(Function(t) t.BillingAddressID)
			builder.HasOne(Function(t) t.ShippingAddress).WithMany().HasForeignKey(Function(t) t.ShippingAddressID)
			builder.HasOne(Function(t) t.ShipMethod).WithMany().HasForeignKey(Function(t) t.ShipMethodID)
			builder.HasOne(Function(t) t.CreditCard).WithMany().HasForeignKey(Function(t) t.CreditCardID)
			builder.HasOne(Function(t) t.CurrencyRate).WithMany().HasForeignKey(Function(t) t.CurrencyRateID)
			builder.HasOne(Function(t) t.Customer).WithMany().HasForeignKey(Function(t) t.CustomerID)
			builder.HasOne(Function(t) t.SalesPerson).WithMany().HasForeignKey(Function(t) t.SalesPersonID)
			builder.HasOne(Function(t) t.SalesTerritory).WithMany().HasForeignKey(Function(t) t.SalesTerritoryID)
		End Sub
	End Module
End Namespace