Imports System.Data.Entity.ModelConfiguration

Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders

Namespace AW.Mapping
	Public Class StateProvinceMap
		Inherits EntityTypeConfiguration(Of StateProvince)

		Public Sub New()
			' Primary Key
			HasKey(Function(t) t.StateProvinceID)

			' Properties
			[Property](Function(t) t.StateProvinceCode).IsRequired().IsFixedLength().HasMaxLength(3)

			[Property](Function(t) t.CountryRegionCode).IsRequired().HasMaxLength(3)

			[Property](Function(t) t.Name).IsRequired().HasMaxLength(50)

			' Table & Column Mappings
			ToTable("StateProvince", "Person")
			[Property](Function(t) t.StateProvinceID).HasColumnName("StateProvinceID")
			[Property](Function(t) t.StateProvinceCode).HasColumnName("StateProvinceCode")
			[Property](Function(t) t.CountryRegionCode).HasColumnName("CountryRegionCode")
			[Property](Function(t) t.IsOnlyStateProvinceFlag).HasColumnName("IsOnlyStateProvinceFlag")
			[Property](Function(t) t.Name).HasColumnName("Name")
			[Property](Function(t) t.TerritoryID).HasColumnName("TerritoryID")
			[Property](Function(t) t.rowguid).HasColumnName("rowguid")
			[Property](Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();

			' Relationships
			HasRequired(Function(t) t.CountryRegion).WithMany().HasForeignKey(Function(t) t.CountryRegionCode)
			HasRequired(Function(t) t.SalesTerritory).WithMany(Function(t) t.StateProvince).HasForeignKey(Function(d) d.TerritoryID)
		End Sub
	End Class

	Public Module Mapper
		<System.Runtime.CompilerServices.Extension> _
		Public Sub Map(ByVal builder As EntityTypeBuilder(Of StateProvince))
			builder.HasKey(Function(t) t.StateProvinceID)

			' Properties
			builder.Property(Function(t) t.StateProvinceCode).IsRequired().IsFixedLength().HasMaxLength(3)

			builder.Property(Function(t) t.CountryRegionCode).IsRequired().HasMaxLength(3)

			builder.Property(Function(t) t.Name).IsRequired().HasMaxLength(50)

			' Table & Column Mappings
			builder.ToTable("StateProvince", "Person")
			builder.Property(Function(t) t.StateProvinceID).HasColumnName("StateProvinceID")
			builder.Property(Function(t) t.StateProvinceCode).HasColumnName("StateProvinceCode")
			builder.Property(Function(t) t.CountryRegionCode).HasColumnName("CountryRegionCode")
			builder.Property(Function(t) t.IsOnlyStateProvinceFlag).HasColumnName("IsOnlyStateProvinceFlag")
			builder.Property(Function(t) t.Name).HasColumnName("Name")
			builder.Property(Function(t) t.TerritoryID).HasColumnName("TerritoryID")
			builder.Property(Function(t) t.rowguid).HasColumnName("rowguid")
			builder.Property(Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();

			' Relationships
			builder.HasOne(Function(t) t.CountryRegion).WithMany().HasForeignKey(Function(t) t.CountryRegionCode)
			builder.HasOne(Function(t) t.SalesTerritory).WithMany(Function(t) t.StateProvince).HasForeignKey(Function(d) d.TerritoryID)
		End Sub
	End Module
End Namespace