Imports System.ComponentModel.DataAnnotations.Schema
Imports System.Data.Entity.ModelConfiguration

Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders

Namespace AW.Mapping
	Public Class SalesPersonQuotaHistoryMap
		Inherits EntityTypeConfiguration(Of SalesPersonQuotaHistory)

		Public Sub New()
			' Primary Key
			HasKey(Function(t) New With {
				Key t.BusinessEntityID,
				Key t.QuotaDate
			})

			' Properties
			[Property](Function(t) t.BusinessEntityID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)

			' Table & Column Mappings
			ToTable("SalesPersonQuotaHistory", "Sales")
			[Property](Function(t) t.BusinessEntityID).HasColumnName("BusinessEntityID")
			[Property](Function(t) t.QuotaDate).HasColumnName("QuotaDate")
			[Property](Function(t) t.SalesQuota).HasColumnName("SalesQuota")
			[Property](Function(t) t.rowguid).HasColumnName("rowguid")
			[Property](Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();

			' Relationships
			HasRequired(Function(t) t.SalesPerson).WithMany(Function(t) t.QuotaHistory).HasForeignKey(Function(d) d.BusinessEntityID)
		End Sub
	End Class

	Public Module Mapper
		<System.Runtime.CompilerServices.Extension> _
		Public Sub Map(ByVal builder As EntityTypeBuilder(Of SalesPersonQuotaHistory))
			builder.HasKey(Function(t) New With {
				Key t.BusinessEntityID,
				Key t.QuotaDate
			})

			' Properties
			builder.Property(Function(t) t.BusinessEntityID).ValueGeneratedNever()

			' Table & Column Mappings
			builder.ToTable("SalesPersonQuotaHistory", "Sales")
			builder.Property(Function(t) t.BusinessEntityID).HasColumnName("BusinessEntityID")
			builder.Property(Function(t) t.QuotaDate).HasColumnName("QuotaDate")
			builder.Property(Function(t) t.SalesQuota).HasColumnName("SalesQuota")
			builder.Property(Function(t) t.rowguid).HasColumnName("rowguid")
			builder.Property(Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();

			' Relationships
			builder.HasOne(Function(t) t.SalesPerson).WithMany(Function(t) t.QuotaHistory).HasForeignKey(Function(d) d.BusinessEntityID)
		End Sub
	End Module
End Namespace