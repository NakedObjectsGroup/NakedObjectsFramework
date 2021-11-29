Namespace AW.Types

	<Bounded>
	Partial Public Class Culture
		Implements IHasModifiedDate

		<Hidden>
		Public Property CultureID() As String = ""

		<MemberOrder(10)>
		Public Property Name() As String = ""

		<MemberOrder(99)>
		Public Property ModifiedDate() As DateTime Implements IHasModifiedDate.ModifiedDate

		Public Overrides Function ToString() As String
			Return Name
		End Function
	End Class
End Namespace