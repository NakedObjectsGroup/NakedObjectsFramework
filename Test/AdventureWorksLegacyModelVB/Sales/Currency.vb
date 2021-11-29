Namespace AW.Types

	<Bounded>
	Partial Public Class Currency
		<Hidden>
		Public Property CurrencyCode() As String = ""

		<Hidden>
		Public Property Name() As String = ""

		<Hidden>
		Public Property ModifiedDate() As DateTime

		Public Overrides Function ToString() As String
			Return $"{CurrencyCode} - {Name}"
		End Function
	End Class
End Namespace