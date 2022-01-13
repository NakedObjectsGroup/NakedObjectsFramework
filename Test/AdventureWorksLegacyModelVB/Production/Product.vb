Imports AW.Services
Imports NakedFramework.Value

Namespace AW.Types

    Partial Public Class Product
        Implements ITitledObject, IContainerAware


#Region "Container"

        Public Property Container As IContainer Implements IContainerAware.Container
#End Region

#Region "Visible properties"
#Region "Name"
        Public mappedName As String
        Friend myName As TextString

        <MemberOrder(1)>
        Public ReadOnly Property Name As TextString
            Get
                Return If(myName, New TextString(mappedName, Function(v) mappedName = v))
            End Get
        End Property

        Public Sub AboutName(a As FieldAbout, Name As TextString)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case AboutTypeCodes.Visible
            End Select
        End Sub
#End Region

#Region "ProductNumber"
        Public mappedProductNumber As String
        Friend myProductNumber As TextString

        <MemberOrder(2)>
        Public ReadOnly Property ProductNumber As TextString
            Get
                Return If(myProductNumber, New TextString(mappedProductNumber, Function(v) mappedProductNumber = v))
            End Get
        End Property

        Public Sub AboutProductNumber(a As FieldAbout, ProductNumber As TextString)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case AboutTypeCodes.Visible
            End Select
        End Sub
#End Region

#Region "Color"
        Public mappedColor As String
        Friend myColor As TextString

        <MemberOrder(3)>
        Public ReadOnly Property Color As TextString
            Get
                Return If(myColor, New TextString(mappedColor, Function(v) mappedColor = v))
            End Get
        End Property

        Public Sub AboutColor(a As FieldAbout, Color As TextString)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case AboutTypeCodes.Visible
            End Select
        End Sub
#End Region

        <MemberOrder(4)>
        Public Overridable ReadOnly Property Photo() As Image
            Get
                Return Nothing 'Product_Functions.Photo(Me)
            End Get
        End Property

        <MemberOrder(10)>
        Public Overridable Property ProductModel() As ProductModel

#Region "ListPrice"
        Public mappedListPrice As Decimal
        Friend myListPrice As Money

        <MemberOrder(12)>
        Public ReadOnly Property ListPrice As Money
            Get
                Return If(myListPrice, New Money(mappedListPrice, Function(v) mappedListPrice = v))
            End Get
        End Property

        Public Sub AboutListPrice(a As FieldAbout, ListPrice As Money)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case AboutTypeCodes.Visible
            End Select
        End Sub
#End Region

        <MemberOrder(13)>
        Public Overridable ReadOnly Property ProductCategory() As ProductCategory
            Get
                Return Nothing 'TODO: ProductSubcategory.ProductCategory
            End Get
        End Property

        <MemberOrder(14)>
        Public Overridable Property ProductSubcategory() As ProductSubcategory

#Region "ProductLine"
        Public mappedProductLine As String
        Friend myProductLine As TextString

        <MemberOrder(15)>
        Public ReadOnly Property ProductLine As TextString
            Get
                Return If(myProductLine, New TextString(mappedProductLine, Function(v) mappedProductLine = v))
            End Get
        End Property

        Public Sub AboutProductLine(a As FieldAbout, ProductLine As TextString)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case AboutTypeCodes.Visible
            End Select
        End Sub
#End Region

#Region "SizeWithUnit"
        Public mappedSizeWithUnit As String
        Friend mySizeWithUnit As TextString

        <MemberOrder(16)>
        Public ReadOnly Property SizeWithUnit As TextString
            Get
                Return If(mySizeWithUnit, New TextString(mappedSizeWithUnit, Function(v) mappedSizeWithUnit = v))
            End Get
        End Property

        Public Sub AboutSizeWithUnit(a As FieldAbout, SizeWithUnit As TextString)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                    a.Name = "Size"
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case AboutTypeCodes.Visible
            End Select
        End Sub
#End Region

