Namespace AW.Types

	Partial Public Class ProductDescription
		Implements IHasRowGuid, IHasModifiedDate

		<Hidden>
		Public Property ProductDescriptionID() As Integer

		<MultiLine(10)>
		<MemberOrder(2)>
		Public Property Description() As String = ""

		<MemberOrder(99)>
		Public Property ModifiedDate() As DateTime Implements IHasModifiedDate.ModifiedDate

		<Hidden>
		Public Property rowguid() As Guid Implements IHasRowGuid.rowguid

		Public Overrides Function ToString() As String
			Return Description
		End Function
	End Class
End Namespace