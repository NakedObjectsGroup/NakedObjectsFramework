Namespace AW.Types

	Partial Public Class EmployeePayHistory
		<Hidden>
		Public Property EmployeeID() As Integer

		<MemberOrder(1), Mask("d")>
		Public Property RateChangeDate() As DateTime

		<MemberOrder(2), Mask("C")>
		Public Property Rate() As Decimal

		<MemberOrder(3)>
		Public Property PayFrequency() As Byte

		<MemberOrder(4)>
		Public Overridable Property Employee() As Employee

		<MemberOrder(99)>
		Public Property ModifiedDate() As DateTime

		Public Overrides Function ToString() As String
			Return $"{Rate.ToString("C")} from {RateChangeDate.ToString("d")}"
		End Function
	End Class
End Namespace