#Region "WeightWithUnit"
        Public mappedWeightWithUnit As String
        Friend myWeightWithUnit As TextString

        <MemberOrder(17)>
        Public ReadOnly Property WeightWithUnit As TextString
            Get
                Return If(myWeightWithUnit, New TextString(mappedWeightWithUnit, Function(v) mappedWeightWithUnit = v))
            End Get
        End Property

        Public Sub AboutWeightWithUnit(a As FieldAbout, WeightWithUnit As TextString)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                    a.Name = "Weight"
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case AboutTypeCodes.Visible
            End Select
        End Sub
#End Region

#Region "Style"
        Public mappedStyle As String
        Friend myStyle As TextString

        <MemberOrder(18)>
        Public ReadOnly Property Style As TextString
            Get
                Return If(myStyle, New TextString(mappedStyle, Function(v) mappedStyle = v))
            End Get
        End Property

        Public Sub AboutStyle(a As FieldAbout, Style As TextString)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case AboutTypeCodes.Visible
            End Select
        End Sub
#End Region

#Region "Class"
        Public mappedClass As String
        Friend My_Class As TextString

        <MemberOrder(19)>
        Public ReadOnly Property [Class] As TextString
            Get
                Return If(My_Class, New TextString(mappedClass, Function(v) mappedClass = v))
            End Get
        End Property

        Public Sub AboutClass(a As FieldAbout, [Class] As TextString)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case AboutTypeCodes.Visible
            End Select
        End Sub
#End Region

#Region "Make"
        Public mappedMake As Boolean
        Friend myMake As Logical

        <MemberOrder(20)>
        Public ReadOnly Property Make As Logical
            Get
                Return If(myMake, New Logical(mappedMake, Function(v) mappedMake = v))
            End Get
        End Property

        Public Sub AboutMake(a As FieldAbout, Make As Logical)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case AboutTypeCodes.Visible
            End Select
        End Sub
#End Region

#Region "FinishedGoods"
        Public mappedFinishedGoods As Boolean
        Friend myFinishedGoods As Logical

        <MemberOrder(21)>
        Public ReadOnly Property FinishedGoods As Logical
            Get
                Return If(myFinishedGoods, New Logical(mappedFinishedGoods, Function(v) mappedFinishedGoods = v))
            End Get
        End Property

        Public Sub AboutFinishedGoods(a As FieldAbout, FinishedGoods As Logical)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case AboutTypeCodes.Visible
            End Select
        End Sub
#End Region

#Region "SafetyStockLevel"
        Public mappedSafetyStockLevel As Short
        Friend mySafetyStockLevel As WholeNumber

        <MemberOrder(22)>
        Public ReadOnly Property SafetyStockLevel As WholeNumber
            Get
                Return If(mySafetyStockLevel, New WholeNumber(mappedSafetyStockLevel, Function(v) mappedSafetyStockLevel = v))
            End Get
        End Property

        Public Sub AboutSafetyStockLevel(a As FieldAbout, SafetyStockLevel As WholeNumber)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case AboutTypeCodes.Visible
            End Select
        End Sub
#End Region

#Region "ReorderPoint"
        Public mappedReorderPoint As Short
        Friend myReorderPoint As WholeNumber

        <MemberOrder(23)>
        Public ReadOnly Property ReorderPoint As WholeNumber
            Get
                Return If(myReorderPoint, New WholeNumber(mappedReorderPoint, Function(v) mappedReorderPoint = v))
            End Get
        End Property

        Public Sub AboutReorderPoint(a As FieldAbout, ReorderPoint As WholeNumber)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case AboutTypeCodes.Visible
            End Select
        End Sub
#End Region

