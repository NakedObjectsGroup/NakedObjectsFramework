Namespace AW.Types

	Partial Public Class SalesTerritoryHistory

		Implements ITitledObject
		'
		'<Hidden>
		Public Property BusinessEntityID() As Integer

#Region "StartDate"
		Public Property mappedStartDate As Date
		Friend myStartDate As NODate

		<MemberOrder(1)>
		Public ReadOnly Property StartDate As NODate
			Get
				Return If(myStartDate, New NODate(mappedStartDate, Function(v) mappedStartDate = v))
			End Get
		End Property

		Public Sub AboutStartDate(a As FieldAbout, StartDate As NODate)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region

#Region "EndDate"
		Public Property mappedEndDate As Date?
		Friend myEndDate As NODateNullable

		<MemberOrder(2)>
		Public ReadOnly Property EndDate As NODateNullable
			Get
				Return If(myEndDate, New NODateNullable(mappedEndDate, Function(v) mappedEndDate = v))
			End Get
		End Property

		Public Sub AboutEndDate(a As FieldAbout, EndDate As NODateNullable)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region

		<MemberOrder(3)>
		Public Overridable Property SalesPerson() As SalesPerson

		''<Hidden>
		Public Property SalesTerritoryID() As Integer

		<MemberOrder(4)>
		Public Overridable Property SalesTerritory() As SalesTerritory

#Region "ModifiedDate"
		Public Property mappedModifiedDate As Date
		Friend myModifiedDate As TimeStamp

		<MemberOrder(99)>
		Public ReadOnly Property ModifiedDate As TimeStamp
			Get
				Return If(myModifiedDate, New TimeStamp(mappedModifiedDate, Function(v) mappedModifiedDate = v))
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
			Return $"{SalesPerson} {SalesTerritory}"
		End Function
	End Class
End Namespace