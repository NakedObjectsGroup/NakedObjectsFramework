Namespace AW.Types

	Partial Public Class ProductCostHistory
		Implements IHasModifiedDate

		<Hidden>
		Public Property ProductID() As Integer

		Public Property StartDate() As DateTime
		Public Property EndDate() As DateTime?
		Public Property StandardCost() As Decimal

		<Hidden>
		Public Overridable Property Product() As Product

		<MemberOrder(99)>
		Public Property ModifiedDate() As DateTime Implements IHasModifiedDate.ModifiedDate

		Public Overrides Function ToString() As String
			Return $"{StandardCost} {StartDate}~"
		End Function
	End Class
End Namespace