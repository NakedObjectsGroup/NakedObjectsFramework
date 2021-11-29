Namespace AW.Types

	Partial Public Class SalesPersonQuotaHistory
		<Hidden>
		Public Property BusinessEntityID() As Integer

		<MemberOrder(1), Mask("d")>
		Public Property QuotaDate() As DateTime

		<MemberOrder(2), Mask("C")>
		Public Property SalesQuota() As Decimal

		<MemberOrder(3)>
		Public Overridable Property SalesPerson() As SalesPerson

		Public Overrides Function ToString() As String
			Return $"{QuotaDate.ToString("d")} {SalesQuota.ToString("C")}"
		End Function

		<MemberOrder(99)>
		Public Property ModifiedDate() As DateTime

		<Hidden>
		Public Property rowguid() As Guid
	End Class
End Namespace