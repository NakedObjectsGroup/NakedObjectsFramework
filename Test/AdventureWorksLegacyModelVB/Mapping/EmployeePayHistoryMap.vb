Imports System.ComponentModel.DataAnnotations.Schema
Imports System.Data.Entity.ModelConfiguration

Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders

Namespace AW.Mapping
	Public Class EmployeePayHistoryMap
		Inherits EntityTypeConfiguration(Of EmployeePayHistory)

		Public Sub New()
			' Primary Key
			HasKey(Function(t) New With {
				Key t.EmployeeID,
				Key t.RateChangeDate
			})

			' Properties
			[Property](Function(t) t.EmployeeID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)

			' Table & Column Mappings
			ToTable("EmployeePayHistory", "HumanResources")
			[Property](Function(t) t.EmployeeID).HasColumnName("BusinessEntityID")
			[Property](Function(t) t.RateChangeDate).HasColumnName("RateChangeDate")
			[Property](Function(t) t.Rate).HasColumnName("Rate")
			[Property](Function(t) t.PayFrequency).HasColumnName("PayFrequency")
			[Property](Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();

			' Relationships
			HasRequired(Function(t) t.Employee).WithMany(Function(t) t.PayHistory).HasForeignKey(Function(d) d.EmployeeID)
		End Sub
	End Class

	Public Module Mapper
		<System.Runtime.CompilerServices.Extension> _
		Public Sub Map(ByVal builder As EntityTypeBuilder(Of EmployeePayHistory))
			builder.HasKey(Function(t) New With {
				Key t.EmployeeID,
				Key t.RateChangeDate
			})

			' Properties
			builder.Property(Function(t) t.EmployeeID).ValueGeneratedNever()

			' Table & Column Mappings
			builder.ToTable("EmployeePayHistory", "HumanResources")
			builder.Property(Function(t) t.EmployeeID).HasColumnName("BusinessEntityID")
			builder.Property(Function(t) t.RateChangeDate).HasColumnName("RateChangeDate")
			builder.Property(Function(t) t.Rate).HasColumnName("Rate")
			builder.Property(Function(t) t.PayFrequency).HasColumnName("PayFrequency")
			builder.Property(Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();

			' Relationships
			builder.HasOne(Function(t) t.Employee).WithMany(Function(t) t.PayHistory).HasForeignKey(Function(d) d.EmployeeID)
		End Sub
	End Module
End Namespace