Namespace AW.Types

	Partial Public Class Password

		Public Property BusinessEntityID() As Integer

		Public Overridable Property Person() As Person

		Public Property PasswordHash() As String 'Not visible on UI

		Public Property PasswordSalt() As String

#Region "ModifiedDate"
		Public mappedModifiedDate As Date
		Friend myModifiedDate As TimeStamp

		Public ReadOnly Property ModifiedDate As TimeStamp
			Get
				Return If(myModifiedDate, New TimeStamp(mappedModifiedDate, Function(v) mappedModifiedDate = v))
			End Get
		End Property

		Public Sub AboutModifiedDate(a As FieldAbout, ModifiedDate As TimeStamp)
			Select Case a.TypeCode
				Case AboutTypeCodes.Visible
					a.Visible = False
			End Select
		End Sub
#End Region

		Public Property RowGuid() As Guid

		Public Function Title() As Title
			Return New Title("Password")
		End Function

	End Class
End Namespace