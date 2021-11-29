Namespace AW.Types

	<Bounded>
	Partial Public Class ShipMethod

		<Hidden>
		Public Property ShipMethodID() As Integer

		<MemberOrder(1)>
		Public Property Name() As String = ""

		<MemberOrder(2)>
		Public Property ShipBase() As Decimal

		<MemberOrder(3)>
		Public Property ShipRate() As Decimal

		<MemberOrder(99)>
		Public Property ModifiedDate() As DateTime

		<Hidden>
		Public Property rowguid() As Guid

		Public Overrides Function ToString() As String
			Return Name
		End Function
	End Class
End Namespace