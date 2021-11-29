Namespace AW.Types

	<Bounded>
	Partial Public Class ProductSubcategory
		Implements IHasRowGuid, IHasModifiedDate

		<Hidden>
		Public Property ProductSubcategoryID() As Integer

		Public Property Name() As String = ""

		<Hidden>
		Public Property ProductCategoryID() As Integer

		Public Overridable Property ProductCategory() As ProductCategory

		<MemberOrder(99)>
		Public Property ModifiedDate() As DateTime Implements IHasModifiedDate.ModifiedDate

		<Hidden>
		Public Property rowguid() As Guid Implements IHasRowGuid.rowguid

		Public Overrides Function ToString() As String
			Return Name
		End Function
	End Class
End Namespace