Namespace AW.Types

	<Bounded>
	Partial Public Class SalesReason
		<Hidden>
		Public Property SalesReasonID() As Integer

		Public Property Name() As String = ""

		Public Property ReasonType() As String = ""

		<MemberOrder(99)>
		Public Property ModifiedDate() As DateTime

		Public Overrides Function ToString() As String
			Return Name
		End Function
	End Class
End Namespace