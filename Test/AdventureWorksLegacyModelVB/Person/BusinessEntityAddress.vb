Namespace AW.Types

	'<Named("Address")>
	Partial Public Class BusinessEntityAddress

		Implements ITitledObject

		Public Property BusinessEntityID() As Integer

		<MemberOrder(3)>
		Public Overridable Property BusinessEntity() As BusinessEntity

		Public Property AddressTypeID() As Integer

		<MemberOrder(1)>
		Public Overridable Property AddressType() As AddressType

		'<Hidden>
		Public Property AddressID() As Integer

		<MemberOrder(2)>
		Public Overridable Property Address() As Address

#Region "ModifiedDate"
		Public Property mappedModifiedDate As Date
		Friend myModifiedDate As TimeStamp

		<MemberOrder(99)>
		Public ReadOnly Property ModifiedDate As TimeStamp
			Get
				Return If(myModifiedDate, New TimeStamp(mappedModifiedDate, Function(v) mappedModifiedDate = v))
			End Get
		End Property

		Public Sub AboutModifiedDate(a As FieldAbout)
			Select Case a.TypeCode
				Case AboutTypeCodes.Usable
					a.Usable = False
			End Select
		End Sub
#End Region

		Public Property RowGuid() As Guid

		Public Function Title() As ITitle Implements ITitledObject.Title
			Return New Title(ToString())
		End Function

		Public Overrides Function ToString() As String
			Return $"{AddressType}: {Address}"
		End Function
	End Class
End Namespace