Imports System.ComponentModel.DataAnnotations.Schema
Imports System.Data.Entity.ModelConfiguration

Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders

Namespace AW.Mapping
	Public Class ShiftMap
		Inherits EntityTypeConfiguration(Of Shift)

		Public Sub New()
			' Primary Key
			HasKey(Function(t) t.ShiftID)

			' Properties
			[Property](Function(t) t.Name).IsRequired().HasMaxLength(50)

			' Table & Column Mappings
			ToTable("Shift", "HumanResources")
			[Property](Function(t) t.ShiftID).HasColumnName("ShiftID").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
			[Property](Function(t) t.Name).HasColumnName("Name")
			[Property](Function(t) t.StartTime).HasColumnName("StartTime")
			[Property](Function(t) t.EndTime).HasColumnName("EndTime")
			[Property](Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();
		End Sub
	End Class

	Public Module Mapper
		<System.Runtime.CompilerServices.Extension> _
		Public Sub Map(ByVal builder As EntityTypeBuilder(Of Shift))
			builder.HasKey(Function(t) t.ShiftID)

			' Properties
			builder.Property(Function(t) t.Name).IsRequired().HasMaxLength(50)

			' Table & Column Mappings
			builder.ToTable("Shift", "HumanResources")
			builder.Property(Function(t) t.ShiftID).HasColumnName("ShiftID").ValueGeneratedOnAdd()
			builder.Property(Function(t) t.Name).HasColumnName("Name")
			builder.Property(Function(t) t.StartTime).HasColumnName("StartTime")
			builder.Property(Function(t) t.EndTime).HasColumnName("EndTime")
			builder.Property(Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();
		End Sub
	End Module
End Namespace