Namespace AW.Types

	Partial Public Class SalesTaxRate
		<Hidden>
		Public Property SalesTaxRateID() As Integer

		Public Property TaxType() As Byte

		Public Property TaxRate() As Decimal

		Public Property Name() As String = ""

		<Hidden>
		Public Property StateProvinceID() As Integer

		Public Overridable Property StateProvince() As StateProvince

		<MemberOrder(99)>
		Public Property ModifiedDate() As DateTime

		Public Property rowguid() As Guid

		Public Overrides Function ToString() As String
			Return $"Sales Tax Rate for {StateProvince}"
		End Function
	End Class
End Namespace