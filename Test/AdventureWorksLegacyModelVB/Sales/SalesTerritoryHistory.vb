Namespace AW.Types

	Partial Public Class SalesTerritoryHistory
		Public Sub New()
		End Sub

		Public Sub New(ByVal cloneFrom As SalesTerritoryHistory)
			BusinessEntityID = cloneFrom.BusinessEntityID
			StartDate = cloneFrom.StartDate
			EndDate = cloneFrom.EndDate
			SalesPerson = cloneFrom.SalesPerson
			SalesTerritoryID = cloneFrom.SalesTerritoryID
			SalesTerritory = cloneFrom.SalesTerritory
			ModifiedDate = cloneFrom.ModifiedDate
			rowguid = cloneFrom.rowguid
		End Sub

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