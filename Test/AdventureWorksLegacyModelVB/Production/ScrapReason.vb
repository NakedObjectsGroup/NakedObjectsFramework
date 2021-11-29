Namespace AW.Types

	<Bounded>
	Partial Public Class ScrapReason

		<Hidden>
		Public Property ScrapReasonID() As Short

		Public Property Name() As String = ""

		<MemberOrder(99)>
		Public Property ModifiedDate() As DateTime

		Public Overrides Function ToString() As String
			Return Name
		End Function
	End Class
End Namespace