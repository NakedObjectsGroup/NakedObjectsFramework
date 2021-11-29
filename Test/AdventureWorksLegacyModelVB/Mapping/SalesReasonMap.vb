Imports System.Data.Entity.ModelConfiguration

Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders

Namespace AW.Mapping
	Public Class SalesReasonMap
		Inherits EntityTypeConfiguration(Of SalesReason)

		Public Sub New()
			' Primary Key
			HasKey(Function(t) t.SalesReasonID)

			' Properties
			[Property](Function(t) t.Name).IsRequired().HasMaxLength(50)

			[Property](Function(t) t.ReasonType).IsRequired().HasMaxLength(50)

			' Table & Column Mappings
			ToTable("SalesReason", "Sales")
			[Property](Function(t) t.SalesReasonID).HasColumnName("SalesReasonID")
			[Property](Function(t) t.Name).HasColumnName("Name")
			[Property](Function(t) t.ReasonType).HasColumnName("ReasonType")
			[Property](Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();
		End Sub
	End Class

	Public Module Mapper
		<System.Runtime.CompilerServices.Extension> _
		Public Sub Map(ByVal builder As EntityTypeBuilder(Of SalesReason))
			builder.HasKey(Function(t) t.SalesReasonID)

			' Properties
			builder.Property(Function(t) t.Name).IsRequired().HasMaxLength(50)

			builder.Property(Function(t) t.ReasonType).IsRequired().HasMaxLength(50)

			' Table & Column Mappings
			builder.ToTable("SalesReason", "Sales")
			builder.Property(Function(t) t.SalesReasonID).HasColumnName("SalesReasonID")
			builder.Property(Function(t) t.Name).HasColumnName("Name")
			builder.Property(Function(t) t.ReasonType).HasColumnName("ReasonType")
			builder.Property(Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();
		End Sub
	End Module
End Namespace