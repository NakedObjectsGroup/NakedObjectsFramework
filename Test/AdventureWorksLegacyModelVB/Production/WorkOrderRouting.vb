Namespace AW.Types

	Partial Public Class WorkOrderRouting

		Implements ITitledObject

		Public Property WorkOrderID() As Integer

		Public Property ProductID() As Integer

#Region "OperationSequence"
		Public Property mappedOperationSequence As Short
		Friend myOperationSequence As WholeNumber

		<MemberOrder(1)>
		Public ReadOnly Property OperationSequence As WholeNumber
			Get
				Return If(myOperationSequence, New WholeNumber(mappedOperationSequence, Function(v) mappedOperationSequence = v))
			End Get
		End Property

		Public Sub AboutOperationSequence(a As FieldAbout, OperationSequence As WholeNumber)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region

#Region "ScheduledStartDate"
		Public mappedScheduledStartDate As DateTime?
		Friend myScheduledStartDate As NODateNullable

		<MemberOrder(20)>
		Public ReadOnly Property ScheduledStartDate As NODateNullable
			Get
				Return If(myScheduledStartDate, New NODateNullable(mappedScheduledStartDate, Function(v) mappedScheduledStartDate = v))
			End Get
		End Property

		Public Sub AboutScheduledStartDate(a As FieldAbout, ScheduledStartDate As NODateNullable)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region

#Region "ScheduledEndDate"
		Public mappedScheduledEndDate As DateTime?
		Friend myScheduledEndDate As NODateNullable

		<MemberOrder(22)>
		Public ReadOnly Property ScheduledEndDate As NODateNullable
			Get
				Return If(myScheduledEndDate, New NODateNullable(mappedScheduledEndDate, Function(v) mappedScheduledEndDate = v))
			End Get
		End Property

		Public Sub AboutScheduledEndDate(a As FieldAbout, ScheduledEndDate As NODateNullable)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region

#Region "ActualStartDate"
		Public mappedActualStartDate As DateTime?
		Friend myActualStartDate As NODateNullable

		<MemberOrder(21)>
		Public ReadOnly Property ActualStartDate As NODateNullable
			Get
				Return If(myActualStartDate, New NODateNullable(mappedActualStartDate, Function(v) mappedActualStartDate = v))
			End Get
		End Property

		Public Sub AboutActualStartDate(a As FieldAbout, ActualStartDate As NODateNullable)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region

#Region "ActualEndDate"
		Public mappedActualEndDate As DateTime?
		Friend myActualEndDate As NODateNullable

		<MemberOrder(23)>
		Public ReadOnly Property ActualEndDate As NODateNullable
			Get
				Return If(myActualEndDate, New NODateNullable(mappedActualEndDate, Function(v) mappedActualEndDate = v))
			End Get
		End Property

		Public Sub AboutActualEndDate(a As FieldAbout, ActualEndDate As NODateNullable)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region

#Region "ActualResourceHrs"
		Public mappedActualResourceHrs As Decimal?
		Friend myActualResourceHrs As FloatingPointNumberNullable

		<MemberOrder(31)>
		Public ReadOnly Property ActualResourceHrs As FloatingPointNumberNullable
			Get
				Return If(myActualResourceHrs, New FloatingPointNumberNullable(mappedActualResourceHrs, Function(v) mappedActualResourceHrs = v))
			End Get
		End Property

		Public Sub AboutActualResourceHrs(a As FieldAbout, ActualResourceHrs As FloatingPointNumberNullable)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region

#Region "PlannedCost"
		Public mappedPlannedCost As Decimal
		Friend myPlannedCost As Money

		<MemberOrder(40)>
		Public ReadOnly Property PlannedCost As Money
			Get
				Return If(myPlannedCost, New Money(mappedPlannedCost, Function(v) mappedPlannedCost = v))
			End Get
		End Property

		Public Sub AboutPlannedCost(a As FieldAbout, PlannedCost As Money)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region

#Region "ActualCost"
		Public mappedActualCost As Decimal?
		Friend myActualCost As MoneyNullable

		<MemberOrder(41)>
		Public ReadOnly Property ActualCost As MoneyNullable
			Get
				Return If(myActualCost, New MoneyNullable(mappedActualCost, Function(v) mappedActualCost = v))
			End Get
		End Property

		Public Sub AboutActualCost(a As FieldAbout, ActualCost As MoneyNullable)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region

#Region "WorkOrder"
		Public Overridable Property WorkOrder() As WorkOrder

		Public Sub AboutWorkOrder(a As FieldAbout, w As WorkOrder)
			Select Case a.TypeCode
				Case AboutTypeCodes.Visible
					a.Visible = False
			End Select
		End Sub
#End Region

		Public Property LocationID() As Short

		<MemberOrder(10)>
		Public Overridable Property Location() As Location

#Region "ModifiedDate"
		Public mappedModifiedDate As Date
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

		Public Function Title() As ITitle Implements ITitledObject.Title
			Return New Title(ToString())
		End Function

		Public Overrides Function ToString() As String
			Return Location.ToString()
		End Function
	End Class
End Namespace