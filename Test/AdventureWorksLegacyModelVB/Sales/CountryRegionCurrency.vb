Namespace AW.Types

	Partial Public Class CountryRegionCurrency
		Public Property CountryRegionCode() As String = ""

		Public Property CurrencyCode() As String = ""

		Public Overridable Property CountryRegion() As CountryRegion

		Public Overridable Property Currency() As Currency

		
		Public Property ModifiedDate() As DateTime

		Public Overrides Function ToString() As String
			Return $"CountryRegionCurrency: {CountryRegion} {Currency}"
		End Function
	End Class
End Namespace