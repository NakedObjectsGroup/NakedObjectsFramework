﻿Namespace AW.Types

	Partial Public Class BillOfMaterial

		Public Property BillOfMaterialID() As Integer

#Region "StartDate"
		Friend mappedStartDate As Date
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

#Region "PropName"
		Friend mappedPropName As DateTime?
		Friend myPropName As NODate

		<MemberOrder(1)>
		Public ReadOnly Property PropName As NODate
			Get
				Return If(myPropName, New NODate(mappedPropName, Function(v) mappedPropName = v))
			End Get
		End Property

		Public Sub AboutPropName(a As FieldAbout, PropName As NODate)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region

#Region "BOMLevel"
		Friend mappedBOMLevel As Short
		Friend myBOMLevel As WholeNumber

		<MemberOrder(1)>
		Public ReadOnly Property BOMLevel As WholeNumber
			Get
				Return If(myBOMLevel, New WholeNumber(mappedBOMLevel, Function(v) mappedBOMLevel = v))
			End Get
		End Property

		Public Sub AboutBOMLevel(a As FieldAbout, BOMLevel As WholeNumber)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region

#Region "PerAssemblyQty"
		Friend mappedPerAssemblyQty As Decimal
		Friend myPerAssemblyQty As Money 'TODO: needs a new value type

		<MemberOrder(1)>
		Public ReadOnly Property PerAssemblyQty As Money
			Get
				Return If(myPerAssemblyQty, New Money(mappedPerAssemblyQty, Function(v) mappedPerAssemblyQty = v))
			End Get
		End Property

		Public Sub AboutPerAssemblyQty(a As FieldAbout, PerAssemblyQty As Money)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region

		Public Property ProductAssemblyID() As Integer? 'TODO: name does not match ?

		Public Overridable Property Product() As Product

		Public Property ComponentID() As Integer 'TODO: name does not match ?

		Public Overridable Property Product1() As Product

		Public Property UnitMeasureCode() As String

		Public Overridable Property UnitMeasure() As UnitMeasure

#Region "ModifiedDate"
		Friend mappedModifiedDate As Date
		Friend myModifiedDate As TimeStamp

		<MemberOrder(99)>
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
			Return New Title($"BillOfMaterial: {BillOfMaterialID}")
		End Function
	End Class
End Namespace