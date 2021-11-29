Namespace AW.Types

	Partial Public Class PersonCreditCard
		<Hidden>
		Public Property PersonID() As Integer

		<Hidden>
		Public Property CreditCardID() As Integer

		Public Overridable Property Person() As Person

		Public Overridable Property CreditCard() As CreditCard

		<MemberOrder(99)>
		Public Property ModifiedDate() As DateTime

		Public Overrides Function ToString() As String
			Return $"PersonCreditCard: {PersonID}-{CreditCardID}"
		End Function
	End Class
End Namespace