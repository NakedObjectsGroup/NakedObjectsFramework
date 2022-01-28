Namespace AW.Types

	Partial Public Class WorkOrder
		Implements ITitledObject 'Does not implement INotEditableOncePersistent - to permit testing

		Public Property WorkOrderID() As Integer

#Region "StockedQty"
		Public Property mappedStockedQty As Integer
		Friend myStockedQty As WholeNumber

		<DemoProperty(Order:=22)>
		Public ReadOnly Property StockedQty As WholeNumber
			Get
				myStockedQty = If(myStockedQty, New WholeNumber(mappedStockedQty, Sub(v) mappedStockedQty = v))
				Return myStockedQty
			End Get
		End Property

		Public Sub AboutStockedQty(a As FieldAbout, StockedQty As WholeNumber)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case Else
			End Select
		End Sub
#End Region

#Region "ScrappedQty"
		Public Property mappedScrappedQty As Short
		Friend myScrappedQty As WholeNumber

		<DemoProperty(Order:=24)>
		Public ReadOnly Property ScrappedQty As WholeNumber
			Get
				myScrappedQty = If(myScrappedQty, New WholeNumber(mappedScrappedQty, Sub(v) mappedScrappedQty = CType(v, Short)))
				Return myScrappedQty
			End Get
		End Property

		Public Sub AboutScrappedQty(a As FieldAbout, ScrappedQty As WholeNumber)
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

		<DemoProperty(Order:=32)>
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

		Public Property ScrapReasonID() As Short?

		<DemoProperty(Order:=26)>
		Public Overridable Property ScrapReason() As ScrapReason

#Region "OrderQty"
		Public Property mappedOrderQty As Integer
		Friend myOrderQty As WholeNumber

		<DemoProperty(Order:=20)>
		Public ReadOnly Property OrderQty As WholeNumber
			Get
				myOrderQty = If(myOrderQty, New WholeNumber(mappedOrderQty, Sub(v) mappedOrderQty = v))
				Return myOrderQty
			End Get
		End Property

		Public Sub AboutOrderQty(a As FieldAbout, OrderQty As WholeNumber)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case Else
			End Select
		End Sub
#End Region

#Region "StartDate"
		Public Property mappedStartDate As Date
		Friend myStartDate As NODate

		<DemoProperty(Order:=30)>
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

#Region "DueDate"
		Public Property mappedDueDate As Date
		Friend myDueDate As NODate

		<DemoProperty(Order:=34)>
		Public ReadOnly Property DueDate As NODate
			Get
				myDueDate = If(myDueDate, New NODate(mappedDueDate, Sub(v) mappedDueDate = v))
				Return myDueDate
			End Get
		End Property

		Public Sub AboutDueDate(a As FieldAbout, DueDate As NODate)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case Else
			End Select
		End Sub
#End Region

		Public Property ProductID() As Integer

		<DemoProperty(Order:=10)>
		Public Overridable Property Product() As Product

#Region "WorkOrderRoutings (Collection)"
		Public Overridable Property mappedWorkOrderRoutings As ICollection(Of WorkOrderRouting) = New List(Of WorkOrderRouting)()

		Private myWorkOrderRoutings As InternalCollection

		''<TableView(True, "OperationSequence", "ScheduledStartDate", "ScheduledEndDate", "Location", "PlannedCost")>
		Public ReadOnly Property WorkOrderRoutings As InternalCollection
			Get
				myWorkOrderRoutings = If(myWorkOrderRoutings, New InternalCollection(Of WorkOrderRouting)(mappedWorkOrderRoutings))
				Return myWorkOrderRoutings
			End Get
		End Property

		Public Sub AboutWorkOrderRoutings(a As FieldAbout)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case Else
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

		Public Function Title() As Title Implements ITitledObject.Title
			Return New Title(ToString())
		End Function

		Public Overrides Function ToString() As String
			Return $"{Product}: {StartDate}"
		End Function
	End Class
End Namespace