Namespace AW.Types

	Partial Public Class WorkOrderRouting
		Public Sub New()
		End Sub

		Public Sub New(ByVal cloneFrom As WorkOrderRouting)
			WorkOrderID = cloneFrom.WorkOrderID
			ProductID = cloneFrom.ProductID
			OperationSequence = cloneFrom.OperationSequence
			ScheduledStartDate = cloneFrom.ScheduledStartDate
			ScheduledEndDate = cloneFrom.ScheduledEndDate
			ActualStartDate = cloneFrom.ActualStartDate
			ActualEndDate = cloneFrom.ActualEndDate
			ActualResourceHrs = cloneFrom.ActualResourceHrs
			PlannedCost = cloneFrom.PlannedCost
			ActualCost = cloneFrom.ActualCost
			WorkOrder = cloneFrom.WorkOrder
			LocationID = cloneFrom.LocationID
			Location = cloneFrom.Location
			ModifiedDate = cloneFrom.ModifiedDate
		End Sub

		<Hidden>
		Public Property WorkOrderID() As Integer

		<Hidden>
		Public Property ProductID() As Integer

		<MemberOrder(1)>
		Public Property OperationSequence() As Short

		<MemberOrder(20)>
		Public Property ScheduledStartDate() As DateTime?

		<MemberOrder(22)>
		Public Property ScheduledEndDate() As DateTime?

		<MemberOrder(21)>
		<Mask("d")>
		Public Property ActualStartDate() As DateTime?

		<MemberOrder(23)>
		<Mask("d")>
		Public Property ActualEndDate() As DateTime?

		<MemberOrder(31)>
		Public Property ActualResourceHrs() As Decimal?

		<MemberOrder(40), Mask("C")>
		Public Property PlannedCost() As Decimal

		<MemberOrder(41), Mask("C")>
		Public Property ActualCost() As Decimal?

		<Hidden>
		Public Overridable Property WorkOrder() As WorkOrder

		<Hidden>
		Public Property LocationID() As Short

		<MemberOrder(10)>
		Public Overridable Property Location() As Location

		<MemberOrder(99)>
		Public Property ModifiedDate() As DateTime

		Public Overrides Function ToString() As String
			Return $"{Location}"
		End Function
	End Class
End Namespace