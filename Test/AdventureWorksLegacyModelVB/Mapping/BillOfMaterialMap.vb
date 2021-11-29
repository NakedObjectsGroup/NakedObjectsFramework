Imports System.Data.Entity.ModelConfiguration

Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders

Namespace AW.Mapping
	Public Class BillOfMaterialMap
		Inherits EntityTypeConfiguration(Of BillOfMaterial)

		Public Sub New()
			' Primary Key
			HasKey(Function(t) t.BillOfMaterialID)

			' Properties
			[Property](Function(t) t.UnitMeasureCode).IsRequired().IsFixedLength().HasMaxLength(3)

			' Table & Column Mappings
			ToTable("BillOfMaterials", "Production")
			[Property](Function(t) t.BillOfMaterialID).HasColumnName("BillOfMaterialsID")
			[Property](Function(t) t.ProductAssemblyID).HasColumnName("ProductAssemblyID")
			[Property](Function(t) t.ComponentID).HasColumnName("ComponentID")
			[Property](Function(t) t.StartDate).HasColumnName("StartDate")
			[Property](Function(t) t.EndDate).HasColumnName("EndDate")
			[Property](Function(t) t.UnitMeasureCode).HasColumnName("UnitMeasureCode")
			[Property](Function(t) t.BOMLevel).HasColumnName("BOMLevel")
			[Property](Function(t) t.PerAssemblyQty).HasColumnName("PerAssemblyQty")
			[Property](Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();

			' Relationships
			HasOptional(Function(t) t.Product).WithMany().HasForeignKey(Function(t) t.ProductAssemblyID)
			HasRequired(Function(t) t.Product1).WithMany().HasForeignKey(Function(t) t.ComponentID)
			HasRequired(Function(t) t.UnitMeasure).WithMany().HasForeignKey(Function(t) t.UnitMeasureCode)
		End Sub
	End Class

	Public Module Mapper
		<System.Runtime.CompilerServices.Extension> _
		Public Sub Map(ByVal builder As EntityTypeBuilder(Of BillOfMaterial))
			builder.HasKey(Function(t) t.BillOfMaterialID)

			' Properties
			builder.Property(Function(t) t.UnitMeasureCode).IsRequired().IsFixedLength().HasMaxLength(3)

			' Table & Column Mappings
			builder.ToTable("BillOfMaterials", "Production")
			builder.Property(Function(t) t.BillOfMaterialID).HasColumnName("BillOfMaterialsID")
			builder.Property(Function(t) t.ProductAssemblyID).HasColumnName("ProductAssemblyID")
			builder.Property(Function(t) t.ComponentID).HasColumnName("ComponentID")
			builder.Property(Function(t) t.StartDate).HasColumnName("StartDate")
			builder.Property(Function(t) t.EndDate).HasColumnName("EndDate")
			builder.Property(Function(t) t.UnitMeasureCode).HasColumnName("UnitMeasureCode")
			builder.Property(Function(t) t.BOMLevel).HasColumnName("BOMLevel")
			builder.Property(Function(t) t.PerAssemblyQty).HasColumnName("PerAssemblyQty")
			builder.Property(Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();

			' Relationships
			builder.HasOne(Function(t) t.Product).WithMany().HasForeignKey(Function(t) t.ProductAssemblyID)
			builder.HasOne(Function(t) t.Product1).WithMany().HasForeignKey(Function(t) t.ComponentID)
			builder.HasOne(Function(t) t.UnitMeasure).WithMany().HasForeignKey(Function(t) t.UnitMeasureCode)
		End Sub
	End Module
End Namespace