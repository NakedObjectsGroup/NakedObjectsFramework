

Namespace AW.Types

	Partial Public Class CreditCard

		Implements ITitledObject
		''<Hidden>
		Public Property CreditCardID() As Integer

		''<Hidden>
		Public Property CardType() As String = ""

		''<Hidden>
		Public Property CardNumber() As String = ""

		''<Hidden>
		Public Property ExpMonth() As Byte

		''<Hidden>
		Public Property ExpYear() As Short

#Region "PersonLinks (Collection)"
		Public Overridable Property mappedPersonLinks As ICollection(Of PersonCreditCard) = New List(Of PersonCreditCard)()

		Private myPersonLinks As InternalCollection

		'<TableView(False, NameOf(PersonCreditCard.Person))>
		'<MemberOrder(5)>
		Public ReadOnly Property PersonLinks As InternalCollection
			Get
				Return If(myPersonLinks, New InternalCollection(Of PersonCreditCard)(mappedPersonLinks))
			End Get
		End Property

		Public Sub AboutPersonLinks(a As FieldAbout)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
					a.Name = "Persons"
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region

#Region "ModifiedDate"
		Public mappedModifiedDate As Date
		Friend myModifiedDate As TimeStamp

		'<MemberOrder(99)>
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

		Public Function Title() As Title Implements ITitledObject.Title
			Return New Title(ToString())
		End Function

		Public Overrides Function ToString() As String
			Return "TODO - Title" 'CreditCard_Functions.ObfuscatedNumber(Me
		End Function
	End Class
End Namespace