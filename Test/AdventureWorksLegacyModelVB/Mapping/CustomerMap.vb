Imports System.ComponentModel.DataAnnotations.Schema
Imports System.Data.Entity.ModelConfiguration

Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders

Namespace AW.Mapping
	Public Class CustomerMap
		Inherits EntityTypeConfiguration(Of Customer)

		Public Sub New()
			' Primary Key
			HasKey(Function(t) t.CustomerID)

			' Properties
			[Property](Function(t) t.AccountNumber).IsRequired().HasMaxLength(10).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed)

			[Property](Function(t) t.StoreID).IsOptional()
			[Property](Function(t) t.PersonID).IsOptional()
			[Property](Function(t) t.SalesTerritoryID).IsOptional()

			' Table & Column Mappings
			ToTable("Customer", "Sales")
			[Property](Function(t) t.CustomerID).HasColumnName("CustomerID")
			[Property](Function(t) t.SalesTerritoryID).HasColumnName("TerritoryID")
			[Property](Function(t) t.StoreID).HasColumnName("StoreID")
			[Property](Function(t) t.PersonID).HasColumnName("PersonID")
			[Property](Function(t) t.AccountNumber).HasColumnName("AccountNumber")
			[Property](Function(t) t.CustomerRowguid).HasColumnName("rowguid")
			[Property](Function(t) t.CustomerModifiedDate).HasColumnName("ModifiedDate")

			' Relationships
			HasOptional(Function(t) t.SalesTerritory).WithMany().HasForeignKey(Function(t) t.SalesTerritoryID)
			HasOptional(Function(t) t.Store).WithMany().HasForeignKey(Function(t) t.StoreID)
			HasOptional(Function(t) t.Person).WithMany().HasForeignKey(Function(t) t.PersonID)
		End Sub
	End Class

	Public Module Mapper
		<System.Runtime.CompilerServices.Extension> _
		Public Sub Map(ByVal builder As EntityTypeBuilder(Of Customer))
			builder.HasKey(Function(t) t.CustomerID)

			' Properties
			builder.Property(Function(t) t.AccountNumber).IsRequired().HasMaxLength(10).ValueGeneratedOnAddOrUpdate()

			builder.Property(Function(t) t.StoreID) '.IsOptional();
			builder.Property(Function(t) t.PersonID) '.IsOptional();
			builder.Property(Function(t) t.SalesTerritoryID) '.IsOptional();

			' Table & Column Mappings
			builder.ToTable("Customer", "Sales")
			builder.Property(Function(t) t.CustomerID).HasColumnName("CustomerID")
			builder.Property(Function(t) t.SalesTerritoryID).HasColumnName("TerritoryID")
			builder.Property(Function(t) t.StoreID).HasColumnName("StoreID")
			builder.Property(Function(t) t.PersonID).HasColumnName("PersonID")
			builder.Property(Function(t) t.AccountNumber).HasColumnName("AccountNumber")
			builder.Property(Function(t) t.CustomerRowguid).HasColumnName("rowguid")
			builder.Property(Function(t) t.CustomerModifiedDate).HasColumnName("ModifiedDate")

			' Relationships
			builder.HasOne(Function(t) t.SalesTerritory).WithMany().HasForeignKey(Function(t) t.SalesTerritoryID)
			builder.HasOne(Function(t) t.Store).WithMany().HasForeignKey(Function(t) t.StoreID)
			builder.HasOne(Function(t) t.Person).WithMany().HasForeignKey(Function(t) t.PersonID)
		End Sub
	End Module
End Namespace