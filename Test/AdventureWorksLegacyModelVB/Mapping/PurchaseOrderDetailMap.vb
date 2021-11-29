Imports System.ComponentModel.DataAnnotations.Schema
Imports System.Data.Entity.ModelConfiguration

Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders

Namespace AW.Mapping
	Public Class PurchaseOrderDetailMap
		Inherits EntityTypeConfiguration(Of PurchaseOrderDetail)

		Public Sub New()
			' Primary Key
			HasKey(Function(t) New With {
				Key t.PurchaseOrderID,
				Key t.PurchaseOrderDetailID
			})

			' Properties
			[Property](Function(t) t.PurchaseOrderID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)

			[Property](Function(t) t.PurchaseOrderDetailID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)

			' Table & Column Mappings
			ToTable("PurchaseOrderDetail", "Purchasing")
			[Property](Function(t) t.PurchaseOrderID).HasColumnName("PurchaseOrderID")
			[Property](Function(t) t.PurchaseOrderDetailID).HasColumnName("PurchaseOrderDetailID")
			[Property](Function(t) t.DueDate).HasColumnName("DueDate")
			[Property](Function(t) t.OrderQty).HasColumnName("OrderQty")
			[Property](Function(t) t.ProductID).HasColumnName("ProductID")
			[Property](Function(t) t.UnitPrice).HasColumnName("UnitPrice")
			[Property](Function(t) t.LineTotal).HasColumnName("LineTotal").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed)
			[Property](Function(t) t.ReceivedQty).HasColumnName("ReceivedQty")
			[Property](Function(t) t.RejectedQty).HasColumnName("RejectedQty")
			[Property](Function(t) t.StockedQty).HasColumnName("StockedQty").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed)
			[Property](Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();

			' Relationships
			HasRequired(Function(t) t.Product).WithMany().HasForeignKey(Function(t) t.ProductID)
			HasRequired(Function(t) t.PurchaseOrderHeader).WithMany(Function(t) t.Details).HasForeignKey(Function(d) d.PurchaseOrderID)
		End Sub
	End Class

	Public Module Mapper
		<System.Runtime.CompilerServices.Extension> _
		Public Sub Map(ByVal builder As EntityTypeBuilder(Of PurchaseOrderDetail))
			builder.HasKey(Function(t) New With {
				Key t.PurchaseOrderID,
				Key t.PurchaseOrderDetailID
			})

			' Properties
			builder.Property(Function(t) t.PurchaseOrderID).ValueGeneratedNever()

			builder.Property(Function(t) t.PurchaseOrderDetailID).ValueGeneratedOnAdd()

			' Table & Column Mappings
			builder.ToTable("PurchaseOrderDetail", "Purchasing")
			builder.Property(Function(t) t.PurchaseOrderID).HasColumnName("PurchaseOrderID")
			builder.Property(Function(t) t.PurchaseOrderDetailID).HasColumnName("PurchaseOrderDetailID")
			builder.Property(Function(t) t.DueDate).HasColumnName("DueDate")
			builder.Property(Function(t) t.OrderQty).HasColumnName("OrderQty")
			builder.Property(Function(t) t.ProductID).HasColumnName("ProductID")
			builder.Property(Function(t) t.UnitPrice).HasColumnName("UnitPrice")
			builder.Property(Function(t) t.LineTotal).HasColumnName("LineTotal").ValueGeneratedOnAddOrUpdate()
			builder.Property(Function(t) t.ReceivedQty).HasColumnName("ReceivedQty")
			builder.Property(Function(t) t.RejectedQty).HasColumnName("RejectedQty")
			builder.Property(Function(t) t.StockedQty).HasColumnName("StockedQty").ValueGeneratedOnAddOrUpdate()
			builder.Property(Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();

			' Relationships
			builder.HasOne(Function(t) t.Product).WithMany().HasForeignKey(Function(t) t.ProductID)
			builder.HasOne(Function(t) t.PurchaseOrderHeader).WithMany(Function(t) t.Details).HasForeignKey(Function(d) d.PurchaseOrderID)
		End Sub
	End Module
End Namespace