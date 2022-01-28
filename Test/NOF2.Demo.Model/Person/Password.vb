Namespace AW.Types

	Partial Public Class Password
		Implements ITitledObject, INotEditableOncePersistent

		Public Property BusinessEntityID() As Integer

		Public Overridable Property Person() As Person

		Public Property PasswordHash() As String 'Not visible on UI

		Public Property PasswordSalt() As String

#Region "ModifiedDate"
		Public Property mappedModifiedDate As Date
		Friend myModifiedDate As TimeStamp

		Public ReadOnly Property ModifiedDate As TimeStamp
			Get
				myModifiedDate = If(myModifiedDate, New TimeStamp(mappedModifiedDate, Sub(v) mappedModifiedDate = v))
Return myModifiedDate
			End Get
		End Property

		Public Sub AboutModifiedDate(a As FieldAbout)
			Select Case a.TypeCode
				Case Else
					a.Visible = False
			End Select
		End Sub
#End Region

		Public Property RowGuid() As Guid

		Public Function Title() As Title Implements ITitledObject.Title
			Return New Title(ToString())
		End Function

		Public Overrides Function ToString() As String
			Return "Password"
		End Function

	End Class
End Namespace