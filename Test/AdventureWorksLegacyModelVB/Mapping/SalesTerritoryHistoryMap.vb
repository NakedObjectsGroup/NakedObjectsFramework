Imports System.ComponentModel.DataAnnotations.Schema
Imports System.Data.Entity.ModelConfiguration

Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders

Namespace AW.Mapping
	Public Class SalesTerritoryHistoryMap
		Inherits EntityTypeConfiguration(Of SalesTerritoryHistory)

		Public Sub New()
			' Primary Key
			HasKey(Function(t) New With {
				Key t.BusinessEntityID,
				Key t.SalesTerritoryID,
				Key t.StartDate
			})

			' Properties
			[Property](Function(t) t.BusinessEntityID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)

			[Property](Function(t) t.SalesTerritoryID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)

			' Table & Column Mappings
			ToTable("SalesTerritoryHistory", "Sales")
			[Property](Function(t) t.BusinessEntityID).HasColumnName("BusinessEntityID")
			[Property](Function(t) t.SalesTerritoryID).HasColumnName("TerritoryID")
			[Property](Function(t) t.StartDate).HasColumnName("StartDate")
			[Property](Function(t) t.EndDate).HasColumnName("EndDate")
			[Property](Function(t) t.rowguid).HasColumnName("rowguid")
			[Property](Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();

			' Relationships
			HasRequired(Function(t) t.SalesPerson).WithMany(Function(t) t.TerritoryHistory).HasForeignKey(Function(d) d.BusinessEntityID)
			HasRequired(Function(t) t.SalesTerritory).WithMany().HasForeignKey(Function(t) t.SalesTerritoryID)
		End Sub
	End Class

	Public Module Mapper
		<System.Runtime.CompilerServices.Extension> _
		Public Sub Map(ByVal builder As EntityTypeBuilder(Of SalesTerritoryHistory))
			builder.HasKey(Function(t) New With {
				Key t.BusinessEntityID,
				Key t.SalesTerritoryID,
				Key t.StartDate
			})

			' Properties
			builder.Property(Function(t) t.BusinessEntityID).ValueGeneratedNever()

			builder.Property(Function(t) t.SalesTerritoryID).ValueGeneratedNever()

			' Table & Column Mappings
			builder.ToTable("SalesTerritoryHistory", "Sales")
			builder.Property(Function(t) t.BusinessEntityID).HasColumnName("BusinessEntityID")
			builder.Property(Function(t) t.SalesTerritoryID).HasColumnName("TerritoryID")
			builder.Property(Function(t) t.StartDate).HasColumnName("StartDate")
			builder.Property(Function(t) t.EndDate).HasColumnName("EndDate")
			builder.Property(Function(t) t.rowguid).HasColumnName("rowguid")
			builder.Property(Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();

			' Relationships
			builder.HasOne(Function(t) t.SalesPerson).WithMany(Function(t) t.TerritoryHistory).HasForeignKey(Function(d) d.BusinessEntityID)
			builder.HasOne(Function(t) t.SalesTerritory).WithMany().HasForeignKey(Function(t) t.SalesTerritoryID)
		End Sub
	End Module
End Namespace