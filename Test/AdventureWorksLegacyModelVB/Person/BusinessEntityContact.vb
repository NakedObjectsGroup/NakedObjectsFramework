Namespace AW.Types
	'<Named("Contact")>
	Partial Public Class BusinessEntityContact

		Implements ITitledObject

		Public Property BusinessEntityID() As Integer

		Public Overridable Property BusinessEntity() As BusinessEntity

		Public Sub AboutBusinessEntity(a As FieldAbout)
			Select Case a.TypeCode
				Case AboutTypeCodes.Visible
					a.Visible = False
			End Select
		End Sub

		Public Property PersonID() As Integer

		<MemberOrder(1)>
		Public Overridable Property Person() As Person

		Public Property ContactTypeID() As Integer

		<MemberOrder(2)>
		Public Overridable Property ContactType() As ContactType

#Region "ModifiedDate"
		Public Property mappedModifiedDate As Date
		Friend myModifiedDate As TimeStamp

		<MemberOrder(99)>
		Public ReadOnly Property ModifiedDate As TimeStamp
			Get
				Return If(myModifiedDate, New TimeStamp(mappedModifiedDate, Sub(v) mappedModifiedDate = v))
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
			Return Person.ToString()
		End Function
	End Class
End Namespace