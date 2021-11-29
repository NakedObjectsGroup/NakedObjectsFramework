Namespace AW.Types

	Partial Public Class TransactionHistory
		Public Property TransactionID() As Integer

		Public Property ReferenceOrderID() As Integer

		Public Property ReferenceOrderLineID() As Integer

		Public Property TransactionDate() As DateTime

		Public Property TransactionType() As String = ""

		Public Property Quantity() As Integer

		Public Property ActualCost() As Decimal

		<Hidden>
		Public Property ProductID() As Integer

		Public Overridable Property Product() As Product

		<MemberOrder(99)>
		Public Property ModifiedDate() As DateTime

		Public Overrides Function ToString() As String
			Return $"TransactionHistory: {TransactionID}"
		End Function
	End Class
End Namespace