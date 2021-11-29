Namespace AW.Types

	Partial Public Class ProductListPriceHistory
		Implements IHasModifiedDate

		Public Property ProductID() As Integer
		Public Property StartDate() As DateTime
		Public Property EndDate() As DateTime?
		Public Property ListPrice() As Decimal

		Public Overridable Property Product() As Product

		<MemberOrder(99)>
		Public Property ModifiedDate() As DateTime Implements IHasModifiedDate.ModifiedDate

		Public Overrides Function ToString() As String
			Return $"ProductListPriceHistory: {ProductID}"
		End Function
	End Class
End Namespace