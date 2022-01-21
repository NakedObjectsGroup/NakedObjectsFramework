Namespace AW.Types

	Partial Public Class BillOfMaterial

		Implements ITitledObject

		Public Property BillOfMaterialID() As Integer

#Region "StartDate"
		Public Property mappedStartDate As Date
		Friend myStartDate As NODate

		<MemberOrder(1)>
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

		<MemberOrder(1)>
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

#Region "BOMLevel"
		Public Property mappedBOMLevel As Short
		Friend myBOMLevel As WholeNumber

		<MemberOrder(1)>
		Public ReadOnly Property BOMLevel As WholeNumber
			Get
				myBOMLevel = If(myBOMLevel, New WholeNumber(mappedBOMLevel, Sub(v) mappedBOMLevel = CType(v, Short)))
				Return myBOMLevel
			End Get
		End Property

		Public Sub AboutBOMLevel(a As FieldAbout, BOMLevel As WholeNumber)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case Else
			End Select
		End Sub
#End Region

#Region "PerAssemblyQty"
		Public Property mappedPerAssemblyQty As Decimal
		Friend myPerAssemblyQty As FloatingPointNumber

		<MemberOrder(1)>
		Public ReadOnly Property PerAssemblyQty As FloatingPointNumber
			Get
				myPerAssemblyQty = If(myPerAssemblyQty, New FloatingPointNumber(mappedPerAssemblyQty, Sub(v) mappedPerAssemblyQty = v))
				Return myPerAssemblyQty
			End Get
		End Property

		Public Sub AboutPerAssemblyQty(a As FieldAbout, PerAssemblyQty As FloatingPointNumber)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case Else
			End Select
		End Sub
#End Region

		Public Property ProductID() As Integer? 'TODO: name does not match ?

		Public Overridable Property Product() As Product

		Public Property Product1ID() As Integer 'TODO: name does not match ?

		Public Overridable Property Product1() As Product

		Public Property UnitMeasureCode() As String

		Public Overridable Property UnitMeasure() As UnitMeasure

#Region "ModifiedDate"
		Public Property mappedModifiedDate As Date
		Friend myModifiedDate As TimeStamp

		<MemberOrder(99)>
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
			Return $"BillOfMaterial: {BillOfMaterialID}"
		End Function
	End Class
End Namespace