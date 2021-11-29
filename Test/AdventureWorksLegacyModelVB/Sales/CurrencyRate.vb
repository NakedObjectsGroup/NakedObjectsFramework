Namespace AW.Types

	Partial Public Class CurrencyRate
		<Hidden>
		Public Property CurrencyRateID() As Integer

		Public Property CurrencyRateDate() As DateTime

		Public Property AverageRate() As Decimal

		Public Property EndOfDayRate() As Decimal

		<Hidden>
		Public Property FromCurrencyCode() As String = ""

		Public Overridable Property Currency() As Currency

		<Hidden>
		Public Property ToCurrencyCode() As String = ""

		Public Overridable Property Currency1() As Currency

		<MemberOrder(99)>
		Public Property ModifiedDate() As DateTime

		Public Overrides Function ToString() As String
			Return $"{AverageRate}"
		End Function
	End Class
End Namespace