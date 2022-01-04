Namespace AW.Types

	'<Bounded>
	Partial Public Class SalesTerritory
		'<Hidden>
		Public Property TerritoryID() As Integer

#Region "Name"
		Public mappedName As String
		Friend myName As TextString

		'<MemberOrder(10)>
		Public ReadOnly Property Name As TextString
			Get
				Return If(myName, New TextString(mappedName, Function(v) mappedName = v))
			End Get
		End Property

		Public Sub AboutName(a As FieldAbout, Name As TextString)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region

#Region "CountryRegionCode"
		Public mappedCountryRegionCode As String
		Friend myCountryRegionCode As TextString

		'<MemberOrder(20)>
		Public ReadOnly Property CountryRegionCode As TextString
			Get
				Return If(myCountryRegionCode, New TextString(mappedCountryRegionCode, Function(v) mappedCountryRegionCode = v))
			End Get
		End Property

		Public Sub AboutCountryRegionCode(a As FieldAbout, CountryRegionCode As TextString)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region

#Region "Group"
		Public mappedGroup As String
		Friend myGroup As TextString

		'<MemberOrder(30)>
		Public ReadOnly Property Group As TextString
			Get
				Return If(myGroup, New TextString(mappedGroup, Function(v) mappedGroup = v))
			End Get
		End Property

		Public Sub AboutGroup(a As FieldAbout, Group As TextString)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region

#Region "SalesYTD"
		Public mappedSalesYTD As Decimal
		Friend mySalesYTD As Money

		'<MemberOrder(40)>
		Public ReadOnly Property SalesYTD As Money
			Get
				Return If(mySalesYTD, New Money(mappedSalesYTD, Function(v) mappedSalesYTD = v))
			End Get
		End Property

		Public Sub AboutSalesYTD(a As FieldAbout, SalesYTD As Money)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region

#Region "SalesLastYear"
		Public mappedSalesLastYear As Decimal
		Friend mySalesLastYear As Money

		'<MemberOrder(41)>
		Public ReadOnly Property SalesLastYear As Money
			Get
				Return If(mySalesLastYear, New Money(mappedSalesLastYear, Function(v) mappedSalesLastYear = v))
			End Get
		End Property

		Public Sub AboutSalesLastYear(a As FieldAbout, SalesLastYear As Money)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region

#Region "CostYTD"
		Public mappedCostYTD As Decimal
		Friend myCostYTD As Money

		'<MemberOrder(42)>
		Public ReadOnly Property CostYTD As Money
			Get
				Return If(myCostYTD, New Money(mappedCostYTD, Function(v) mappedCostYTD = v))
			End Get
		End Property

		Public Sub AboutCostYTD(a As FieldAbout, CostYTD As Money)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region

#Region "CostLastYear"
		Public mappedCostLastYear As Decimal
		Friend myCostLastYear As Money

		'<MemberOrder(43)>
		Public ReadOnly Property CostLastYear As Money
			Get
				Return If(myCostLastYear, New Money(mappedCostLastYear, Function(v) mappedCostLastYear = v))
			End Get
		End Property

		Public Sub AboutCostLastYear(a As FieldAbout, CostLastYear As Money)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region

#Region "StateProvince (Collection)"
		Public Overridable Property mappedStateProvince As ICollection(Of StateProvince) = New List(Of StateProvince)()

		Private myStateProvince As InternalCollection

		'<MemberOrder(1)>
		Public ReadOnly Property StateProvince As InternalCollection
			Get
				Return If(myStateProvince, New InternalCollection(Of StateProvince)(mappedStateProvince))
			End Get
		End Property

		Public Sub AboutStateProvince(a As FieldAbout)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
					a.Name = "States/Provinces covered"
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region

		''<Hidden>
		Public Property RowGuid() As Guid

#Region "ModifiedDate"
		Public mappedModifiedDate As Date
		Friend myModifiedDate As TimeStamp

		'<MemberOrder(99)>
		Public ReadOnly Property ModifiedDate As TimeStamp
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


		Public Function Title() As Title
			Return New Title(Name)
		End Function
	End Class
End Namespace