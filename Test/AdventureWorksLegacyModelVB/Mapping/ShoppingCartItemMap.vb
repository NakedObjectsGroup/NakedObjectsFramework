Imports System.Data.Entity.ModelConfiguration

Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders

Namespace AW.Mapping
	Public Class ShoppingCartItemMap
		Inherits EntityTypeConfiguration(Of ShoppingCartItem)

		Public Sub New()
			' Primary Key
			HasKey(Function(t) t.ShoppingCartItemID)

			' Properties
			[Property](Function(t) t.ShoppingCartID).IsRequired().HasMaxLength(50)

			' Table & Column Mappings
			ToTable("ShoppingCartItem", "Sales")
			[Property](Function(t) t.ShoppingCartItemID).HasColumnName("ShoppingCartItemID")
			[Property](Function(t) t.ShoppingCartID).HasColumnName("ShoppingCartID")
			[Property](Function(t) t.Quantity).HasColumnName("Quantity")
			[Property](Function(t) t.ProductID).HasColumnName("ProductID")
			[Property](Function(t) t.DateCreated).HasColumnName("DateCreated")
			[Property](Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();

			' Relationships
			HasRequired(Function(t) t.Product).WithMany().HasForeignKey(Function(t) t.ProductID)
		End Sub
	End Class

	Public Module Mapper
		<System.Runtime.CompilerServices.Extension> _
		Public Sub Map(ByVal builder As EntityTypeBuilder(Of ShoppingCartItem))
			builder.HasKey(Function(t) t.ShoppingCartItemID)

			' Properties
			builder.Property(Function(t) t.ShoppingCartID).IsRequired().HasMaxLength(50)

			' Table & Column Mappings
			builder.ToTable("ShoppingCartItem", "Sales")
			builder.Property(Function(t) t.ShoppingCartItemID).HasColumnName("ShoppingCartItemID")
			builder.Property(Function(t) t.ShoppingCartID).HasColumnName("ShoppingCartID")
			builder.Property(Function(t) t.Quantity).HasColumnName("Quantity")
			builder.Property(Function(t) t.ProductID).HasColumnName("ProductID")
			builder.Property(Function(t) t.DateCreated).HasColumnName("DateCreated")
			builder.Property(Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();

			' Relationships
			builder.HasOne(Function(t) t.Product).WithMany().HasForeignKey(Function(t) t.ProductID)
		End Sub
	End Module
End Namespace