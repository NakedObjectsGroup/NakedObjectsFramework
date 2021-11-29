Namespace AW.Types

	<Bounded>
	Partial Public Class CountryRegion
		Implements IHasModifiedDate

		<MemberOrder(1)>
		Public Property Name() As String = ""

		<MemberOrder(2)>
		Public Property CountryRegionCode() As String = ""

		<MemberOrder(99)>
		Public Property ModifiedDate() As DateTime Implements IHasModifiedDate.ModifiedDate

		Public Overrides Function ToString() As String
			Return Name
		End Function
	End Class
End Namespace