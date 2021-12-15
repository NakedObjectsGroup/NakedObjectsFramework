

Namespace AW.Types

	Partial Public Class CreditCard
		'<Hidden>
		Public Property CreditCardID() As Integer

		'<Hidden>
		Public Property CardType() As String = ""

		'<Hidden>
		Public Property CardNumber() As String = ""

		'<Hidden>
		Public Property ExpMonth() As Byte

		'<Hidden>
		Public Property ExpYear() As Short

		<Named("Persons"), MemberOrder(5), TableView(False, NameOf(PersonCreditCard.Person))>
		Public Overridable Property PersonLinks() As ICollection(Of PersonCreditCard) = New List(Of PersonCreditCard)()

#Region "ModifiedDate"
		Friend mappedModifiedDate As Date
		Friend myModifiedDate As TimeStamp

		<MemberOrder(99)>
		Public ReadOnly Property ModifiedDate As TimeStamp
			Get
				Return If(myModifiedDate, New TimeStamp(mappedModifiedDate, Function(v) mappedModifiedDate = v))
			End Get
		End Property

		Public Sub AboutModifiedDate(a As FieldAbout, ModifiedDate As TimeStamp)
			Select Case a.TypeCode
				Case AboutTypeCodes.Usable
					a.Usable = False
			End Select
		End Sub
#End Region

		Public Function Title() As Title
			Return New Title("TODO - Title") 'CreditCard_Functions.ObfuscatedNumber(Me)
		End Function
	End Class
End Namespace