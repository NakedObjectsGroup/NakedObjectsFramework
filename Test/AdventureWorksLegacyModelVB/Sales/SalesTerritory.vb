Namespace AW.Types

	<Bounded>
	Partial Public Class SalesTerritory
		<Hidden>
		Public Property TerritoryID() As Integer

		<MemberOrder(10)>
		Public Property Name() As String = ""

		<MemberOrder(20)>
		Public Property CountryRegionCode() As String = ""

		<MemberOrder(30)>
		Public Property Group() As String = ""

		<MemberOrder(40)>
		<Mask("C")>
		Public Property SalesYTD() As Decimal

		<MemberOrder(41)>
		<Mask("C")>
		Public Property SalesLastYear() As Decimal

		<MemberOrder(42)>
		<Mask("C")>
		Public Property CostYTD() As Decimal

		<MemberOrder(43)>
		<Mask("C")>
		Public Property CostLastYear() As Decimal

		<Named("States/Provinces covered")>
		<TableView(True)>
		Public Overridable Property StateProvince() As ICollection(Of StateProvince) = New List(Of StateProvince)()

		<Hidden>
		Public Property rowguid() As Guid

		<MemberOrder(99)>
		Public Property ModifiedDate() As DateTime

		Public Overrides Function ToString() As String
			Return Name
		End Function
	End Class
End Namespace