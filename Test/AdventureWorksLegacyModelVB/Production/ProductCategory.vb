Namespace AW.Types

	<Bounded>
	Partial Public Class ProductCategory
		Implements IHasRowGuid, IHasModifiedDate

		<Hidden>
		Public Property ProductCategoryID() As Integer

		Public Property Name() As String = ""

		<Named("Subcategories")>
		<TableView(True)>
		Public Overridable Property ProductSubcategory() As ICollection(Of ProductSubcategory) = New List(Of ProductSubcategory)()

		<MemberOrder(99)>
		Public Property ModifiedDate() As DateTime Implements IHasModifiedDate.ModifiedDate

		<Hidden>
		Public Property rowguid() As Guid Implements IHasRowGuid.rowguid

		Public Overrides Function ToString() As String
			Return Name
		End Function
	End Class
End Namespace