Namespace AW.Types


	Partial Public Class SalesTerritory
		Implements ITitledObject, INotEditableOncePersistent, IBounded

		'<Hidden>
		Public Property TerritoryID() As Integer

#Region "Name"
		Public Property mappedName As String
		Friend myName As TextString

		<DemoProperty(Order:=10)>
		Public ReadOnly Property Name As TextString
			Get
				myName = If(myName, New TextString(mappedName, Sub(v) mappedName = v))
Return myName
			End Get
		End Property

		Public Sub AboutName(a As FieldAbout, Name As TextString)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case Else
			End Select
		End Sub
#End Region

#Region "CountryRegionCode"
		Public Property mappedCountryRegionCode As String
		Friend myCountryRegionCode As TextString

		<DemoProperty(Order:=20)>
		Public ReadOnly Property CountryRegionCode As TextString
			Get
				myCountryRegionCode = If(myCountryRegionCode, New TextString(mappedCountryRegionCode, Sub(v) mappedCountryRegionCode = v))
Return myCountryRegionCode
			End Get
		End Property

		Public Sub AboutCountryRegionCode(a As FieldAbout, CountryRegionCode As TextString)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case Else
			End Select
		End Sub
#End Region

#Region "Group"
		Public Property mappedGroup As String
		Friend myGroup As TextString

		<DemoProperty(Order:=30)>
		Public ReadOnly Property Group As TextString
			Get
				myGroup = If(myGroup, New TextString(mappedGroup, Sub(v) mappedGroup = v))
Return myGroup
			End Get
		End Property

		Public Sub AboutGroup(a As FieldAbout, Group As TextString)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case Else
			End Select
		End Sub
#End Region

#Region "SalesYTD"
		Public Property mappedSalesYTD As Decimal
		Friend mySalesYTD As Money

		<DemoProperty(Order:=40)>
		Public ReadOnly Property SalesYTD As Money
			Get
				mySalesYTD = If(mySalesYTD, New Money(mappedSalesYTD, Sub(v) mappedSalesYTD = v))
Return mySalesYTD
			End Get
		End Property

		Public Sub AboutSalesYTD(a As FieldAbout, SalesYTD As Money)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case Else
			End Select
		End Sub
#End Region

#Region "SalesLastYear"
		Public Property mappedSalesLastYear As Decimal
		Friend mySalesLastYear As Money

		<DemoProperty(Order:=41)>
		Public ReadOnly Property SalesLastYear As Money
			Get
				mySalesLastYear = If(mySalesLastYear, New Money(mappedSalesLastYear, Sub(v) mappedSalesLastYear = v))
Return mySalesLastYear
			End Get
		End Property

		Public Sub AboutSalesLastYear(a As FieldAbout, SalesLastYear As Money)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case Else
			End Select
		End Sub
#End Region

#Region "CostYTD"
		Public Property mappedCostYTD As Decimal
		Friend myCostYTD As Money

		<DemoProperty(Order:=42)>
		Public ReadOnly Property CostYTD As Money
			Get
				myCostYTD = If(myCostYTD, New Money(mappedCostYTD, Sub(v) mappedCostYTD = v))
Return myCostYTD
			End Get
		End Property

		Public Sub AboutCostYTD(a As FieldAbout, CostYTD As Money)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case Else
			End Select
		End Sub
#End Region

#Region "CostLastYear"
		Public Property mappedCostLastYear As Decimal
		Friend myCostLastYear As Money

		<DemoProperty(Order:=43)>
		Public ReadOnly Property CostLastYear As Money
			Get
				myCostLastYear = If(myCostLastYear, New Money(mappedCostLastYear, Sub(v) mappedCostLastYear = v))
Return myCostLastYear
			End Get
		End Property

		Public Sub AboutCostLastYear(a As FieldAbout, CostLastYear As Money)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case Else
			End Select
		End Sub
#End Region

#Region "StateProvince (Collection)"
		Public Overridable Property mappedStateProvince As ICollection(Of StateProvince) = New List(Of StateProvince)()

		Private myStateProvince As InternalCollection

		<DemoProperty(Order:=1)>
		Public ReadOnly Property StateProvince As InternalCollection
			Get
				myStateProvince = If(myStateProvince, New InternalCollection(Of StateProvince)(mappedStateProvince))
Return myStateProvince
			End Get
		End Property

		Public Sub AboutStateProvince(a As FieldAbout)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
					a.Name = "States/Provinces covered"
				Case Else
			End Select
		End Sub
#End Region

		''<Hidden>
		Public Property RowGuid() As Guid

#Region "ModifiedDate"
		Public Property mappedModifiedDate As Date
		Friend myModifiedDate As TimeStamp

		<DemoProperty(Order:=99)>
		Public ReadOnly Property ModifiedDate As TimeStamp
			Get
				myModifiedDate = If(myModifiedDate, New TimeStamp(mappedModifiedDate, Sub(v) mappedModifiedDate = v))
Return myModifiedDate
			End Get
		End Property

		Public Sub AboutModifiedDate(a As FieldAbout)
			Select Case a.TypeCode
				Case AboutTypeCodes.Usable
					a.Usable = False
			End Select
		End Sub
#End Region


		Public Function Title() As Title Implements ITitledObject.Title
			Return New Title(ToString())
		End Function

		Public Overrides Function ToString() As String
			Return mappedName
		End Function
	End Class
End Namespace