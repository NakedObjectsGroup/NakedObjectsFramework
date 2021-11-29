Imports System.Data.Entity.ModelConfiguration

Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders

Namespace AW.Mapping
	Public Class DepartmentMap
		Inherits EntityTypeConfiguration(Of Department)

		Public Sub New()
			' Primary Key
			HasKey(Function(t) t.DepartmentID)

			' Properties
			[Property](Function(t) t.Name).IsRequired().HasMaxLength(50)

			[Property](Function(t) t.GroupName).IsRequired().HasMaxLength(50)

			' Table & Column Mappings
			ToTable("Department", "HumanResources")
			[Property](Function(t) t.DepartmentID).HasColumnName("DepartmentID")
			[Property](Function(t) t.Name).HasColumnName("Name")
			[Property](Function(t) t.GroupName).HasColumnName("GroupName")
			[Property](Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();
		End Sub
	End Class

	Public Module Mapper
		<System.Runtime.CompilerServices.Extension> _
		Public Sub Map(ByVal builder As EntityTypeBuilder(Of Department))
			' Primary Key
			builder.HasKey(Function(t) t.DepartmentID)

			' Properties
			builder.Property(Function(t) t.Name).IsRequired().HasMaxLength(50)

			builder.Property(Function(t) t.GroupName).IsRequired().HasMaxLength(50)

			' Table & Column Mappings
			builder.ToTable("Department", "HumanResources")
			builder.Property(Function(t) t.DepartmentID).HasColumnName("DepartmentID")
			builder.Property(Function(t) t.Name).HasColumnName("Name")
			builder.Property(Function(t) t.GroupName).HasColumnName("GroupName")
			builder.Property(Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();
		End Sub
	End Module
End Namespace