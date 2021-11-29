Namespace AW.Types

	Partial Public Class Password
		Implements IHasRowGuid, IHasModifiedDate

		Public Sub New()
		End Sub

		Public Sub New(ByVal cloneFrom As Password)
			BusinessEntityID = cloneFrom.BusinessEntityID
			Person = cloneFrom.Person
			PasswordHash = cloneFrom.PasswordHash
			PasswordSalt = cloneFrom.PasswordSalt
			ModifiedDate = cloneFrom.ModifiedDate
			rowguid = cloneFrom.rowguid
		End Sub

		<Hidden>
		Public Property BusinessEntityID() As Integer

		<Hidden>
		Public Overridable Property Person() As Person

		<Hidden>
		Public Property PasswordHash() As String = ""

		<Hidden>
		Public Property PasswordSalt() As String = ""

		<Hidden>
		Public Property ModifiedDate() As DateTime Implements IHasModifiedDate.ModifiedDate

		<Hidden>
		Public Property rowguid() As Guid Implements IHasRowGuid.rowguid

		Public Overrides Function ToString() As String
			Return "Password"
		End Function
	End Class
End Namespace