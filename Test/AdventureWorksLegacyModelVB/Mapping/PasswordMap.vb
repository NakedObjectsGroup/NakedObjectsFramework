Imports System.Data.Entity.ModelConfiguration

Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders
#Disable Warning BC8603

Namespace AW.Mapping
	Public Class PasswordMap
		Inherits EntityTypeConfiguration(Of Password)

		Public Sub New()
			' Primary Key
			HasKey(Function(t) t.BusinessEntityID)

			' Table & Column Mappings
			ToTable("Password", "Person")
			[Property](Function(t) t.BusinessEntityID).HasColumnName("BusinessEntityID")
			[Property](Function(t) t.PasswordHash).HasColumnName("PasswordHash").IsRequired()
			[Property](Function(t) t.PasswordSalt).HasColumnName("PasswordSalt").IsRequired()
			[Property](Function(t) t.rowguid).HasColumnName("rowguid")
			[Property](Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();
			HasRequired(Function(pw) pw.Person)
		End Sub
	End Class

	Public Module Mapper
		<System.Runtime.CompilerServices.Extension> _
		Public Sub Map(ByVal builder As EntityTypeBuilder(Of Password))
			builder.HasKey(Function(t) t.BusinessEntityID)

			' Table & Column Mappings
			builder.ToTable("Password", "Person")
			builder.Property(Function(t) t.BusinessEntityID).HasColumnName("BusinessEntityID")
			builder.Property(Function(t) t.PasswordHash).HasColumnName("PasswordHash").IsRequired()
			builder.Property(Function(t) t.PasswordSalt).HasColumnName("PasswordSalt").IsRequired()
			builder.Property(Function(t) t.rowguid).HasColumnName("rowguid")
			builder.Property(Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();
			builder.HasOne(Function(pw) pw.Person).WithOne(Function(p) p.Password)
		End Sub
	End Module
End Namespace