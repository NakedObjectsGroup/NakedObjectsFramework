Namespace AW.Types

	Partial Public Class WorkOrderRouting

		Public Property WorkOrderID() As Integer

		Public Property ProductID() As Integer

#Region "OperationSequence"
		Friend mappedOperationSequence As Short
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
		Friend mappedScheduledStartDate As DateTime?
		Friend myScheduledStartDate As NODate

		<MemberOrder(20)>
		Public ReadOnly Property ScheduledStartDate As NODate
			Get
				Return If(myScheduledStartDate, New NODate(mappedScheduledStartDate, Function(v) mappedScheduledStartDate = v))
			End Get
		End Property

		Public Sub AboutScheduledStartDate(a As FieldAbout, ScheduledStartDate As NODate)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region

#Region "ScheduledEndDate"
		Friend mappedScheduledEndDate As DateTime?
		Friend myScheduledEndDate As NODate

		<MemberOrder(22)>
		Public ReadOnly Property ScheduledEndDate As NODate
			Get
				Return If(myScheduledEndDate, New NODate(mappedScheduledEndDate, Function(v) mappedScheduledEndDate = v))
			End Get
		End Property

		Public Sub AboutScheduledEndDate(a As FieldAbout, ScheduledEndDate As NODate)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region

#Region "ActualStartDate"
		Friend mappedActualStartDate As DateTime?
		Friend myActualStartDate As NODate

		<MemberOrder(21)>
		Public ReadOnly Property ActualStartDate As NODate
			Get
				Return If(myActualStartDate, New NODate(mappedActualStartDate, Function(v) mappedActualStartDate = v))
			End Get
		End Property

		Public Sub AboutActualStartDate(a As FieldAbout, ActualStartDate As NODate)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region

#Region "ActualEndDate"
		Friend mappedActualEndDate As DateTime?
		Friend myActualEndDate As NODate

		<MemberOrder(23)>
		Public ReadOnly Property ActualEndDate As NODate
			Get
				Return If(myActualEndDate, New NODate(mappedActualEndDate, Function(v) mappedActualEndDate = v))
			End Get
		End Property

		Public Sub AboutActualEndDate(a As FieldAbout, ActualEndDate As NODate)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region
		<MemberOrder(31)>
		Public Property ActualResourceHrs() As Decimal? 'TODO  need new value type

#Region "PlannedCost"
		Friend mappedPlannedCost As Decimal
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
		Friend mappedActualCost As Decimal?
		Friend myActualCost As Money

		<MemberOrder(41)>
		Public ReadOnly Property ActualCost As Money
			Get
				Return If(myActualCost, New Money(mappedActualCost, Function(v) mappedActualCost = v))
			End Get
		End Property

		Public Sub AboutActualCost(a As FieldAbout, ActualCost As Money)
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
			Return New Title(Location)
		End Function
	End Class
End Namespace