Namespace AW.Types

	<Bounded>
	Partial Public Class ContactType
		Implements IHasModifiedDate

		<Hidden>
		Public Property ContactTypeID() As Integer

		<MemberOrder(1)>
		Public Property Name() As String = ""

		<MemberOrder(99)>
		Public Property ModifiedDate() As DateTime Implements IHasModifiedDate.ModifiedDate

		Public Overrides Function ToString() As String
			Return Name
		End Function

	End Class
End Namespace