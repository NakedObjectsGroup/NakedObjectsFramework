Namespace AW.Types

	<Bounded>
	Partial Public Class UnitMeasure
		Implements IHasModifiedDate

		<MemberOrder(10)>
		Public Property UnitMeasureCode() As String = ""

		<MemberOrder(20)>
		Public Property Name() As String = ""

		<MemberOrder(99)>
		Public Property ModifiedDate() As DateTime Implements IHasModifiedDate.ModifiedDate

		Public Overrides Function ToString() As String
			Return Name
		End Function
	End Class
End Namespace