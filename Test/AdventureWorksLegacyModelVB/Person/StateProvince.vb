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

#Region "ModifiedDate"
		Friend mappedModifiedDate As Date
		Friend myModifiedDate As TimeStamp

		<MemberOrder(1)>
		Public ReadOnly Property ModifiedDate As TimeStamp Implements IHasModifiedDate.ModifiedDate
			Get
				Return If(myModifiedDate, New TimeStamp(mappedModifiedDate, Function(v) mappedModifiedDate = v))
			End Get
		End Property

		Public Sub AboutModifiedDate(a As FieldAbout, ModifiedDate As TimeStamp)
			Select Case a.TypeCode
				Case AboutTypeCodes.Usable
					a.Usable = False
			End Select
		End Sub
#End Region

		<Hidden>
		Public Property rowguid() As Guid Implements IHasRowGuid.rowguid

		Public Overrides Function ToString() As String
			Return Name
		End Function
	End Class
End Namespace