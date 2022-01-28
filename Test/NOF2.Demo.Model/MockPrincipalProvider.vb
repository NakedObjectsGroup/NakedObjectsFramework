Imports System.Security.Principal


Namespace AW
	Public Class MockPrincipalProvider
		Implements IPrincipalProvider

		Public ReadOnly Property CurrentUser() As IPrincipal
			Get
				Return New GenericPrincipal(New GenericIdentity("adventure-works\ken0"), { "root" })
			End Get
		End Property
	End Class
End Namespace