Imports System.ComponentModel.DataAnnotations.Schema
Imports System.Data.Entity.ModelConfiguration

Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders

Namespace AW.Mapping
	Public Class SpecialOfferProductMap
		Inherits EntityTypeConfiguration(Of SpecialOfferProduct)

		Public Sub New()
			' Primary Key
			HasKey(Function(t) New With {
				Key t.SpecialOfferID,
				Key t.ProductID
			})

			' Properties
			[Property](Function(t) t.SpecialOfferID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)

			[Property](Function(t) t.ProductID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)

			' Table & Column Mappings
			ToTable("SpecialOfferProduct", "Sales")
			[Property](Function(t) t.SpecialOfferID).HasColumnName("SpecialOfferID")
			[Property](Function(t) t.ProductID).HasColumnName("ProductID")
			[Property](Function(t) t.rowguid).HasColumnName("rowguid")
			[Property](Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();

			' Relationships
			HasRequired(Function(t) t.Product).WithMany(Function(t) t.SpecialOfferProduct).HasForeignKey(Function(d) d.ProductID)
			HasRequired(Function(t) t.SpecialOffer).WithMany().HasForeignKey(Function(t) t.SpecialOfferID)
		End Sub
	End Class

	Public Module Mapper
		<System.Runtime.CompilerServices.Extension> _
		Public Sub Map(ByVal builder As EntityTypeBuilder(Of SpecialOfferProduct))
			builder.HasKey(Function(t) New With {
				Key t.SpecialOfferID,
				Key t.ProductID
			})

			' Properties
			builder.Property(Function(t) t.SpecialOfferID).ValueGeneratedNever()

			builder.Property(Function(t) t.ProductID).ValueGeneratedNever()

			' Table & Column Mappings
			builder.ToTable("SpecialOfferProduct", "Sales")
			builder.Property(Function(t) t.SpecialOfferID).HasColumnName("SpecialOfferID")
			builder.Property(Function(t) t.ProductID).HasColumnName("ProductID")
			builder.Property(Function(t) t.rowguid).HasColumnName("rowguid")
			builder.Property(Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();

			' Relationships
			builder.HasOne(Function(t) t.Product).WithMany(Function(t) t.SpecialOfferProduct).HasForeignKey(Function(d) d.ProductID)
			builder.HasOne(Function(t) t.SpecialOffer).WithMany().HasForeignKey(Function(t) t.SpecialOfferID)
		End Sub
	End Module
End Namespace