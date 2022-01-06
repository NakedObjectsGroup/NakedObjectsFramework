Namespace AW.Types

	'<Bounded>
	Partial Public Class Currency
 Implements ITitledObject
		'<Hidden>
		Public Property CurrencyCode() As String = ""

		'<Hidden>
		Public Property Name() As String = ""

		'<Hidden>
		Public Property ModifiedDate() As Date

		Public Function Title() As Title Implements ITitledObject.Title
			Return New Title($"{CurrencyCode} - {Name}")
		End Function
	End Class
End Namespace