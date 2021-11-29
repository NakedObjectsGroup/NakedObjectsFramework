Imports System.Data.Entity.ModelConfiguration

Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders

Namespace AW.Mapping
	Public Class SpecialOfferMap
		Inherits EntityTypeConfiguration(Of SpecialOffer)

		Public Sub New()
			' Primary Key
			HasKey(Function(t) t.SpecialOfferID)

			' Properties
			[Property](Function(t) t.Description).IsRequired().HasMaxLength(255)

			[Property](Function(t) t.Type).IsRequired().HasMaxLength(50)

			[Property](Function(t) t.Category).IsRequired().HasMaxLength(50)

			' Table & Column Mappings
			ToTable("SpecialOffer", "Sales")
			[Property](Function(t) t.SpecialOfferID).HasColumnName("SpecialOfferID")
			[Property](Function(t) t.Description).HasColumnName("Description")
			[Property](Function(t) t.DiscountPct).HasColumnName("DiscountPct")
			[Property](Function(t) t.Type).HasColumnName("Type")
			[Property](Function(t) t.Category).HasColumnName("Category")
			[Property](Function(t) t.StartDate).HasColumnName("StartDate")
			[Property](Function(t) t.EndDate).HasColumnName("EndDate")
			[Property](Function(t) t.MinQty).HasColumnName("MinQty")
			[Property](Function(t) t.MaxQty).HasColumnName("MaxQty")
			[Property](Function(t) t.rowguid).HasColumnName("rowguid")
			[Property](Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();
		End Sub
	End Class

	Public Module Mapper
		<System.Runtime.CompilerServices.Extension> _
		Public Sub Map(ByVal builder As EntityTypeBuilder(Of SpecialOffer))
			builder.HasKey(Function(t) t.SpecialOfferID)

			' Properties
			builder.Property(Function(t) t.Description).IsRequired().HasMaxLength(255)

			builder.Property(Function(t) t.Type).IsRequired().HasMaxLength(50)

			builder.Property(Function(t) t.Category).IsRequired().HasMaxLength(50)

			' Table & Column Mappings
			builder.ToTable("SpecialOffer", "Sales")
			builder.Property(Function(t) t.SpecialOfferID).HasColumnName("SpecialOfferID")
			builder.Property(Function(t) t.Description).HasColumnName("Description")
			builder.Property(Function(t) t.DiscountPct).HasColumnName("DiscountPct")
			builder.Property(Function(t) t.Type).HasColumnName("Type")
			builder.Property(Function(t) t.Category).HasColumnName("Category")
			builder.Property(Function(t) t.StartDate).HasColumnName("StartDate")
			builder.Property(Function(t) t.EndDate).HasColumnName("EndDate")
			builder.Property(Function(t) t.MinQty).HasColumnName("MinQty")
			builder.Property(Function(t) t.MaxQty).HasColumnName("MaxQty")
			builder.Property(Function(t) t.rowguid).HasColumnName("rowguid")
			builder.Property(Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();
		End Sub
	End Module
End Namespace