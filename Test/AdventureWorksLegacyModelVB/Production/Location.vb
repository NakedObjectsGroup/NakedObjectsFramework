Namespace AW.Types

	<Bounded, PresentationHint("Topaz")>
	Partial Public Class Location
		<Hidden>
		Public Property LocationID() As Short

		Public Property Name() As String = ""

		<Mask("C")>
		Public Property CostRate() As Decimal

		<Mask("########.##")>
		Public Property Availability() As Decimal

		<MemberOrder(99)>
		Public Property ModifiedDate() As DateTime

		Public Overrides Function ToString() As String
			Return Name
		End Function
	End Class
End Namespace