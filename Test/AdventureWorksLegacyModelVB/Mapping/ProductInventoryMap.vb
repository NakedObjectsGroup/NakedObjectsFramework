Imports System.ComponentModel.DataAnnotations.Schema
Imports System.Data.Entity.ModelConfiguration

Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders

Namespace AW.Mapping
	Public Class ProductInventoryMap
		Inherits EntityTypeConfiguration(Of ProductInventory)

		Public Sub New()
			' Primary Key
			HasKey(Function(t) New With {
				Key t.ProductID,
				Key t.LocationID
			})

			' Properties
			[Property](Function(t) t.ProductID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)

			[Property](Function(t) t.LocationID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)

			[Property](Function(t) t.Shelf).IsRequired().HasMaxLength(10)

			' Table & Column Mappings
			ToTable("ProductInventory", "Production")
			[Property](Function(t) t.ProductID).HasColumnName("ProductID")
			[Property](Function(t) t.LocationID).HasColumnName("LocationID")
			[Property](Function(t) t.Shelf).HasColumnName("Shelf")
			[Property](Function(t) t.Bin).HasColumnName("Bin")
			[Property](Function(t) t.Quantity).HasColumnName("Quantity")
			[Property](Function(t) t.rowguid).HasColumnName("rowguid")
			[Property](Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();

			' Relationships
			HasRequired(Function(t) t.Location).WithMany().HasForeignKey(Function(t) t.LocationID)
			HasRequired(Function(t) t.Product).WithMany(Function(t) t.ProductInventory).HasForeignKey(Function(d) d.ProductID)
		End Sub
	End Class

	Public Module Mapper
		<System.Runtime.CompilerServices.Extension> _
		Public Sub Map(ByVal builder As EntityTypeBuilder(Of ProductInventory))
			builder.HasKey(Function(t) New With {
				Key t.ProductID,
				Key t.LocationID
			})

			' Properties
			builder.Property(Function(t) t.ProductID).ValueGeneratedNever()

			builder.Property(Function(t) t.LocationID).ValueGeneratedNever()

			builder.Property(Function(t) t.Shelf).IsRequired().HasMaxLength(10)

			' Table & Column Mappings
			builder.ToTable("ProductInventory", "Production")
			builder.Property(Function(t) t.ProductID).HasColumnName("ProductID")
			builder.Property(Function(t) t.LocationID).HasColumnName("LocationID")
			builder.Property(Function(t) t.Shelf).HasColumnName("Shelf")
			builder.Property(Function(t) t.Bin).HasColumnName("Bin")
			builder.Property(Function(t) t.Quantity).HasColumnName("Quantity")
			builder.Property(Function(t) t.rowguid).HasColumnName("rowguid")
			builder.Property(Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();

			' Relationships
			builder.HasOne(Function(t) t.Location).WithMany().HasForeignKey(Function(t) t.LocationID)
			builder.HasOne(Function(t) t.Product).WithMany(Function(t) t.ProductInventory).HasForeignKey(Function(d) d.ProductID)
		End Sub
	End Module
End Namespace