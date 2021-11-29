Namespace AW.Types

	<Bounded>
	Partial Public Class StateProvince
		Implements IHasRowGuid, IHasModifiedDate

		<Hidden>
		Public Property StateProvinceID() As Integer

		Public Property StateProvinceCode() As String = ""

		Public Property IsOnlyStateProvinceFlag() As Boolean

		Public Property Name() As String = ""

		<Hidden>
		Public Property CountryRegionCode() As String = ""

		Public Overridable Property CountryRegion() As CountryRegion

		<Hidden>
		Public Property TerritoryID() As Integer

		Public Overridable Property SalesTerritory() As SalesTerritory

		<MemberOrder(99)>
		Public Property ModifiedDate() As DateTime Implements IHasModifiedDate.ModifiedDate

		<Hidden>
		Public Property rowguid() As Guid Implements IHasRowGuid.rowguid

		Public Overrides Function ToString() As String
			Return Name
		End Function
	End Class
End Namespace