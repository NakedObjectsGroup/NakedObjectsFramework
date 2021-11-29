Imports System.ComponentModel.DataAnnotations.Schema
Imports System.Data.Entity.ModelConfiguration

Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders

Namespace AW.Mapping
	Public Class SalesOrderHeaderSalesReasonMap
		Inherits EntityTypeConfiguration(Of SalesOrderHeaderSalesReason)

		Public Sub New()
			' Primary Key
			HasKey(Function(t) New With {
				Key t.SalesOrderID,
				Key t.SalesReasonID
			})

			' Properties
			[Property](Function(t) t.SalesOrderID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)

			[Property](Function(t) t.SalesReasonID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)

			' Table & Column Mappings
			ToTable("SalesOrderHeaderSalesReason", "Sales")
			[Property](Function(t) t.SalesOrderID).HasColumnName("SalesOrderID")
			[Property](Function(t) t.SalesReasonID).HasColumnName("SalesReasonID")
			[Property](Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();

			' Relationships
			HasRequired(Function(t) t.SalesOrderHeader).WithMany(Function(t) t.SalesOrderHeaderSalesReason).HasForeignKey(Function(d) d.SalesOrderID)
			HasRequired(Function(t) t.SalesReason).WithMany().HasForeignKey(Function(t) t.SalesReasonID)
		End Sub
	End Class

	Public Module Mapper
		<System.Runtime.CompilerServices.Extension> _
		Public Sub Map(ByVal builder As EntityTypeBuilder(Of SalesOrderHeaderSalesReason))
			builder.HasKey(Function(t) New With {
				Key t.SalesOrderID,
				Key t.SalesReasonID
			})

			' Properties
			builder.Property(Function(t) t.SalesOrderID).ValueGeneratedNever()

			builder.Property(Function(t) t.SalesReasonID).ValueGeneratedNever()

			' Table & Column Mappings
			builder.ToTable("SalesOrderHeaderSalesReason", "Sales")
			builder.Property(Function(t) t.SalesOrderID).HasColumnName("SalesOrderID")
			builder.Property(Function(t) t.SalesReasonID).HasColumnName("SalesReasonID")
			builder.Property(Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();

			' Relationships
			builder.HasOne(Function(t) t.SalesOrderHeader).WithMany(Function(t) t.SalesOrderHeaderSalesReason).HasForeignKey(Function(d) d.SalesOrderID)
			builder.HasOne(Function(t) t.SalesReason).WithMany().HasForeignKey(Function(t) t.SalesReasonID)
		End Sub
	End Module
End Namespace