#Region "DaysToManufacture"
        Public mappedDaysToManufacture As Integer
        Friend myDaysToManufacture As WholeNumber

        <MemberOrder(24)>
        Public ReadOnly Property DaysToManufacture As WholeNumber
            Get
                Return If(myDaysToManufacture, New WholeNumber(mappedDaysToManufacture, Function(v) mappedDaysToManufacture = v))
            End Get
        End Property

        Public Sub AboutDaysToManufacture(a As FieldAbout, DaysToManufacture As WholeNumber)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case AboutTypeCodes.Visible
            End Select
        End Sub
#End Region

#Region "SellStartDate"
        Public mappedSellStartDate As Date
        Friend mySellStartDate As NODate

        <MemberOrder(81)>
        Public ReadOnly Property SellStartDate As NODate
            Get
                Return If(mySellStartDate, New NODate(mappedSellStartDate, Function(v) mappedSellStartDate = v))
            End Get
        End Property

        Public Sub AboutSellStartDate(a As FieldAbout, SellStartDate As NODate)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case AboutTypeCodes.Visible
            End Select
        End Sub
#End Region

#Region "SellEndDate"
        Public mappedSellEndDate As Date?
        Friend mySellEndDate As NODateNullable

        <MemberOrder(1)>
        Public ReadOnly Property SellEndDate As NODateNullable
            Get
                Return If(mySellEndDate, New NODateNullable(mappedSellEndDate, Function(v) mappedSellEndDate = v))
            End Get
        End Property

        Public Sub AboutSellEndDate(a As FieldAbout, SellEndDate As NODateNullable)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case AboutTypeCodes.Visible
            End Select
        End Sub
#End Region

#Region "PropName"
        Public mappedPropName As Date
        Friend myPropName As NODate

        <MemberOrder(82)>
        Public ReadOnly Property PropName As NODate
            Get
                Return If(myPropName, New NODate(mappedPropName, Function(v) mappedPropName = v))
            End Get
        End Property

        Public Sub AboutPropName(a As FieldAbout, PropName As NODate)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case AboutTypeCodes.Visible
            End Select
        End Sub
#End Region

#Region "DiscontinuedDate"
        Public mappedDiscontinuedDate As Date?
        Friend myDiscontinuedDate As NODateNullable

        <MemberOrder(1)>
        Public ReadOnly Property DiscontinuedDate As NODateNullable
            Get
                Return If(myDiscontinuedDate, New NODateNullable(mappedDiscontinuedDate, Function(v) mappedDiscontinuedDate = v))
            End Get
        End Property

        Public Sub AboutDiscontinuedDate(a As FieldAbout, DiscontinuedDate As NODateNullable)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case AboutTypeCodes.Visible
            End Select
        End Sub
#End Region

#Region "StandardCost"
        Public mappedStandardCost As Decimal
        Friend myStandardCost As Money

        <MemberOrder(90)>
        Public ReadOnly Property StandardCost As Money
            Get
                Return If(myStandardCost, New Money(mappedStandardCost, Function(v) mappedStandardCost = v))
            End Get
        End Property

        Public Sub AboutStandardCost(a As FieldAbout, StandardCost As Money)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case AboutTypeCodes.Visible
            End Select
        End Sub
#End Region

#Region "ModifiedDate"
        Public mappedModifiedDate As Date
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

#End Region

#Region "Visible Collections"

#Region "ProductReviews (Collection)"
        Public Overridable Property mappedProductReviews As ICollection(Of ProductReview) = New List(Of ProductReview)()

        Private myProductReviews As InternalCollection

        <MemberOrder(100)>
        Public ReadOnly Property ProductReviews As InternalCollection
            Get
                Return If(myProductReviews, New InternalCollection(Of ProductReview)(mappedProductReviews))
            End Get
        End Property

        Public Sub AboutProductReviews(a As FieldAbout)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Visible
            End Select
        End Sub
#End Region

#Region "ProductInventory (Collection)"
        Public Overridable Property mappedProductInventory As ICollection(Of ProductInventory) = New List(Of ProductInventory)()

        Private myProductInventory As InternalCollection

        <MemberOrder(120)>
        Public ReadOnly Property ProductInventory As InternalCollection
            Get
                Return If(myProductInventory, New InternalCollection(Of ProductInventory)(mappedProductInventory))
            End Get
        End Property

        Public Sub AboutProductInventory(a As FieldAbout)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Visible
            End Select
        End Sub
