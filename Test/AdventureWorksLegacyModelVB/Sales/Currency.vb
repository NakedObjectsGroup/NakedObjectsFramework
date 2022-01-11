Namespace AW.Types


	Partial Public Class Currency
		Implements ITitledObject, IBounded

		'<Hidden>
		Public Property CurrencyCode() As String = ""

		'<Hidden>
		Public Property Name() As String = ""

		'<Hidden>
		Public Property ModifiedDate() As Date

		Public Function Title() As ITitle Implements ITitledObject.Title
			Return New Title(ToString())
		End Function

		Public Overrides Function ToString() As String
			Return $"{CurrencyCode} - {Name}"
		End Function
	End Class
End Namespace