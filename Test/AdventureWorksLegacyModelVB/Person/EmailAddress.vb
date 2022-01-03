Namespace AW.Types

	Partial Public Class EmailAddress

		Public Property BusinessEntityID() As Integer

		Public Property EmailAddressID() As Integer

#Region "EmailAddress1"
		Public mappedEmailAddress1 As String
		Friend myEmailAddress1 As TextString

		'<MemberOrder(1)>
		Private ReadOnly Property EmailAddress1 As TextString
			Get
				Return If(myEmailAddress1, New TextString(mappedEmailAddress1, Function(v) mappedEmailAddress1 = v))
			End Get
		End Property

		Public Sub AboutEmailAddress1(a As FieldAbout, EmailAddress1 As TextString)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
					a.Name = "Email Address"
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region

#Region "ModifiedDate"
		Public mappedModifiedDate As Date
		Friend myModifiedDate As TimeStamp

		'<MemberOrder(99)>
		Private ReadOnly Property ModifiedDate As TimeStamp
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

		Public Property RowGuid() As Guid

		Public Function Title() As Title
			Return New Title(EmailAddress1)
		End Function
	End Class
End Namespace