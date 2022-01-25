Namespace AW.Types

	Partial Public Class SpecialOffer
		Implements ITitledObject, IContainerAware

#Region "Container"
		Public Property Container As IContainer Implements IContainerAware.Container
#End Region
		''<Hidden>
		Public Property SpecialOfferID() As Integer

#Region "Description"
		Public Property mappedDescription As String
		Friend myDescription As TextString

		<MemberOrder(10)>
		Public ReadOnly Property Description As TextString
			Get
				myDescription = If(myDescription, New TextString(mappedDescription, Sub(v) mappedDescription = v))
				Return myDescription
			End Get
		End Property

		Public Sub AboutDescription(a As FieldAbout, Description As TextString)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
					If Description.IsEmpty() Then
						a.IsValid = False
						a.InvalidReason = "Cannot be empty"
					End If
				Case Else
			End Select
		End Sub
#End Region

#Region "DiscountPct"
		Public Property mappedDiscountPct As Decimal
		Friend myDiscountPct As Percentage

		<MemberOrder(20)>
		Public ReadOnly Property DiscountPct As Percentage
			Get
				myDiscountPct = If(myDiscountPct, New Percentage(mappedDiscountPct, Sub(v) mappedDiscountPct = v))
				Return myDiscountPct
			End Get
		End Property

		Public Sub AboutDiscountPct(a As FieldAbout, DiscountPct As Percentage)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
					If DiscountPct.IsEmpty OrElse DiscountPct.Value > 100 Then
						a.IsValid = False
						a.InvalidReason = "Must be in range 0 - 100"
					End If
				Case Else
			End Select
		End Sub
#End Region

#Region "Type"
		Public Property mappedType As String
		Friend myType As TextString

		<MemberOrder(30)>
		Public ReadOnly Property Type As TextString
			Get
				myType = If(myType, New TextString(mappedType, Sub(v) mappedType = v))
				Return myType
			End Get
		End Property

		Public Sub AboutType(a As FieldAbout, Type As TextString)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
					If Type.IsEmpty() Then
						a.IsValid = False
						a.InvalidReason = "Type cannot be empty"
					End If
				Case Else
			End Select
		End Sub
#End Region

#Region "Category"
		Public Property mappedCategory As String
		Friend myCategory As TextString

		<MemberOrder(40)>
		Public ReadOnly Property Category As TextString
			Get
				myCategory = If(myCategory, New TextString(mappedCategory, Sub(v) mappedCategory = v))
				Return myCategory
			End Get
		End Property

		Public Sub AboutCategory(a As FieldAbout, Category As TextString)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Parameters
					a.Options = New Object() {New TextString("Clearance"), New TextString("Promotion")}
				Case AboutTypeCodes.Valid
					If Category.IsEmpty() Then
						a.IsValid = False
						a.InvalidReason = "Cannot be empty"
					End If
				Case Else
			End Select
		End Sub
#End Region

#Region "StartDate"
		Public Property mappedStartDate As Date
		Friend myStartDate As NODate

		<MemberOrder(51)>
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
					If StartDate.IsEmpty() OrElse StartDate.Value < Today Then
						a.IsValid = False
						a.InvalidReason = "Must be today or later"
					End If
				Case Else
			End Select
		End Sub
#End Region

#Region "EndDate"
		Public Property mappedEndDate As Date
		Friend myEndDate As NODate

		<MemberOrder(52)>
		Public ReadOnly Property EndDate As NODate
			Get
				myEndDate = If(myEndDate, New NODate(mappedEndDate, Sub(v) mappedEndDate = v))
				Return myEndDate
			End Get
		End Property

		Public Sub AboutEndDate(a As FieldAbout, EndDate As NODate)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
					If EndDate.IsEmpty OrElse StartDate.Value < Today Then
						a.IsValid = False
						a.InvalidReason = "Must be after today"
					End If
				Case Else
			End Select
		End Sub
#End Region

#Region "MinQty"
		Public Property mappedMinQty As Integer
		Friend myMinQty As WholeNumber

		<MemberOrder(61)>
		Public ReadOnly Property MinQty As WholeNumber
			Get
				myMinQty = If(myMinQty, New WholeNumber(mappedMinQty, Sub(v) mappedMinQty = v))
				Return myMinQty
			End Get
		End Property

		Public Sub AboutMinQty(a As FieldAbout, MinQty As WholeNumber)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
					If MinQty.IsEmpty OrElse MinQty.Value < 1 Then
						a.IsValid = False
						a.InvalidReason = "Must be at least 1"
					End If
				Case Else
			End Select
		End Sub
#End Region

#Region "MaxQty"
		Public Property mappedMaxQty As Integer?
		Friend myMaxQty As WholeNumberNullable

		<MemberOrder(62)>
		Public ReadOnly Property MaxQty As WholeNumberNullable
			Get
				myMaxQty = If(myMaxQty, New WholeNumberNullable(mappedMaxQty, Sub(v) mappedMaxQty = v))
				Return myMaxQty
			End Get
		End Property

		Public Sub AboutMaxQty(a As FieldAbout, MaxQty As WholeNumberNullable)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
					If Not MaxQty.IsEmpty AndAlso MaxQty.Value < 1 Then
						a.IsValid = False
						a.InvalidReason = "Cannot be < 1"
					End If
				Case Else
			End Select
		End Sub
#End Region

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

		''<Hidden>
		Public Property RowGuid() As Guid

		Public Function Title() As Title Implements ITitledObject.Title
			Return New Title(ToString())
		End Function

		Public Overrides Function ToString() As String
			Return mappedDescription
		End Function

#Region "Actions"
		Public Sub ActionSave()
			mappedModifiedDate = Now
			RowGuid = Guid.NewGuid()
			Container.MakePersistent(Me)
		End Sub

		Public Sub AboutActionSave(a As ActionAbout)
			Select Case a.TypeCode
				Case AboutTypeCodes.Valid
					If Not MaxQty.Value Is Nothing AndAlso MaxQty.Value < MinQty.Value Then
						a.Usable = False
						a.UnusableReason = "Max Qty cannot be less than Min Qty"
					End If
				Case Else
			End Select
		End Sub

		Public Function ActionProductsCovered() As IQueryable(Of Product)
			Return From sop In Container.AllInstances(Of SpecialOfferProduct)
				   Where sop.SpecialOfferID = SpecialOfferID
				   Order By sop.mappedModifiedDate Descending
				   Select sop.Product
		End Function

		Public Function ActionIncludeProduct(product As Product) As SpecialOfferProduct
			Dim pid = product.ProductID
			Dim sop As SpecialOfferProduct = CType(Container.CreateTransientInstance(GetType(SpecialOfferProduct)), SpecialOfferProduct)
			sop.SpecialOfferID = Me.SpecialOfferID
			sop.ProductID = pid
			sop.mappedModifiedDate = DateTime.Now
			sop.RowGuid = Guid.NewGuid()
			Container.MakePersistent(sop)
			Return sop
		End Function

#End Region
	End Class
End Namespace