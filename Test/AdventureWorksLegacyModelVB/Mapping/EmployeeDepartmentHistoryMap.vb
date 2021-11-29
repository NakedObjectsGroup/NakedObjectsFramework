Imports System.ComponentModel.DataAnnotations.Schema
Imports System.Data.Entity.ModelConfiguration

Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders

Namespace AW.Mapping
	Public Class EmployeeDepartmentHistoryMap
		Inherits EntityTypeConfiguration(Of EmployeeDepartmentHistory)

		Public Sub New()
			' Primary Key
			HasKey(Function(t) New With {
				Key t.EmployeeID,
				Key t.DepartmentID,
				Key t.ShiftID,
				Key t.StartDate
			})

			' Properties
			[Property](Function(t) t.EmployeeID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)

			[Property](Function(t) t.DepartmentID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)

			' Table & Column Mappings
			ToTable("EmployeeDepartmentHistory", "HumanResources")
			[Property](Function(t) t.EmployeeID).HasColumnName("BusinessEntityID")
			[Property](Function(t) t.DepartmentID).HasColumnName("DepartmentID")
			[Property](Function(t) t.ShiftID).HasColumnName("ShiftID")
			[Property](Function(t) t.StartDate).HasColumnName("StartDate")
			[Property](Function(t) t.EndDate).HasColumnName("EndDate")
			[Property](Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();

			' Relationships
			HasRequired(Function(t) t.Department).WithMany().HasForeignKey(Function(t) t.DepartmentID)
			HasRequired(Function(t) t.Employee).WithMany(Function(t) t.DepartmentHistory).HasForeignKey(Function(d) d.EmployeeID)
			HasRequired(Function(t) t.Shift).WithMany().HasForeignKey(Function(t) t.ShiftID)
		End Sub
	End Class

	Public Module Mapper
		<System.Runtime.CompilerServices.Extension> _
		Public Sub Map(ByVal builder As EntityTypeBuilder(Of EmployeeDepartmentHistory))
			builder.HasKey(Function(t) New With {
				Key t.EmployeeID,
				Key t.DepartmentID,
				Key t.ShiftID,
				Key t.StartDate
			})

			' Properties
			builder.Property(Function(t) t.EmployeeID).ValueGeneratedNever()

			builder.Property(Function(t) t.DepartmentID).ValueGeneratedNever()

			' Table & Column Mappings
			builder.ToTable("EmployeeDepartmentHistory", "HumanResources")
			builder.Property(Function(t) t.EmployeeID).HasColumnName("BusinessEntityID")
			builder.Property(Function(t) t.DepartmentID).HasColumnName("DepartmentID")
			builder.Property(Function(t) t.ShiftID).HasColumnName("ShiftID")
			builder.Property(Function(t) t.StartDate).HasColumnName("StartDate")
			builder.Property(Function(t) t.EndDate).HasColumnName("EndDate")
			builder.Property(Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();

			' Relationships
			builder.HasOne(Function(t) t.Department).WithMany().HasForeignKey(Function(t) t.DepartmentID)
			builder.HasOne(Function(t) t.Employee).WithMany(Function(t) t.DepartmentHistory).HasForeignKey(Function(d) d.EmployeeID)
			builder.HasOne(Function(t) t.Shift).WithMany().HasForeignKey(Function(t) t.ShiftID)
		End Sub
	End Module
End Namespace