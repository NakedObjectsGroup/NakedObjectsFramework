Namespace AW.Types


	Partial Public Class PhoneNumberType
		Implements ITitledObject, IBounded

		Public Property PhoneNumberTypeID() As Integer

#Region "Name"
		Public Property mappedName As String
		Friend myName As TextString

		Public ReadOnly Property Name As TextString
			Get
				myName = If(myName, New TextString(mappedName, Sub(v) mappedName = v))
				Return myName
			End Get
		End Property

		Public Sub AboutName(a As FieldAbout, Name As TextString)
			Select Case a.TypeCode
				Case AboutTypeCodes.Visible
					a.Visible = False
			End Select
		End Sub
#End Region
#Region "ModifiedDate"
		Public Property mappedModifiedDate As Date
		Friend myModifiedDate As TimeStamp

		<MemberOrder(99)>
		Public ReadOnly Property ModifiedDate As TimeStamp
			Get
				myModifiedDate = If(myModifiedDate, New TimeStamp(mappedModifiedDate, Sub(v) mappedModifiedDate = v))
				Return myModifiedDate
			End Get
		End Property

		Public Sub AboutModifiedDate(a As FieldAbout)
			Select Case a.TypeCode
				Case AboutTypeCodes.Visible
					a.Visible = False
			End Select
		End Sub
#End Region

		Public Function Title() As Title Implements ITitledObject.Title
			Return New Title(ToString())
		End Function

		Public Overrides Function ToString() As String
			Return mappedName
		End Function
	End Class
End Namespace