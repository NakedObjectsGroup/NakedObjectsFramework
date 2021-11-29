Imports System.ComponentModel.DataAnnotations.Schema
Imports System.Data.Entity.ModelConfiguration

Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders

Namespace AW.Mapping
	Public Class WorkOrderMap
		Inherits EntityTypeConfiguration(Of WorkOrder)

		Public Sub New()
			' Primary Key
			HasKey(Function(t) t.WorkOrderID)

			Ignore(Function(t) t.AnAlwaysHiddenReadOnlyProperty)

			' Properties
			' Table & Column Mappings
			ToTable("WorkOrder", "Production")
			[Property](Function(t) t.WorkOrderID).HasColumnName("WorkOrderID")
			[Property](Function(t) t.ProductID).HasColumnName("ProductID")
			[Property](Function(t) t.OrderQty).HasColumnName("OrderQty")
			[Property](Function(t) t.StockedQty).HasColumnName("StockedQty").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed)

			[Property](Function(t) t.ScrappedQty).HasColumnName("ScrappedQty")
			[Property](Function(t) t.StartDate).HasColumnName("StartDate")
			[Property](Function(t) t.EndDate).HasColumnName("EndDate")
			[Property](Function(t) t.DueDate).HasColumnName("DueDate")
			[Property](Function(t) t.ScrapReasonID).HasColumnName("ScrapReasonID")
			[Property](Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();

			' Relationships
			HasRequired(Function(t) t.Product).WithMany().HasForeignKey(Function(t) t.ProductID)
			HasOptional(Function(t) t.ScrapReason).WithMany().HasForeignKey(Function(t) t.ScrapReasonID)
		End Sub
	End Class

	Public Module Mapper
		<System.Runtime.CompilerServices.Extension> _
		Public Sub Map(ByVal builder As EntityTypeBuilder(Of WorkOrder))
			builder.HasKey(Function(t) t.WorkOrderID)

			builder.Ignore(Function(t) t.AnAlwaysHiddenReadOnlyProperty)

			' Properties
			' Table & Column Mappings
			builder.ToTable("WorkOrder", "Production")
			builder.Property(Function(t) t.WorkOrderID).HasColumnName("WorkOrderID")
			builder.Property(Function(t) t.ProductID).HasColumnName("ProductID")
			builder.Property(Function(t) t.OrderQty).HasColumnName("OrderQty")
			builder.Property(Function(t) t.StockedQty).HasColumnName("StockedQty").ValueGeneratedOnAddOrUpdate()

			builder.Property(Function(t) t.ScrappedQty).HasColumnName("ScrappedQty")
			builder.Property(Function(t) t.StartDate).HasColumnName("StartDate")
			builder.Property(Function(t) t.EndDate).HasColumnName("EndDate")
			builder.Property(Function(t) t.DueDate).HasColumnName("DueDate")
			builder.Property(Function(t) t.ScrapReasonID).HasColumnName("ScrapReasonID")
			builder.Property(Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();

			' Relationships
			builder.HasOne(Function(t) t.Product).WithMany().HasForeignKey(Function(t) t.ProductID)
			builder.HasOne(Function(t) t.ScrapReason).WithMany().HasForeignKey(Function(t) t.ScrapReasonID)
		End Sub
	End Module
End Namespace