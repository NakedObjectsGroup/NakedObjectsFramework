Imports System.Data.Entity.ModelConfiguration

Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders

Namespace AW.Mapping
	Public Class SalesTerritoryMap
		Inherits EntityTypeConfiguration(Of SalesTerritory)

		Public Sub New()
			' Primary Key
			HasKey(Function(t) t.TerritoryID)

			' Properties
			[Property](Function(t) t.Name).IsRequired().HasMaxLength(50)

			[Property](Function(t) t.CountryRegionCode).IsRequired().HasMaxLength(3)

			[Property](Function(t) t.Group).IsRequired().HasMaxLength(50)

			' Table & Column Mappings
			ToTable("SalesTerritory", "Sales")
			[Property](Function(t) t.TerritoryID).HasColumnName("TerritoryID")
			[Property](Function(t) t.Name).HasColumnName("Name")
			[Property](Function(t) t.CountryRegionCode).HasColumnName("CountryRegionCode")
			[Property](Function(t) t.Group).HasColumnName("Group")
			[Property](Function(t) t.SalesYTD).HasColumnName("SalesYTD")
			[Property](Function(t) t.SalesLastYear).HasColumnName("SalesLastYear")
			[Property](Function(t) t.CostYTD).HasColumnName("CostYTD")
			[Property](Function(t) t.CostLastYear).HasColumnName("CostLastYear")
			[Property](Function(t) t.rowguid).HasColumnName("rowguid")
			[Property](Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();
		End Sub
	End Class

	Public Module Mapper
		<System.Runtime.CompilerServices.Extension> _
		Public Sub Map(ByVal builder As EntityTypeBuilder(Of SalesTerritory))
			builder.HasKey(Function(t) t.TerritoryID)

			' Properties
			builder.Property(Function(t) t.Name).IsRequired().HasMaxLength(50)

			builder.Property(Function(t) t.CountryRegionCode).IsRequired().HasMaxLength(3)

			builder.Property(Function(t) t.Group).IsRequired().HasMaxLength(50)

			' Table & Column Mappings
			builder.ToTable("SalesTerritory", "Sales")
			builder.Property(Function(t) t.TerritoryID).HasColumnName("TerritoryID")
			builder.Property(Function(t) t.Name).HasColumnName("Name")
			builder.Property(Function(t) t.CountryRegionCode).HasColumnName("CountryRegionCode")
			builder.Property(Function(t) t.Group).HasColumnName("Group")
			builder.Property(Function(t) t.SalesYTD).HasColumnName("SalesYTD")
			builder.Property(Function(t) t.SalesLastYear).HasColumnName("SalesLastYear")
			builder.Property(Function(t) t.CostYTD).HasColumnName("CostYTD")
			builder.Property(Function(t) t.CostLastYear).HasColumnName("CostLastYear")
			builder.Property(Function(t) t.rowguid).HasColumnName("rowguid")
			builder.Property(Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();
		End Sub
	End Module
End Namespace