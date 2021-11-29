Imports System.Data.Entity.ModelConfiguration

Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders

Namespace AW.Mapping
	Public Class ProductReviewMap
		Inherits EntityTypeConfiguration(Of ProductReview)

		Public Sub New()
			' Primary Key
			HasKey(Function(t) t.ProductReviewID)

			' Properties
			[Property](Function(t) t.ReviewerName).IsRequired().HasMaxLength(50)

			[Property](Function(t) t.EmailAddress).IsRequired().HasMaxLength(50)

			[Property](Function(t) t.Comments).HasMaxLength(3850)

			' Table & Column Mappings
			ToTable("ProductReview", "Production")
			[Property](Function(t) t.ProductReviewID).HasColumnName("ProductReviewID")
			[Property](Function(t) t.ProductID).HasColumnName("ProductID")
			[Property](Function(t) t.ReviewerName).HasColumnName("ReviewerName")
			[Property](Function(t) t.ReviewDate).HasColumnName("ReviewDate")
			[Property](Function(t) t.EmailAddress).HasColumnName("EmailAddress")
			[Property](Function(t) t.Rating).HasColumnName("Rating")
			[Property](Function(t) t.Comments).HasColumnName("Comments")
			[Property](Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();

			' Relationships
			HasRequired(Function(t) t.Product).WithMany(Function(t) t.ProductReviews).HasForeignKey(Function(d) d.ProductID)
		End Sub
	End Class

	Public Module Mapper
		<System.Runtime.CompilerServices.Extension> _
		Public Sub Map(ByVal builder As EntityTypeBuilder(Of ProductReview))
			builder.HasKey(Function(t) t.ProductReviewID)

			' Properties
			builder.Property(Function(t) t.ReviewerName).IsRequired().HasMaxLength(50)

			builder.Property(Function(t) t.EmailAddress).IsRequired().HasMaxLength(50)

			builder.Property(Function(t) t.Comments).HasMaxLength(3850)

			' Table & Column Mappings
			builder.ToTable("ProductReview", "Production")
			builder.Property(Function(t) t.ProductReviewID).HasColumnName("ProductReviewID")
			builder.Property(Function(t) t.ProductID).HasColumnName("ProductID")
			builder.Property(Function(t) t.ReviewerName).HasColumnName("ReviewerName")
			builder.Property(Function(t) t.ReviewDate).HasColumnName("ReviewDate")
			builder.Property(Function(t) t.EmailAddress).HasColumnName("EmailAddress")
			builder.Property(Function(t) t.Rating).HasColumnName("Rating")
			builder.Property(Function(t) t.Comments).HasColumnName("Comments")
			builder.Property(Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();

			' Relationships
			builder.HasOne(Function(t) t.Product).WithMany(Function(t) t.ProductReviews).HasForeignKey(Function(d) d.ProductID)
		End Sub
	End Module
End Namespace