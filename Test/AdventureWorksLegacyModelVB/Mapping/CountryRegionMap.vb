Imports System.Data.Entity.ModelConfiguration

Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders

Namespace AW.Mapping
	Public Class CountryRegionMap
		Inherits EntityTypeConfiguration(Of CountryRegion)

		Public Sub New()
			' Primary Key
			HasKey(Function(t) t.CountryRegionCode)

			' Properties
			[Property](Function(t) t.CountryRegionCode).IsRequired().HasMaxLength(3)

			[Property](Function(t) t.Name).IsRequired().HasMaxLength(50)

			' Table & Column Mappings
			ToTable("CountryRegion", "Person")
			[Property](Function(t) t.CountryRegionCode).HasColumnName("CountryRegionCode")
			[Property](Function(t) t.Name).HasColumnName("Name")
			[Property](Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();
		End Sub
	End Class

	Public Module Mapper
		<System.Runtime.CompilerServices.Extension> _
		Public Sub Map(ByVal builder As EntityTypeBuilder(Of CountryRegion))
			builder.HasKey(Function(t) t.CountryRegionCode)

			' Properties
			builder.Property(Function(t) t.CountryRegionCode).IsRequired().HasMaxLength(3)

			builder.Property(Function(t) t.Name).IsRequired().HasMaxLength(50)

			' Table & Column Mappings
			builder.ToTable("CountryRegion", "Person")
			builder.Property(Function(t) t.CountryRegionCode).HasColumnName("CountryRegionCode")
			builder.Property(Function(t) t.Name).HasColumnName("Name")
			builder.Property(Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();
		End Sub
	End Module
End Namespace