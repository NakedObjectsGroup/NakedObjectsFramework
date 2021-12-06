Namespace AW.Types

	Partial Public Class SalesPerson

		<Hidden>
		Public Property BusinessEntityID() As Integer

		<MemberOrder(10)>
		Public Overridable Property EmployeeDetails() As Employee

		<MemberOrder(11)>
		Public Overridable ReadOnly Property PersonDetails() As Person
			Get
				Return EmployeeDetails.PersonDetails
			End Get
		End Property

		<Hidden>
		Public Property SalesTerritoryID() As Integer?

		'INSTANT VB WARNING: Nullable reference types have no equivalent in VB:
		'ORIGINAL LINE: public virtual SalesTerritory? SalesTerritory {get;set;}
		<MemberOrder(20)>
		Public Overridable Property SalesTerritory() As SalesTerritory

		<MemberOrder(30), Mask("C")>
		Public Property SalesQuota() As Decimal?

		<MemberOrder(40), Mask("C")>
		Public Property Bonus() As Decimal

		<MemberOrder(50)>
		<Mask("P")>
		Public Property CommissionPct() As Decimal

		<MemberOrder(60), Mask("C")>
		Public Property SalesYTD() As Decimal

		<MemberOrder(70), Mask("C")>
		Public Property SalesLastYear() As Decimal

		<TableView(False, "QuotaDate", "SalesQuota")>
		Public Overridable Property QuotaHistory() As ICollection(Of SalesPersonQuotaHistory) = New List(Of SalesPersonQuotaHistory)()

		<TableView(False, "StartDate", "EndDate", "SalesTerritory")>
		Public Overridable Property TerritoryHistory() As ICollection(Of SalesTerritoryHistory) = New List(Of SalesTerritoryHistory)()

		<MemberOrder(99)>
		Public Property ModifiedDate() As DateTime

		<Hidden>
		Public Property rowguid() As Guid

		Public Overrides Function ToString() As String
			Return $"{EmployeeDetails}"
		End Function
	End Class
End Namespace