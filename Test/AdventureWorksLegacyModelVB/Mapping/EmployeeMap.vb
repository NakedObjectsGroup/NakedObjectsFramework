Imports System.Data.Entity.ModelConfiguration

Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders
#Disable Warning BC8603
#Disable Warning BC8602

Namespace AW.Mapping
	Public Class EmployeeMap
		Inherits EntityTypeConfiguration(Of Employee)

		Public Sub New()
			' Primary Key
			HasKey(Function(t) t.BusinessEntityID)

			' Properties
			[Property](Function(t) t.NationalIDNumber).IsRequired().HasMaxLength(15)

			[Property](Function(t) t.LoginID).IsRequired().HasMaxLength(256)

			[Property](Function(t) t.JobTitle).IsRequired().HasMaxLength(50)

			[Property](Function(t) t.MaritalStatus).IsRequired().IsFixedLength().HasMaxLength(1)

			[Property](Function(t) t.Gender).IsRequired().IsFixedLength().HasMaxLength(1)

			' Table & Column Mappings
			ToTable("Employee", "HumanResources")
			[Property](Function(t) t.BusinessEntityID).HasColumnName("BusinessEntityID")
			[Property](Function(t) t.NationalIDNumber).HasColumnName("NationalIDNumber")
			[Property](Function(t) t.LoginID).HasColumnName("LoginID")
			[Property](Function(t) t.JobTitle).HasColumnName("JobTitle")
			[Property](Function(t) t.DateOfBirth).HasColumnName("BirthDate")
			[Property](Function(t) t.MaritalStatus).HasColumnName("MaritalStatus")
			[Property](Function(t) t.Gender).HasColumnName("Gender")
			[Property](Function(t) t.HireDate).HasColumnName("HireDate")
			[Property](Function(t) t.Salaried).HasColumnName("SalariedFlag")
			[Property](Function(t) t.VacationHours).HasColumnName("VacationHours")
			[Property](Function(t) t.SickLeaveHours).HasColumnName("SickLeaveHours")
			[Property](Function(t) t.Current).HasColumnName("CurrentFlag")
			[Property](Function(t) t.rowguid).HasColumnName("rowguid")
			[Property](Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();

			' Relationships
			Ignore(Function(t) t.Manager)
			HasOptional(Function(t) t.SalesPerson).WithRequired(Function(t) t.EmployeeDetails)
			HasRequired(Function(t) t.PersonDetails).WithOptional(Function(t) t.Employee)
		End Sub
	End Class

	Public Module Mapper
		<System.Runtime.CompilerServices.Extension> _
		Public Sub Map(ByVal builder As EntityTypeBuilder(Of Employee))
			builder.HasKey(Function(t) t.BusinessEntityID)

			' Properties
			builder.Property(Function(t) t.NationalIDNumber).IsRequired().HasMaxLength(15)

			builder.Property(Function(t) t.LoginID).IsRequired().HasMaxLength(256)

			builder.Property(Function(t) t.JobTitle).IsRequired().HasMaxLength(50)

			builder.Property(Function(t) t.MaritalStatus).IsRequired().IsFixedLength().HasMaxLength(1)

			builder.Property(Function(t) t.Gender).IsRequired().IsFixedLength().HasMaxLength(1)

			' Table & Column Mappings
			builder.ToTable("Employee", "HumanResources")
			builder.Property(Function(t) t.BusinessEntityID).HasColumnName("BusinessEntityID")
			builder.Property(Function(t) t.NationalIDNumber).HasColumnName("NationalIDNumber")
			builder.Property(Function(t) t.LoginID).HasColumnName("LoginID")
			builder.Property(Function(t) t.JobTitle).HasColumnName("JobTitle")
			builder.Property(Function(t) t.DateOfBirth).HasColumnName("BirthDate")
			builder.Property(Function(t) t.MaritalStatus).HasColumnName("MaritalStatus")
			builder.Property(Function(t) t.Gender).HasColumnName("Gender")
			builder.Property(Function(t) t.HireDate).HasColumnName("HireDate")
			builder.Property(Function(t) t.Salaried).HasColumnName("SalariedFlag")
			builder.Property(Function(t) t.VacationHours).HasColumnName("VacationHours")
			builder.Property(Function(t) t.SickLeaveHours).HasColumnName("SickLeaveHours")
			builder.Property(Function(t) t.Current).HasColumnName("CurrentFlag")
			builder.Property(Function(t) t.rowguid).HasColumnName("rowguid")
			builder.Property(Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();

			' Relationships
			builder.Ignore(Function(t) t.Manager)
			builder.HasOne(Function(t) t.PersonDetails).WithOne(Function(t) t.Employee)
		End Sub
	End Module
End Namespace