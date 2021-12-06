Namespace AW.Types

	Partial Public Class SalesTerritoryHistory

		<Hidden>
		Public Property BusinessEntityID() As Integer

		<MemberOrder(1), Mask("d")>
		Public Property StartDate() As DateTime

		<MemberOrder(2), Mask("d")>
		Public Property EndDate() As DateTime?

		<MemberOrder(3)>
		Public Overridable Property SalesPerson() As SalesPerson

		<Hidden>
		Public Property SalesTerritoryID() As Integer

		<MemberOrder(4)>
		Public Overridable Property SalesTerritory() As SalesTerritory

		<MemberOrder(99)>
		Public Property ModifiedDate() As DateTime

		<Hidden>
		Public Property rowguid() As Guid

		Public Overrides Function ToString() As String
			Return $"{SalesPerson} {SalesTerritory}"
		End Function
	End Class
End Namespace