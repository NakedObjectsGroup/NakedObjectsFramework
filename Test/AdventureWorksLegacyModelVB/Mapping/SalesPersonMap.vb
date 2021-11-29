Imports System.ComponentModel.DataAnnotations.Schema
Imports System.Data.Entity.ModelConfiguration

Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders
#Disable Warning BC8603

Namespace AW.Mapping
	Public Class SalesPersonMap
		Inherits EntityTypeConfiguration(Of SalesPerson)

		Public Sub New()
			' Primary Key
			HasKey(Function(t) t.BusinessEntityID)

			' Properties
			[Property](Function(t) t.BusinessEntityID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)

			' Table & Column Mappings
			ToTable("SalesPerson", "Sales")
			[Property](Function(t) t.BusinessEntityID).HasColumnName("BusinessEntityID")
			[Property](Function(t) t.SalesTerritoryID).HasColumnName("TerritoryID")
			[Property](Function(t) t.SalesQuota).HasColumnName("SalesQuota")
			[Property](Function(t) t.Bonus).HasColumnName("Bonus")
			[Property](Function(t) t.CommissionPct).HasColumnName("CommissionPct")
			[Property](Function(t) t.SalesYTD).HasColumnName("SalesYTD")
			[Property](Function(t) t.SalesLastYear).HasColumnName("SalesLastYear")
			[Property](Function(t) t.rowguid).HasColumnName("rowguid")
			[Property](Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();

			' Relationships
			HasRequired(Function(t) t.EmployeeDetails).WithOptional(Function(t) t.SalesPerson)
			HasOptional(Function(t) t.SalesTerritory).WithMany().HasForeignKey(Function(t) t.SalesTerritoryID)

			Ignore(Function(t) t.PersonDetails)
		End Sub
	End Class

	Public Module Mapper
		<System.Runtime.CompilerServices.Extension> _
		Public Sub Map(ByVal builder As EntityTypeBuilder(Of SalesPerson))
			builder.HasKey(Function(t) t.BusinessEntityID)

			' Properties
			builder.Property(Function(t) t.BusinessEntityID).ValueGeneratedNever()

			' Table & Column Mappings
			builder.ToTable("SalesPerson", "Sales")
			builder.Property(Function(t) t.BusinessEntityID).HasColumnName("BusinessEntityID")
			builder.Property(Function(t) t.SalesTerritoryID).HasColumnName("TerritoryID")
			builder.Property(Function(t) t.SalesQuota).HasColumnName("SalesQuota")
			builder.Property(Function(t) t.Bonus).HasColumnName("Bonus")
			builder.Property(Function(t) t.CommissionPct).HasColumnName("CommissionPct")
			builder.Property(Function(t) t.SalesYTD).HasColumnName("SalesYTD")
			builder.Property(Function(t) t.SalesLastYear).HasColumnName("SalesLastYear")
			builder.Property(Function(t) t.rowguid).HasColumnName("rowguid")
			builder.Property(Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();

			' Relationships
			builder.HasOne(Function(t) t.EmployeeDetails).WithOne(Function(t) t.SalesPerson).HasForeignKey(Of Employee)(Function(t) t.BusinessEntityID)
			builder.HasOne(Function(t) t.SalesTerritory).WithMany().HasForeignKey(Function(t) t.SalesTerritoryID)

			builder.Ignore(Function(t) t.PersonDetails)
		End Sub
	End Module
End Namespace