

Namespace AW.Types

	Partial Public Class CreditCard
		<Hidden>
		Public Property CreditCardID() As Integer

		<Hidden>
		Public Property CardType() As String = ""

		<Hidden>
		Public Property CardNumber() As String = ""

		<Hidden>
		Public Property ExpMonth() As Byte

		<Hidden>
		Public Property ExpYear() As Short

		<Named("Persons"), MemberOrder(5), TableView(False, NameOf(PersonCreditCard.Person))>
		Public Overridable Property PersonLinks() As ICollection(Of PersonCreditCard) = New List(Of PersonCreditCard)()

		<MemberOrder(99)>
		Public Property ModifiedDate() As DateTime

		'INSTANT VB WARNING: Nullable reference types have no equivalent in VB:
		'ORIGINAL LINE: public override string? ToString()
		Public Overrides Function ToString() As String
			Return "" 'CreditCard_Functions.ObfuscatedNumber(Me)
		End Function
	End Class
End Namespace