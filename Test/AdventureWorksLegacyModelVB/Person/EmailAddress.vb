Namespace AW.Types

	Partial Public Class EmailAddress

		Implements ITitledObject, INotEditableOncePersistent

		Public Property BusinessEntityID() As Integer

		Public Property EmailAddressID() As Integer

#Region "EmailAddress1"
		Public Property mappedEmailAddress1 As String
		Friend myEmailAddress1 As TextString

		<AWProperty(Order:=1)>
		Public ReadOnly Property EmailAddress1 As TextString
			Get
				myEmailAddress1 = If(myEmailAddress1, New TextString(mappedEmailAddress1, Sub(v) mappedEmailAddress1 = v))
Return myEmailAddress1
			End Get
		End Property

		Public Sub AboutEmailAddress1(a As FieldAbout, EmailAddress1 As TextString)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
					a.Name = "Email Address"
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case Else
			End Select
		End Sub
#End Region

#Region "ModifiedDate"
		Public Property mappedModifiedDate As Date
		Friend myModifiedDate As TimeStamp

		<AWProperty(Order:=99)>
		Public ReadOnly Property ModifiedDate As TimeStamp
			Get
				myModifiedDate = If(myModifiedDate, New TimeStamp(mappedModifiedDate, Sub(v) mappedModifiedDate = v))
Return myModifiedDate
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

		Public Function Title() As Title Implements ITitledObject.Title
			Return New Title(ToString())
		End Function

		Public Overrides Function ToString() As String
			Return mappedEmailAddress1
		End Function
	End Class
End Namespace