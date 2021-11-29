Imports System.Data.Entity.ModelConfiguration

Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders

Namespace AW.Mapping
	Public Class TransactionHistoryMap
		Inherits EntityTypeConfiguration(Of TransactionHistory)

		Public Sub New()
			' Primary Key
			HasKey(Function(t) t.TransactionID)

			' Properties
			[Property](Function(t) t.TransactionType).IsRequired().IsFixedLength().HasMaxLength(1)

			' Table & Column Mappings
			ToTable("TransactionHistory", "Production")
			[Property](Function(t) t.TransactionID).HasColumnName("TransactionID")
			[Property](Function(t) t.ProductID).HasColumnName("ProductID")
			[Property](Function(t) t.ReferenceOrderID).HasColumnName("ReferenceOrderID")
			[Property](Function(t) t.ReferenceOrderLineID).HasColumnName("ReferenceOrderLineID")
			[Property](Function(t) t.TransactionDate).HasColumnName("TransactionDate")
			[Property](Function(t) t.TransactionType).HasColumnName("TransactionType")
			[Property](Function(t) t.Quantity).HasColumnName("Quantity")
			[Property](Function(t) t.ActualCost).HasColumnName("ActualCost")
			[Property](Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();

			' Relationships
			HasRequired(Function(t) t.Product).WithMany().HasForeignKey(Function(t) t.ProductID)
		End Sub
	End Class

	Public Module Mapper
		<System.Runtime.CompilerServices.Extension> _
		Public Sub Map(ByVal builder As EntityTypeBuilder(Of TransactionHistory))
			builder.HasKey(Function(t) t.TransactionID)

			' Properties
			builder.Property(Function(t) t.TransactionType).IsRequired().IsFixedLength().HasMaxLength(1)

			' Table & Column Mappings
			builder.ToTable("TransactionHistory", "Production")
			builder.Property(Function(t) t.TransactionID).HasColumnName("TransactionID")
			builder.Property(Function(t) t.ProductID).HasColumnName("ProductID")
			builder.Property(Function(t) t.ReferenceOrderID).HasColumnName("ReferenceOrderID")
			builder.Property(Function(t) t.ReferenceOrderLineID).HasColumnName("ReferenceOrderLineID")
			builder.Property(Function(t) t.TransactionDate).HasColumnName("TransactionDate")
			builder.Property(Function(t) t.TransactionType).HasColumnName("TransactionType")
			builder.Property(Function(t) t.Quantity).HasColumnName("Quantity")
			builder.Property(Function(t) t.ActualCost).HasColumnName("ActualCost")
			builder.Property(Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();

			' Relationships
			builder.HasOne(Function(t) t.Product).WithMany().HasForeignKey(Function(t) t.ProductID)
		End Sub
	End Module
End Namespace