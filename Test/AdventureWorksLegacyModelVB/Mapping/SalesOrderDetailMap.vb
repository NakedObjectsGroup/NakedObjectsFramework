Imports System.ComponentModel.DataAnnotations.Schema
Imports System.Data.Entity.ModelConfiguration

Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders

Namespace AW.Mapping
	Public Class SalesOrderDetailMap
		Inherits EntityTypeConfiguration(Of SalesOrderDetail)

		Public Sub New()
			' Primary Key
			HasKey(Function(t) New With {
				Key t.SalesOrderID,
				Key t.SalesOrderDetailID
			})

			'Ignores
			Ignore(Function(t) t.Product)

			' Properties
			[Property](Function(t) t.SalesOrderID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)

			[Property](Function(t) t.SalesOrderDetailID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)

			[Property](Function(t) t.CarrierTrackingNumber).HasMaxLength(25)

			' Table & Column Mappings
			ToTable("SalesOrderDetail", "Sales")
			[Property](Function(t) t.SalesOrderID).HasColumnName("SalesOrderID")
			[Property](Function(t) t.SalesOrderDetailID).HasColumnName("SalesOrderDetailID")
			[Property](Function(t) t.CarrierTrackingNumber).HasColumnName("CarrierTrackingNumber")
			[Property](Function(t) t.OrderQty).HasColumnName("OrderQty")
			[Property](Function(t) t.ProductID).HasColumnName("ProductID")
			[Property](Function(t) t.SpecialOfferID).HasColumnName("SpecialOfferID")
			[Property](Function(t) t.UnitPrice).HasColumnName("UnitPrice")
			[Property](Function(t) t.UnitPriceDiscount).HasColumnName("UnitPriceDiscount")
			[Property](Function(t) t.LineTotal).HasColumnName("LineTotal").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed)
			[Property](Function(t) t.rowguid).HasColumnName("rowguid")
			[Property](Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();

			' Relationships
			HasRequired(Function(t) t.SalesOrderHeader).WithMany(Function(t) t.Details).HasForeignKey(Function(d) d.SalesOrderID)
			HasRequired(Function(t) t.SpecialOfferProduct).WithMany().HasForeignKey(Function(t) New With {
				Key t.SpecialOfferID,
				Key t.ProductID
			})
		End Sub
	End Class

	Public Module Mapper
		<System.Runtime.CompilerServices.Extension> _
		Public Sub Map(ByVal builder As EntityTypeBuilder(Of SalesOrderDetail))
			builder.HasKey(Function(t) New With {
				Key t.SalesOrderID,
				Key t.SalesOrderDetailID
			})

			'Ignores
			builder.Ignore(Function(t) t.Product)

			' Properties
			builder.Property(Function(t) t.SalesOrderID).ValueGeneratedNever()

			builder.Property(Function(t) t.SalesOrderDetailID).ValueGeneratedOnAdd()

			builder.Property(Function(t) t.CarrierTrackingNumber).HasMaxLength(25)

			' Table & Column Mappings
			builder.ToTable("SalesOrderDetail", "Sales")
			builder.Property(Function(t) t.SalesOrderID).HasColumnName("SalesOrderID")
			builder.Property(Function(t) t.SalesOrderDetailID).HasColumnName("SalesOrderDetailID")
			builder.Property(Function(t) t.CarrierTrackingNumber).HasColumnName("CarrierTrackingNumber")
			builder.Property(Function(t) t.OrderQty).HasColumnName("OrderQty")
			builder.Property(Function(t) t.ProductID).HasColumnName("ProductID")
			builder.Property(Function(t) t.SpecialOfferID).HasColumnName("SpecialOfferID")
			builder.Property(Function(t) t.UnitPrice).HasColumnName("UnitPrice")
			builder.Property(Function(t) t.UnitPriceDiscount).HasColumnName("UnitPriceDiscount")
			builder.Property(Function(t) t.LineTotal).HasColumnName("LineTotal").ValueGeneratedOnAddOrUpdate()
			builder.Property(Function(t) t.rowguid).HasColumnName("rowguid")
			builder.Property(Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();

			' Relationships
			builder.HasOne(Function(t) t.SalesOrderHeader).WithMany(Function(t) t.Details).HasForeignKey(Function(d) d.SalesOrderID)
			builder.HasOne(Function(t) t.SpecialOfferProduct).WithMany().HasForeignKey(Function(t) New With {
				Key t.SpecialOfferID,
				Key t.ProductID
			})
		End Sub
	End Module
End Namespace