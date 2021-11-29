Imports System.Data.Entity.ModelConfiguration

Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders

Namespace AW.Mapping
	Public Class JobCandidateMap
		Inherits EntityTypeConfiguration(Of JobCandidate)

		Public Sub New()
			' Primary Key
			HasKey(Function(t) t.JobCandidateID)

			' Properties
			' Table & Column Mappings
			ToTable("JobCandidate", "HumanResources")
			[Property](Function(t) t.JobCandidateID).HasColumnName("JobCandidateID")
			[Property](Function(t) t.EmployeeID).HasColumnName("BusinessEntityID")
			[Property](Function(t) t.Resume).HasColumnName("Resume")
			[Property](Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();

			' Relationships
			HasOptional(Function(t) t.Employee).WithMany().HasForeignKey(Function(t) t.EmployeeID)
		End Sub
	End Class

	Public Module Mapper
		<System.Runtime.CompilerServices.Extension> _
		Public Sub Map(ByVal builder As EntityTypeBuilder(Of JobCandidate))
			builder.HasKey(Function(t) t.JobCandidateID)

			' Properties
			' Table & Column Mappings
			builder.ToTable("JobCandidate", "HumanResources")
			builder.Property(Function(t) t.JobCandidateID).HasColumnName("JobCandidateID")
			builder.Property(Function(t) t.EmployeeID).HasColumnName("BusinessEntityID")
			builder.Property(Function(t) t.Resume).HasColumnName("Resume")
			builder.Property(Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();

			' Relationships
			builder.HasOne(Function(t) t.Employee).WithMany().HasForeignKey(Function(t) t.EmployeeID)
		End Sub
	End Module
End Namespace