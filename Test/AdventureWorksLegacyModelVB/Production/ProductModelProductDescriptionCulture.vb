Namespace AW.Types

	Partial Public Class ProductModelProductDescriptionCulture
		Implements IHasModifiedDate

		<Hidden>
		Public Property ProductModelID() As Integer

		<Hidden>
		Public Property ProductDescriptionID() As Integer

		<Hidden>
		Public Property CultureID() As String = ""

		Public Overridable Property Culture() As Culture

		Public Overridable Property ProductDescription() As ProductDescription

		<Hidden>
		Public Overridable Property ProductModel() As ProductModel

		<MemberOrder(99)>
		Public Property ModifiedDate() As DateTime Implements IHasModifiedDate.ModifiedDate

		Public Overrides Function ToString() As String
			Return $"ProductModelProductDescriptionCulture: {ProductModelID}-{ProductDescriptionID}-{CultureID}"
		End Function
	End Class
End Namespace