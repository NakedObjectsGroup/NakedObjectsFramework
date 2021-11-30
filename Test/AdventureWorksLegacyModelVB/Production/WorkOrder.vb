Namespace AW.Types

	Partial Public Class WorkOrder
		Implements IHasModifiedDate

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

#Region "ModifiedDate"
		Friend mappedModifiedDate As Date
		Friend myModifiedDate As TimeStamp

		<MemberOrder(1)>
		Public ReadOnly Property ModifiedDate As TimeStamp Implements IHasModifiedDate.ModifiedDate
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

		Public Overrides Function ToString() As String
			Return $"{Product}: {StartDate}"
		End Function
	End Class
End Namespace