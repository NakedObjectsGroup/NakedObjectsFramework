Namespace AW.Types

	Partial Public Class Password

		<Hidden>
		Public Property BusinessEntityID() As Integer

		<Hidden>
		Public Overridable Property Person() As Person

		<Hidden>
		Public Property PasswordHash() As String = ""

		<Hidden>
		Public Property PasswordSalt() As String = ""

#Region "ModifiedDate"
		Friend mappedModifiedDate As Date
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
		<Hidden>
		Public Property RowGuid() As Guid

		Public Overrides Function ToString() As String
			Return "Password"
		End Function
	End Class
End Namespace