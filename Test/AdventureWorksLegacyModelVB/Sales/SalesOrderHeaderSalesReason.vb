Namespace AW.Types

	<Named("Reason")>
	Partial Public Class SalesOrderHeaderSalesReason
		<Hidden>
		Public Property SalesOrderID() As Integer

		Public Property SalesReasonID() As Integer

		Public Overridable Property SalesOrderHeader() As SalesOrderHeader

		Public Overridable Property SalesReason() As SalesReason

		<MemberOrder(99)>
		Public Property ModifiedDate() As DateTime

		Public Overrides Function ToString() As String
			Return $"SalesOrderHeaderSalesReason: {SalesOrderID}-{SalesReasonID}"
		End Function
	End Class
End Namespace