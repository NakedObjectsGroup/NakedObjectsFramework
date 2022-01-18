Namespace AW.Types

	Partial Public Class SalesPerson

		Implements ITitledObject

		''<Hidden>
		Public Property BusinessEntityID() As Integer

		<MemberOrder(10)>
		Public Overridable Property EmployeeDetails() As Employee

		<MemberOrder(11)>
		Public Overridable ReadOnly Property PersonDetails() As Person
			Get
				Return EmployeeDetails.PersonDetails
			End Get
		End Property

		''<Hidden>
		Public Property SalesTerritoryID() As Integer?

		<MemberOrder(20)>
		Public Overridable Property SalesTerritory() As SalesTerritory

#Region "SalesQuota"
		Public Property mappedSalesQuota As Decimal?
		Friend mySalesQuota As MoneyNullable

		<MemberOrder(30)>
		Public ReadOnly Property SalesQuota As MoneyNullable
			Get
				Return If(mySalesQuota, New MoneyNullable(mappedSalesQuota, Sub(v) mappedSalesQuota = v))
			End Get
		End Property

		Public Sub AboutSalesQuota(a As FieldAbout, SalesQuota As MoneyNullable)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region

#Region "Bonus"
		Public Property mappedBonus As Decimal
		Friend myBonus As Money

		<MemberOrder(40)>
		Public ReadOnly Property Bonus As Money
			Get
				Return If(myBonus, New Money(mappedBonus, Sub(v) mappedBonus = v))
			End Get
		End Property

		Public Sub AboutBonus(a As FieldAbout, Bonus As Money)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region

#Region "CommissionPct"
		Public Property mappedCommissionPct As Decimal
		Friend myCommissionPct As Percentage

		<MemberOrder(50)>
		Public ReadOnly Property CommissionPct As Percentage
			Get
				Return If(myCommissionPct, New Percentage(mappedCommissionPct, Sub(v) mappedCommissionPct = v))
			End Get
		End Property

		Public Sub AboutCommissionPct(a As FieldAbout, CommissionPct As Percentage)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region

#Region "SalesYTD"
		Public Property mappedSalesYTD As Decimal
		Friend mySalesYTD As Money

		<MemberOrder(60)>
		Public ReadOnly Property SalesYTD As Money
			Get
				Return If(mySalesYTD, New Money(mappedSalesYTD, Sub(v) mappedSalesYTD = v))
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
		Public Property mappedSalesLastYear As Decimal
		Friend mySalesLastYear As Money

		<MemberOrder(70)>
		Public ReadOnly Property SalesLastYear As Money
			Get
				Return If(mySalesLastYear, New Money(mappedSalesLastYear, Sub(v) mappedSalesLastYear = v))
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

#Region "QuotaHistory (Collection)"
		Public Overridable Property mappedQuotaHistory As ICollection(Of SalesPersonQuotaHistory) = New List(Of SalesPersonQuotaHistory)()

		Private myQuotaHistory As InternalCollection

		''<TableView(False, "QuotaDate", "SalesQuota")>
		<MemberOrder(1)>
		Public ReadOnly Property QuotaHistory As InternalCollection
			Get
				Return If(myQuotaHistory, New InternalCollection(Of SalesPersonQuotaHistory)(mappedQuotaHistory))
			End Get
		End Property

		Public Sub AboutQuotaHistory(a As FieldAbout)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region

#Region "TerritoryHistory (Collection)"
		Public Overridable Property mappedTerritoryHistory As ICollection(Of SalesTerritoryHistory) = New List(Of SalesTerritoryHistory)()

		Private myTerritoryHistory As InternalCollection

		'		'<TableView(False, "StartDate", "EndDate", "SalesTerritory")>
		<MemberOrder(1)>
		Public ReadOnly Property TerritoryHistory As InternalCollection
			Get
				Return If(myTerritoryHistory, New InternalCollection(Of SalesTerritoryHistory)(mappedTerritoryHistory))
			End Get
		End Property

		Public Sub AboutTerritoryHistory(a As FieldAbout)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region

#Region "ModifiedDate"
		Public Property mappedModifiedDate As Date
		Friend myModifiedDate As TimeStamp

		<MemberOrder(99)>
		Public ReadOnly Property ModifiedDate As TimeStamp
			Get
				Return If(myModifiedDate, New TimeStamp(mappedModifiedDate, Sub(v) mappedModifiedDate = v))
			End Get
		End Property

		Public Sub AboutModifiedDate(a As FieldAbout)
			Select Case a.TypeCode
				Case AboutTypeCodes.Usable
					a.Usable = False
			End Select
		End Sub
#End Region

		''<Hidden>
		Public Property RowGuid() As Guid

		Public Function Title() As ITitle Implements ITitledObject.Title
			Return New Title(ToString())
		End Function

		Public Overrides Function ToString() As String
			Return $"{EmployeeDetails}"
		End Function
	End Class
End Namespace