Imports System.ComponentModel.DataAnnotations.Schema
Imports System.Data.Entity.ModelConfiguration

Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders

Namespace AW.Mapping
	Public Class WorkOrderRoutingMap
		Inherits EntityTypeConfiguration(Of WorkOrderRouting)

		Public Sub New()
			' Primary Key
			HasKey(Function(t) New With {
				Key t.WorkOrderID,
				Key t.ProductID,
				Key t.OperationSequence
			})

			' Properties
			[Property](Function(t) t.WorkOrderID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)

			[Property](Function(t) t.ProductID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)

			[Property](Function(t) t.OperationSequence).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)

			' Table & Column Mappings
			ToTable("WorkOrderRouting", "Production")
			[Property](Function(t) t.WorkOrderID).HasColumnName("WorkOrderID")
			[Property](Function(t) t.ProductID).HasColumnName("ProductID")
			[Property](Function(t) t.OperationSequence).HasColumnName("OperationSequence")
			[Property](Function(t) t.LocationID).HasColumnName("LocationID")
			[Property](Function(t) t.ScheduledStartDate).HasColumnName("ScheduledStartDate")
			[Property](Function(t) t.ScheduledEndDate).HasColumnName("ScheduledEndDate")
			[Property](Function(t) t.ActualStartDate).HasColumnName("ActualStartDate")
			[Property](Function(t) t.ActualEndDate).HasColumnName("ActualEndDate")
			[Property](Function(t) t.ActualResourceHrs).HasColumnName("ActualResourceHrs")
			[Property](Function(t) t.PlannedCost).HasColumnName("PlannedCost")
			[Property](Function(t) t.ActualCost).HasColumnName("ActualCost")
			[Property](Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();

			' Relationships
			HasRequired(Function(t) t.Location).WithMany().HasForeignKey(Function(t) t.LocationID)
			HasRequired(Function(t) t.WorkOrder).WithMany(Function(t) t.WorkOrderRoutings).HasForeignKey(Function(d) d.WorkOrderID)
		End Sub
	End Class

	Public Module Mapper
		<System.Runtime.CompilerServices.Extension> _
		Public Sub Map(ByVal builder As EntityTypeBuilder(Of WorkOrderRouting))
			builder.HasKey(Function(t) New With {
				Key t.WorkOrderID,
				Key t.ProductID,
				Key t.OperationSequence
			})

			' Properties
			builder.Property(Function(t) t.WorkOrderID).ValueGeneratedNever()

			builder.Property(Function(t) t.ProductID).ValueGeneratedNever()

			builder.Property(Function(t) t.OperationSequence).ValueGeneratedNever()

			' Table & Column Mappings
			builder.ToTable("WorkOrderRouting", "Production")
			builder.Property(Function(t) t.WorkOrderID).HasColumnName("WorkOrderID")
			builder.Property(Function(t) t.ProductID).HasColumnName("ProductID")
			builder.Property(Function(t) t.OperationSequence).HasColumnName("OperationSequence")
			builder.Property(Function(t) t.LocationID).HasColumnName("LocationID")
			builder.Property(Function(t) t.ScheduledStartDate).HasColumnName("ScheduledStartDate")
			builder.Property(Function(t) t.ScheduledEndDate).HasColumnName("ScheduledEndDate")
			builder.Property(Function(t) t.ActualStartDate).HasColumnName("ActualStartDate")
			builder.Property(Function(t) t.ActualEndDate).HasColumnName("ActualEndDate")
			builder.Property(Function(t) t.ActualResourceHrs).HasColumnName("ActualResourceHrs")
			builder.Property(Function(t) t.PlannedCost).HasColumnName("PlannedCost")
			builder.Property(Function(t) t.ActualCost).HasColumnName("ActualCost")
			builder.Property(Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();

			' Relationships
			builder.HasOne(Function(t) t.Location).WithMany().HasForeignKey(Function(t) t.LocationID)
			builder.HasOne(Function(t) t.WorkOrder).WithMany(Function(t) t.WorkOrderRoutings).HasForeignKey(Function(d) d.WorkOrderID)
		End Sub
	End Module
End Namespace