#End Region
#End Region

#Region "Hidden Properties & Collections"

        Public Property ProductID() As Integer

#Region "Size"
        Public mappedSize As String
        Friend mySize As TextString

        Public ReadOnly Property Size As TextString
            Get
                Return If(mySize, New TextString(mappedSize, Function(v) mappedSize = v))
            End Get
        End Property

        Public Sub AboutSize(a As FieldAbout, Size As TextString)
            Select Case a.TypeCode
                Case AboutTypeCodes.Visible
                    a.Visible = False
            End Select
        End Sub
#End Region

#Region "SizeUnitMeasureCode"
        Public mappedSizeUnitMeasureCode As String
        Friend mySizeUnitMeasureCode As TextString

        <MemberOrder(1)>
        Public ReadOnly Property SizeUnitMeasureCode As TextString
            Get
                Return If(mySizeUnitMeasureCode, New TextString(mappedSizeUnitMeasureCode, Function(v) mappedSizeUnitMeasureCode = v))
            End Get
        End Property

        Public Sub AboutSizeUnitMeasureCode(a As FieldAbout, SizeUnitMeasureCode As TextString)
            Select Case a.TypeCode
                Case AboutTypeCodes.Visible
                    a.Visible = False
            End Select
        End Sub
#End Region

        '<Hidden>
        Public Overridable Property SizeUnit() As UnitMeasure

        '<Hidden>
        Public Property Weight() As Decimal?

        '<Hidden>
        Public Property WeightUnitMeasureCode() As String

        '<Hidden>
        Public Overridable Property WeightUnit() As UnitMeasure

        '<Hidden>
        Public Property ProductModelID() As Integer?

        '<Hidden>
        Public Property ProductSubcategoryID() As Integer?

        '<Hidden>
        Public Property RowGuid() As Guid

        Public Overridable Property ProductProductPhoto() As ICollection(Of ProductProductPhoto) = New List(Of ProductProductPhoto)()

        '<Hidden>
        Public Overridable Property SpecialOfferProduct() As ICollection(Of SpecialOfferProduct) = New List(Of SpecialOfferProduct)()


#End Region

        Public Function Title() As ITitle Implements ITitledObject.Title
            Return New Title(ToString())
        End Function

        Public Overrides Function ToString() As String
            Return mappedName
        End Function

#Region "Actions"
        Public Shared Function ActionOrder() As String
            Return "ActionBestSpecialOffer, ActionAssociatewithspecialOffer,  actionCurrentWorkOrders,ActionCreateNewWorkOrder"
        End Function

        Public Sub ActionAssociateWithSpecialOffer()
            Throw New NotImplementedException()
        End Sub

        Public Function ActionBestSpecialOffer(quantity As WholeNumber) As SpecialOffer
            Dim pid = ProductID
            Dim today = Date.Today()
            Dim qty = quantity.Value
            Return (From sop In Container.Instances(Of SpecialOfferProduct)
                    Where sop.Product.ProductID = pid AndAlso
                           sop.SpecialOffer.mappedStartDate <= today AndAlso
                           sop.SpecialOffer.mappedMinQty < qty
                    Order By sop.SpecialOffer.mappedDiscountPct Descending
                    Select sop.SpecialOffer).FirstOrDefault()
        End Function

        Public Function ActionCreateNewWorkOrder() As WorkOrder
            Throw New NotImplementedException()
        End Function

        Public Function ActionCurrentWorkOrders() As IQueryable(Of WorkOrder)
            Dim pid = Me.ProductID
            Return From w In Container.Instances(Of WorkOrder)
                   Where w.ProductID = pid AndAlso
                       w.mappedEndDate Is Nothing
        End Function


#End Region

    End Class

End Namespace