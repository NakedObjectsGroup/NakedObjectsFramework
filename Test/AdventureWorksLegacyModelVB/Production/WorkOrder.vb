Namespace AW.Types

	Partial Public Class WorkOrder


		Public Property WorkOrderID() As Integer

#Region "StockedQty"
		Friend mappedStockedQty As Integer
		Friend myStockedQty As WholeNumber

		'<MemberOrder(22)>
		Public ReadOnly Property StockedQty As WholeNumber
			Get
				Return If(myStockedQty, New WholeNumber(mappedStockedQty, Function(v) mappedStockedQty = v))
			End Get
		End Property

		Public Sub AboutStockedQty(a As FieldAbout, StockedQty As WholeNumber)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region

#Region "ScrappedQty"
		Friend mappedScrappedQty As Short
		Friend myScrappedQty As WholeNumber

		'<MemberOrder(24)>
		Public ReadOnly Property ScrappedQty As WholeNumber
			Get
				Return If(myScrappedQty, New WholeNumber(mappedScrappedQty, Function(v) mappedScrappedQty = v))
			End Get
		End Property

		Public Sub AboutScrappedQty(a As FieldAbout, ScrappedQty As WholeNumber)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region

#Region "EndDate"
		Friend mappedEndDate As Date?
		Friend myEndDate As NODate

		'<MemberOrder(32)>
		Public ReadOnly Property EndDate As NODate
			Get
				Return If(myEndDate, New NODate(mappedEndDate, Function(v) mappedEndDate = v))
			End Get
		End Property

		Public Sub AboutEndDate(a As FieldAbout, EndDate As NODate)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region

		Public Property ScrapReasonID() As Short?

		'<MemberOrder(26)>
		Public Overridable Property ScrapReason() As ScrapReason

#Region "OrderQty"
		Friend mappedOrderQty As Integer
		Friend myOrderQty As WholeNumber

		'<MemberOrder(20)>
		Public ReadOnly Property OrderQty As WholeNumber
			Get
				Return If(myOrderQty, New WholeNumber(mappedOrderQty, Function(v) mappedOrderQty = v))
			End Get
		End Property

		Public Sub AboutOrderQty(a As FieldAbout, OrderQty As WholeNumber)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region

#Region "StartDate"
		Friend mappedStartDate As Date
		Friend myStartDate As NODate

		'<MemberOrder(30)>
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

#Region "DueDate"
		Friend mappedDueDate As Date
		Friend myDueDate As NODate

		'<MemberOrder(34)>
		Public ReadOnly Property DueDate As NODate
			Get
				Return If(myDueDate, New NODate(mappedDueDate, Function(v) mappedDueDate = v))
			End Get
		End Property

		Public Sub AboutDueDate(a As FieldAbout, DueDate As NODate)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region

		Public Property ProductID() As Integer

		'<MemberOrder(10)>
		Public Overridable Property Product() As Product

#Region "WorkOrderRoutings (Collection)"
		Public Overridable Property mappedWorkOrderRoutings As ICollection(Of WorkOrderRouting) = New List(Of WorkOrderRouting)()

		Private myWorkOrderRoutings As InternalCollection

		''<TableView(True, "OperationSequence", "ScheduledStartDate", "ScheduledEndDate", "Location", "PlannedCost")>
		Public ReadOnly Property WorkOrderRoutings As InternalCollection
			Get
				Return If(myWorkOrderRoutings, New InternalCollection(Of WorkOrderRouting)(mappedWorkOrderRoutings))
			End Get
		End Property

		Public Sub AboutWorkOrderRoutings(a As FieldAbout)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region
		'<Hidden>
		Public ReadOnly Property AnAlwaysHiddenReadOnlyProperty() As String
			Get
				Return ""
			End Get
		End Property

#Region "ModifiedDate"
		Friend mappedModifiedDate As Date
		Friend myModifiedDate As TimeStamp

		'<MemberOrder(99)>
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
			Return New Title($"{Product}: {StartDate}")
		End Function
	End Class
End Namespace