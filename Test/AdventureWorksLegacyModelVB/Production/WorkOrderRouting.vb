Namespace AW.Types

	Partial Public Class WorkOrderRouting

		Implements ITitledObject, INotEditableOncePersistent

		Public Property WorkOrderID() As Integer

		Public Property ProductID() As Integer

#Region "OperationSequence"
		Public Property mappedOperationSequence As Short
		Friend myOperationSequence As WholeNumber

		<MemberOrder(1)>
		Public ReadOnly Property OperationSequence As WholeNumber
			Get
				myOperationSequence = If(myOperationSequence, New WholeNumber(mappedOperationSequence, Sub(v) mappedOperationSequence = CType(v, Short)))
				Return myOperationSequence
			End Get
		End Property

		Public Sub AboutOperationSequence(a As FieldAbout, OperationSequence As WholeNumber)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case Else
			End Select
		End Sub
#End Region

#Region "ScheduledStartDate"
		Public Property mappedScheduledStartDate As DateTime?
		Friend myScheduledStartDate As NODateNullable

		<MemberOrder(20)>
		Public ReadOnly Property ScheduledStartDate As NODateNullable
			Get
				myScheduledStartDate = If(myScheduledStartDate, New NODateNullable(mappedScheduledStartDate, Sub(v) mappedScheduledStartDate = v))
				Return myScheduledStartDate
			End Get
		End Property

		Public Sub AboutScheduledStartDate(a As FieldAbout, ScheduledStartDate As NODateNullable)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case Else
			End Select
		End Sub
#End Region

#Region "ScheduledEndDate"
		Public Property mappedScheduledEndDate As DateTime?
		Friend myScheduledEndDate As NODateNullable

		<MemberOrder(22)>
		Public ReadOnly Property ScheduledEndDate As NODateNullable
			Get
				myScheduledEndDate = If(myScheduledEndDate, New NODateNullable(mappedScheduledEndDate, Sub(v) mappedScheduledEndDate = v))
				Return myScheduledEndDate
			End Get
		End Property

		Public Sub AboutScheduledEndDate(a As FieldAbout, ScheduledEndDate As NODateNullable)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case Else
			End Select
		End Sub
#End Region

#Region "ActualStartDate"
		Public Property mappedActualStartDate As DateTime?
		Friend myActualStartDate As NODateNullable

		<MemberOrder(21)>
		Public ReadOnly Property ActualStartDate As NODateNullable
			Get
				myActualStartDate = If(myActualStartDate, New NODateNullable(mappedActualStartDate, Sub(v) mappedActualStartDate = v))
				Return myActualStartDate
			End Get
		End Property

		Public Sub AboutActualStartDate(a As FieldAbout, ActualStartDate As NODateNullable)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case Else
			End Select
		End Sub
#End Region

#Region "ActualEndDate"
		Public Property mappedActualEndDate As DateTime?
		Friend myActualEndDate As NODateNullable

		<MemberOrder(23)>
		Public ReadOnly Property ActualEndDate As NODateNullable
			Get
				myActualEndDate = If(myActualEndDate, New NODateNullable(mappedActualEndDate, Sub(v) mappedActualEndDate = v))
				Return myActualEndDate
			End Get
		End Property

		Public Sub AboutActualEndDate(a As FieldAbout, ActualEndDate As NODateNullable)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case Else
			End Select
		End Sub
#End Region

#Region "ActualResourceHrs"
		Public Property mappedActualResourceHrs As Decimal?
		Friend myActualResourceHrs As FloatingPointNumberNullable

		<MemberOrder(31)>
		Public ReadOnly Property ActualResourceHrs As FloatingPointNumberNullable
			Get
				myActualResourceHrs = If(myActualResourceHrs, New FloatingPointNumberNullable(mappedActualResourceHrs, Sub(v) mappedActualResourceHrs = v))
				Return myActualResourceHrs
			End Get
		End Property

		Public Sub AboutActualResourceHrs(a As FieldAbout, ActualResourceHrs As FloatingPointNumberNullable)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case Else
			End Select
		End Sub
#End Region

#Region "PlannedCost"
		Public Property mappedPlannedCost As Decimal
		Friend myPlannedCost As Money

		<MemberOrder(40)>
		Public ReadOnly Property PlannedCost As Money
			Get
				myPlannedCost = If(myPlannedCost, New Money(mappedPlannedCost, Sub(v) mappedPlannedCost = v))
				Return myPlannedCost
			End Get
		End Property

		Public Sub AboutPlannedCost(a As FieldAbout, PlannedCost As Money)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case Else
			End Select
		End Sub
#End Region

#Region "ActualCost"
		Public Property mappedActualCost As Decimal?
		Friend myActualCost As MoneyNullable

		<MemberOrder(41)>
		Public ReadOnly Property ActualCost As MoneyNullable
			Get
				myActualCost = If(myActualCost, New MoneyNullable(mappedActualCost, Sub(v) mappedActualCost = v))
				Return myActualCost
			End Get
		End Property

		Public Sub AboutActualCost(a As FieldAbout, ActualCost As MoneyNullable)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case Else
			End Select
		End Sub
#End Region

#Region "WorkOrder"
		Public Overridable Property WorkOrder() As WorkOrder

		Public Sub AboutWorkOrder(a As FieldAbout, w As WorkOrder)
			Select Case a.TypeCode
				Case Else
					a.Visible = False
			End Select
		End Sub
#End Region

		Public Property LocationID() As Short

		<MemberOrder(10)>
		Public Overridable Property Location() As Location

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
			Return Location.ToString()
		End Function
	End Class
End Namespace