Imports System.Data.Entity.ModelConfiguration

Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders

Namespace AW.Mapping
	Public Class CountryRegionCurrencyMap
		Inherits EntityTypeConfiguration(Of CountryRegionCurrency)

		Public Sub New()
			' Primary Key
			HasKey(Function(t) New With {
				Key t.CountryRegionCode,
				Key t.CurrencyCode
			})

			' Properties
			[Property](Function(t) t.CountryRegionCode).IsRequired().HasMaxLength(3)

			[Property](Function(t) t.CurrencyCode).IsRequired().IsFixedLength().HasMaxLength(3)

			' Table & Column Mappings
			ToTable("CountryRegionCurrency", "Sales")
			[Property](Function(t) t.CountryRegionCode).HasColumnName("CountryRegionCode")
			[Property](Function(t) t.CurrencyCode).HasColumnName("CurrencyCode")
			[Property](Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();

			' Relationships
			HasRequired(Function(t) t.CountryRegion).WithMany().HasForeignKey(Function(t) t.CountryRegionCode)
			HasRequired(Function(t) t.Currency).WithMany().HasForeignKey(Function(t) t.CurrencyCode)
		End Sub
	End Class

	Public Module Mapper
		<System.Runtime.CompilerServices.Extension> _
		Public Sub Map(ByVal builder As EntityTypeBuilder(Of CountryRegionCurrency))
			' Primary Key
			builder.HasKey(Function(t) New With {
				Key t.CountryRegionCode,
				Key t.CurrencyCode
			})

			' Properties
			builder.Property(Function(t) t.CountryRegionCode).IsRequired().HasMaxLength(3)

			builder.Property(Function(t) t.CurrencyCode).IsRequired().IsFixedLength().HasMaxLength(3)

			' Table & Column Mappings
			builder.ToTable("CountryRegionCurrency", "Sales")
			builder.Property(Function(t) t.CountryRegionCode).HasColumnName("CountryRegionCode")
			builder.Property(Function(t) t.CurrencyCode).HasColumnName("CurrencyCode")
			builder.Property(Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();

			' Relationships
			builder.HasOne(Function(t) t.CountryRegion).WithMany().HasForeignKey(Function(t) t.CountryRegionCode)
			builder.HasOne(Function(t) t.Currency).WithMany().HasForeignKey(Function(t) t.CurrencyCode)
		End Sub
	End Module
End Namespace