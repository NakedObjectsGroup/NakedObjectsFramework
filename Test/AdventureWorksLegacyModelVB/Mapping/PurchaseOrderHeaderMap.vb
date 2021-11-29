Imports System.ComponentModel.DataAnnotations.Schema
Imports System.Data.Entity.ModelConfiguration

Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders

Namespace AW.Mapping
	Public Class PurchaseOrderHeaderMap
		Inherits EntityTypeConfiguration(Of PurchaseOrderHeader)

		Public Sub New()
			' Primary Key
			HasKey(Function(t) t.PurchaseOrderID)

			' Properties
			' Table & Column Mappings
			ToTable("PurchaseOrderHeader", "Purchasing")
			[Property](Function(t) t.PurchaseOrderID).HasColumnName("PurchaseOrderID")
			[Property](Function(t) t.RevisionNumber).HasColumnName("RevisionNumber")
			[Property](Function(t) t.Status).HasColumnName("Status")
			[Property](Function(t) t.OrderPlacedByID).HasColumnName("EmployeeID")
			[Property](Function(t) t.VendorID).HasColumnName("VendorID")
			[Property](Function(t) t.ShipMethodID).HasColumnName("ShipMethodID")
			[Property](Function(t) t.OrderDate).HasColumnName("OrderDate")
			[Property](Function(t) t.ShipDate).HasColumnName("ShipDate")
			[Property](Function(t) t.SubTotal).HasColumnName("SubTotal")
			[Property](Function(t) t.TaxAmt).HasColumnName("TaxAmt")
			[Property](Function(t) t.Freight).HasColumnName("Freight")
			[Property](Function(t) t.TotalDue).HasColumnName("TotalDue").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed)
			[Property](Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();

			' Relationships
			HasRequired(Function(t) t.OrderPlacedBy).WithMany().HasForeignKey(Function(t) t.OrderPlacedByID)
			HasRequired(Function(t) t.ShipMethod).WithMany().HasForeignKey(Function(t) t.ShipMethodID)
			HasRequired(Function(t) t.Vendor).WithMany().HasForeignKey(Function(t) t.VendorID)
		End Sub
	End Class

	Public Module Mapper
		<System.Runtime.CompilerServices.Extension> _
		Public Sub Map(ByVal builder As EntityTypeBuilder(Of PurchaseOrderHeader))
			builder.HasKey(Function(t) t.PurchaseOrderID)

			' Properties
			' Table & Column Mappings
			builder.ToTable("PurchaseOrderHeader", "Purchasing")
			builder.Property(Function(t) t.PurchaseOrderID).HasColumnName("PurchaseOrderID")
			builder.Property(Function(t) t.RevisionNumber).HasColumnName("RevisionNumber")
			builder.Property(Function(t) t.Status).HasColumnName("Status")
			builder.Property(Function(t) t.OrderPlacedByID).HasColumnName("EmployeeID")
			builder.Property(Function(t) t.VendorID).HasColumnName("VendorID")
			builder.Property(Function(t) t.ShipMethodID).HasColumnName("ShipMethodID")
			builder.Property(Function(t) t.OrderDate).HasColumnName("OrderDate")
			builder.Property(Function(t) t.ShipDate).HasColumnName("ShipDate")
			builder.Property(Function(t) t.SubTotal).HasColumnName("SubTotal")
			builder.Property(Function(t) t.TaxAmt).HasColumnName("TaxAmt")
			builder.Property(Function(t) t.Freight).HasColumnName("Freight")
			builder.Property(Function(t) t.TotalDue).HasColumnName("TotalDue").ValueGeneratedOnAddOrUpdate()
			builder.Property(Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();

			' Relationships
			builder.HasOne(Function(t) t.OrderPlacedBy).WithMany().HasForeignKey(Function(t) t.OrderPlacedByID)
			builder.HasOne(Function(t) t.ShipMethod).WithMany().HasForeignKey(Function(t) t.ShipMethodID)
			builder.HasOne(Function(t) t.Vendor).WithMany().HasForeignKey(Function(t) t.VendorID)
		End Sub
	End Module
End Namespace