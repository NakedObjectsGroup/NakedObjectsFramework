﻿Namespace AW.Types

	Partial Public Class SalesTerritoryHistory

		Implements ITitledObject, INotEditableOncePersistent
		'
		'<Hidden>
		Public Property BusinessEntityID() As Integer

#Region "StartDate"
		Public Property mappedStartDate As Date
		Friend myStartDate As NODate

		<DemoProperty(Order:=1)>
		Public ReadOnly Property StartDate As NODate
			Get
				myStartDate = If(myStartDate, New NODate(mappedStartDate, Sub(v) mappedStartDate = v))
Return myStartDate
			End Get
		End Property

		Public Sub AboutStartDate(a As FieldAbout, StartDate As NODate)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case Else
			End Select
		End Sub
#End Region

#Region "EndDate"
		Public Property mappedEndDate As Date?
		Friend myEndDate As NODateNullable

		<DemoProperty(Order:=2)>
		Public ReadOnly Property EndDate As NODateNullable
			Get
				myEndDate = If(myEndDate, New NODateNullable(mappedEndDate, Sub(v) mappedEndDate = v))
Return myEndDate
			End Get
		End Property

		Public Sub AboutEndDate(a As FieldAbout, EndDate As NODateNullable)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case Else
			End Select
		End Sub
#End Region

		<DemoProperty(Order:=3)>
		Public Overridable Property SalesPerson() As SalesPerson

		''<Hidden>
		Public Property SalesTerritoryID() As Integer

		<DemoProperty(Order:=4)>
		Public Overridable Property SalesTerritory() As SalesTerritory

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

		''<Hidden>
		Public Property RowGuid() As Guid

		Public Function Title() As Title Implements ITitledObject.Title
			Return New Title(ToString())
		End Function

		Public Overrides Function ToString() As String
			Return $"{SalesPerson} {SalesTerritory}"
		End Function
	End Class
End Namespace