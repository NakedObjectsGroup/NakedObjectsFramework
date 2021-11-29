Imports System.ComponentModel.DataAnnotations.Schema
Imports System.Data.Entity.ModelConfiguration

Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders

Namespace AW.Mapping
	Public Class ProductCostHistoryMap
		Inherits EntityTypeConfiguration(Of ProductCostHistory)

		Public Sub New()
			' Primary Key
			HasKey(Function(t) New With {
				Key t.ProductID,
				Key t.StartDate
			})

			' Properties
			[Property](Function(t) t.ProductID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)

			' Table & Column Mappings
			ToTable("ProductCostHistory", "Production")
			[Property](Function(t) t.ProductID).HasColumnName("ProductID")
			[Property](Function(t) t.StartDate).HasColumnName("StartDate")
			[Property](Function(t) t.EndDate).HasColumnName("EndDate")
			[Property](Function(t) t.StandardCost).HasColumnName("StandardCost")
			[Property](Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();

			' Relationships
			HasRequired(Function(t) t.Product).WithMany().HasForeignKey(Function(t) t.ProductID)
		End Sub
	End Class

	Public Module Mapper
		<System.Runtime.CompilerServices.Extension> _
		Public Sub Map(ByVal builder As EntityTypeBuilder(Of ProductCostHistory))
			builder.HasKey(Function(t) New With {
				Key t.ProductID,
				Key t.StartDate
			})

			' Properties
			builder.Property(Function(t) t.ProductID).ValueGeneratedNever()

			' Table & Column Mappings
			builder.ToTable("ProductCostHistory", "Production")
			builder.Property(Function(t) t.ProductID).HasColumnName("ProductID")
			builder.Property(Function(t) t.StartDate).HasColumnName("StartDate")
			builder.Property(Function(t) t.EndDate).HasColumnName("EndDate")
			builder.Property(Function(t) t.StandardCost).HasColumnName("StandardCost")
			builder.Property(Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();

			' Relationships
			builder.HasOne(Function(t) t.Product).WithMany().HasForeignKey(Function(t) t.ProductID)
		End Sub
	End Module
End Namespace