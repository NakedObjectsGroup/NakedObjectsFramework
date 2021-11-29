Namespace AW.Types

	Partial Public Class WorkOrder
		Implements IHasModifiedDate

		Public Sub New()
		End Sub

		Public Sub New(ByVal cloneFrom As WorkOrder)
			WorkOrderID = cloneFrom.WorkOrderID
			StockedQty = cloneFrom.StockedQty
			ScrappedQty = cloneFrom.ScrappedQty
			EndDate = cloneFrom.EndDate
			ScrapReasonID = cloneFrom.ScrapReasonID
			ScrapReason = cloneFrom.ScrapReason
			OrderQty = cloneFrom.OrderQty
			StartDate = cloneFrom.StartDate
			DueDate = cloneFrom.DueDate
			ProductID = cloneFrom.ProductID
			Product = cloneFrom.Product
			WorkOrderRoutings = cloneFrom.WorkOrderRoutings
			ModifiedDate = cloneFrom.ModifiedDate
		End Sub

		<Hidden>
		Public Property WorkOrderID() As Integer

		<MemberOrder(22)>
		Public Property StockedQty() As Integer

		<MemberOrder(24)>
		Public Property ScrappedQty() As Short

		<MemberOrder(32), Mask("d")>
		Public Property EndDate() As DateTime?

		<Hidden>
		Public Property ScrapReasonID() As Short?

		'INSTANT VB WARNING: Nullable reference types have no equivalent in VB:
		'ORIGINAL LINE: public virtual ScrapReason? ScrapReason {get;set;}
		<MemberOrder(26)>
		Public Overridable Property ScrapReason() As ScrapReason

		<MemberOrder(20)>
		Public Property OrderQty() As Integer

		<MemberOrder(30), Mask("d")>
		Public Property StartDate() As DateTime

		<MemberOrder(34), Mask("d")>
		Public Property DueDate() As DateTime

		<Hidden>
		Public Property ProductID() As Integer

		<MemberOrder(10)>
		Public Overridable Property Product() As Product

		<TableView(True, "OperationSequence", "ScheduledStartDate", "ScheduledEndDate", "Location", "PlannedCost")>
		Public Overridable Property WorkOrderRoutings() As ICollection(Of WorkOrderRouting) = New List(Of WorkOrderRouting)()

		<Hidden>
		Public ReadOnly Property AnAlwaysHiddenReadOnlyProperty() As String
			Get
				Return ""
			End Get
		End Property

		<MemberOrder(99)>
		Public Property ModifiedDate() As DateTime Implements IHasModifiedDate.ModifiedDate

		Public Overrides Function ToString() As String
			Return $"{Product}: {StartDate}"
		End Function
	End Class
End Namespace