Imports AW.Services
Imports NakedFramework.Value

Namespace AW.Types

    Partial Public Class Product
        Implements ITitledObject, INotEditableOncePersistent, IContainerAware


#Region "Container"

        Public Property Container As IContainer Implements IContainerAware.Container
#End Region

#Region "Visible properties"
#Region "Name"
        Public Property mappedName As String
        Friend myName As TextString

        <AWProperty(Order:=1)>
        Public ReadOnly Property Name As TextString
            Get
                myName = If(myName, New TextString(mappedName, Sub(v) mappedName = v))
                Return myName
            End Get
        End Property

        Public Sub AboutName(a As FieldAbout, Name As TextString)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case Else
            End Select
        End Sub
#End Region

#Region "ProductNumber"
        Public Property mappedProductNumber As String
        Friend myProductNumber As TextString

        <AWProperty(Order:=2)>
        Public ReadOnly Property ProductNumber As TextString
            Get
                myProductNumber = If(myProductNumber, New TextString(mappedProductNumber, Sub(v) mappedProductNumber = v))
                Return myProductNumber
            End Get
        End Property

        Public Sub AboutProductNumber(a As FieldAbout, ProductNumber As TextString)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case Else
            End Select
        End Sub
#End Region

#Region "Color"
        Public Property mappedColor As String
        Friend myColor As TextString

        <AWProperty(Order:=3)>
        Public ReadOnly Property Color As TextString
            Get
                myColor = If(myColor, New TextString(mappedColor, Sub(v) mappedColor = v))
                Return myColor
            End Get
        End Property

        Public Sub AboutColor(a As FieldAbout, Color As TextString)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case Else
            End Select
        End Sub
#End Region

        <AWProperty(Order:=4)>
        Public Overridable ReadOnly Property Photo() As Image
            Get
                Return Nothing 'Product_Functions.Photo(Me)
            End Get
        End Property

        <AWProperty(Order:=10)>
        Public Overridable Property ProductModel() As ProductModel

#Region "ListPrice"
        Public Property mappedListPrice As Decimal
        Friend myListPrice As Money

        <AWProperty(Order:=12)>
        Public ReadOnly Property ListPrice As Money
            Get
                myListPrice = If(myListPrice, New Money(mappedListPrice, Sub(v) mappedListPrice = v))
                Return myListPrice
            End Get
        End Property

        Public Sub AboutListPrice(a As FieldAbout, ListPrice As Money)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case Else
            End Select
        End Sub
#End Region

        <AWProperty(Order:=13)>
        Public Overridable ReadOnly Property ProductCategory() As ProductCategory
            Get
                Return Nothing 'TODO: ProductSubcategory.ProductCategory
            End Get
        End Property

        <AWProperty(Order:=14)>
        Public Overridable Property ProductSubcategory() As ProductSubcategory

#Region "ProductLine"
        Public Property mappedProductLine As String
        Friend myProductLine As TextString

        <AWProperty(Order:=15)>
        Public ReadOnly Property ProductLine As TextString
            Get
                myProductLine = If(myProductLine, New TextString(mappedProductLine, Sub(v) mappedProductLine = v))
                Return myProductLine
            End Get
        End Property

        Public Sub AboutProductLine(a As FieldAbout, ProductLine As TextString)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case Else
            End Select
        End Sub
#End Region

#Region "SizeWithUnit"


        <AWProperty(Order:=16)>
        Public ReadOnly Property SizeWithUnit As TextString
            Get
                Return New TextString($"{Size} {SizeUnit}")
            End Get
        End Property

        Public Sub AboutSizeWithUnit(a As FieldAbout, SizeWithUnit As TextString)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                    a.Name = "Size"
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case Else
            End Select
        End Sub
#End Region

#Region "WeightWithUnit"

        <AWProperty(Order:=17)>
        Public ReadOnly Property WeightWithUnit As TextString
            Get
                Return New TextString($"{Weight} {WeightUnit}")
            End Get
        End Property

        Public Sub AboutWeightWithUnit(a As FieldAbout, WeightWithUnit As TextString)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                    a.Name = "Weight"
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case Else
            End Select
        End Sub
#End Region

#Region "Style"
        Public Property mappedStyle As String
        Friend myStyle As TextString

        <AWProperty(Order:=18)>
        Public ReadOnly Property Style As TextString
            Get
                myStyle = If(myStyle, New TextString(mappedStyle, Sub(v) mappedStyle = v))
                Return myStyle
            End Get
        End Property

        Public Sub AboutStyle(a As FieldAbout, Style As TextString)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case Else
            End Select
        End Sub
#End Region

#Region "Class"
        Public Property mappedClass As String
        Friend My_Class As TextString

        <AWProperty(Order:=19)>
        Public ReadOnly Property [Class] As TextString
            Get
                My_Class = If(My_Class, New TextString(mappedClass, Sub(v) mappedClass = v))
                Return My_Class
            End Get
        End Property

        Public Sub AboutClass(a As FieldAbout, [Class] As TextString)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case Else
            End Select
        End Sub
#End Region

#Region "Make"
        Public Property mappedMake As Boolean
        Friend myMake As Logical

        <AWProperty(Order:=20)>
        Public ReadOnly Property Make As Logical
            Get
                myMake = If(myMake, New Logical(mappedMake, Sub(v) mappedMake = v))
                Return myMake
            End Get
        End Property

        Public Sub AboutMake(a As FieldAbout, Make As Logical)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case Else
            End Select
        End Sub
#End Region

#Region "FinishedGoods"
        Public Property mappedFinishedGoods As Boolean
        Friend myFinishedGoods As Logical

        <AWProperty(Order:=21)>
        Public ReadOnly Property FinishedGoods As Logical
            Get
                myFinishedGoods = If(myFinishedGoods, New Logical(mappedFinishedGoods, Sub(v) mappedFinishedGoods = v))
                Return myFinishedGoods
            End Get
        End Property

        Public Sub AboutFinishedGoods(a As FieldAbout, FinishedGoods As Logical)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case Else
            End Select
        End Sub
#End Region

#Region "SafetyStockLevel"
        Public Property mappedSafetyStockLevel As Short
        Friend mySafetyStockLevel As WholeNumber

        <AWProperty(Order:=22)>
        Public ReadOnly Property SafetyStockLevel As WholeNumber
            Get
                mySafetyStockLevel = If(mySafetyStockLevel, New WholeNumber(mappedSafetyStockLevel, Sub(v) mappedSafetyStockLevel = CType(v, Short)))
                Return mySafetyStockLevel
            End Get
        End Property

        Public Sub AboutSafetyStockLevel(a As FieldAbout, SafetyStockLevel As WholeNumber)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case Else
            End Select
        End Sub
#End Region

#Region "ReorderPoint"
        Public Property mappedReorderPoint As Short
        Friend myReorderPoint As WholeNumber

        <AWProperty(Order:=23)>
        Public ReadOnly Property ReorderPoint As WholeNumber
            Get
                myReorderPoint = If(myReorderPoint, New WholeNumber(mappedReorderPoint, Sub(v) mappedReorderPoint = CType(v, Short)))
                Return myReorderPoint
            End Get
        End Property

        Public Sub AboutReorderPoint(a As FieldAbout, ReorderPoint As WholeNumber)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case Else
            End Select
        End Sub
#End Region

#Region "DaysToManufacture"
        Public Property mappedDaysToManufacture As Integer
        Friend myDaysToManufacture As WholeNumber

        <AWProperty(Order:=24)>
        Public ReadOnly Property DaysToManufacture As WholeNumber
            Get
                myDaysToManufacture = If(myDaysToManufacture, New WholeNumber(mappedDaysToManufacture, Sub(v) mappedDaysToManufacture = v))
                Return myDaysToManufacture
            End Get
        End Property

        Public Sub AboutDaysToManufacture(a As FieldAbout, DaysToManufacture As WholeNumber)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case Else
            End Select
        End Sub
#End Region

#Region "SellStartDate"
        Public Property mappedSellStartDate As Date
        Friend mySellStartDate As NODate

        <AWProperty(Order:=81)>
        Public ReadOnly Property SellStartDate As NODate
            Get
                mySellStartDate = If(mySellStartDate, New NODate(mappedSellStartDate, Sub(v) mappedSellStartDate = v))
                Return mySellStartDate
            End Get
        End Property

        Public Sub AboutSellStartDate(a As FieldAbout, SellStartDate As NODate)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case Else
            End Select
        End Sub
#End Region

#Region "SellEndDate"
        Public Property mappedSellEndDate As Date?
        Friend mySellEndDate As NODateNullable

        <AWProperty(Order:=1)>
        Public ReadOnly Property SellEndDate As NODateNullable
            Get
                mySellEndDate = If(mySellEndDate, New NODateNullable(mappedSellEndDate, Sub(v) mappedSellEndDate = v))
                Return mySellEndDate
            End Get
        End Property

        Public Sub AboutSellEndDate(a As FieldAbout, SellEndDate As NODateNullable)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case Else
            End Select
        End Sub
#End Region

#Region "DiscontinuedDate"
        Public Property mappedDiscontinuedDate As Date?
        Friend myDiscontinuedDate As NODateNullable

        <AWProperty(Order:=1)>
        Public ReadOnly Property DiscontinuedDate As NODateNullable
            Get
                myDiscontinuedDate = If(myDiscontinuedDate, New NODateNullable(mappedDiscontinuedDate, Sub(v) mappedDiscontinuedDate = v))
                Return myDiscontinuedDate
            End Get
        End Property

        Public Sub AboutDiscontinuedDate(a As FieldAbout, DiscontinuedDate As NODateNullable)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case Else
            End Select
        End Sub
#End Region

#Region "StandardCost"
        Public Property mappedStandardCost As Decimal
        Friend myStandardCost As Money

        <AWProperty(Order:=90)>
        Public ReadOnly Property StandardCost As Money
            Get
                myStandardCost = If(myStandardCost, New Money(mappedStandardCost, Sub(v) mappedStandardCost = v))
                Return myStandardCost
            End Get
        End Property

        Public Sub AboutStandardCost(a As FieldAbout, StandardCost As Money)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case Else
            End Select
        End Sub
#End Region

#Region "ModifiedDate"
        Public Property mappedModifiedDate As Date
        Friend myModifiedDate As TimeStamp

        <AWProperty(Order:=99)>
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

#End Region

#Region "Visible Collections"

#Region "ProductReviews (Collection)"
        Public Overridable Property mappedProductReviews As ICollection(Of ProductReview) = New List(Of ProductReview)()

        Private myProductReviews As InternalCollection

        <AWProperty(Order:=100)>
        Public ReadOnly Property ProductReviews As InternalCollection
            Get
                myProductReviews = If(myProductReviews, New InternalCollection(Of ProductReview)(mappedProductReviews))
                Return myProductReviews
            End Get
        End Property

        Public Sub AboutProductReviews(a As FieldAbout)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case Else
            End Select
        End Sub
#End Region

#Region "ProductInventory (Collection)"
        Public Overridable Property mappedProductInventory As ICollection(Of ProductInventory) = New List(Of ProductInventory)()

        Private myProductInventory As InternalCollection

        <AWProperty(Order:=120)>
        Public ReadOnly Property ProductInventory As InternalCollection
            Get
                myProductInventory = If(myProductInventory, New InternalCollection(Of ProductInventory)(mappedProductInventory))
                Return myProductInventory
            End Get
        End Property

        Public Sub AboutProductInventory(a As FieldAbout)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case Else
            End Select
        End Sub
#End Region
#End Region

#Region "Hidden Properties & Collections"

        Public Property ProductID() As Integer

#Region "Size"
        Public Property mappedSize As String
        Friend mySize As TextString

        Public ReadOnly Property Size As TextString
            Get
                mySize = If(mySize, New TextString(mappedSize, Sub(v) mappedSize = v))
                Return mySize
            End Get
        End Property

        Public Sub AboutSize(a As FieldAbout, Size As TextString)
            Select Case a.TypeCode
                Case Else
                    a.Visible = False
            End Select
        End Sub
#End Region

#Region "SizeUnitMeasureCode"
        Public Property mappedSizeUnitMeasureCode As String
        Friend mySizeUnitMeasureCode As TextString

        <AWProperty(Order:=1)>
        Public ReadOnly Property SizeUnitMeasureCode As TextString
            Get
                mySizeUnitMeasureCode = If(mySizeUnitMeasureCode, New TextString(mappedSizeUnitMeasureCode, Sub(v) mappedSizeUnitMeasureCode = v))
                Return mySizeUnitMeasureCode
            End Get
        End Property

        Public Sub AboutSizeUnitMeasureCode(a As FieldAbout, SizeUnitMeasureCode As TextString)
            Select Case a.TypeCode
                Case Else
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

        Public Function Title() As Title Implements ITitledObject.Title
            Return New Title(ToString())
        End Function

        Public Overrides Function ToString() As String
            Return mappedName
        End Function

#Region "Actions"

        Public Sub ActionAssociateWithSpecialOffer(offer As SpecialOffer)
            offer.ActionIncludeProduct(Me)
        End Sub

        Public Function ActionBestSpecialOffer(quantity As WholeNumber) As SpecialOffer
            Dim pid = ProductID
            Dim today = Date.Today()
            Dim qty = quantity.Value
            Return (From sop In Container.AllInstances(Of SpecialOfferProduct)
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
            Return From w In Container.AllInstances(Of WorkOrder)
                   Where w.ProductID = pid AndAlso
                       w.mappedEndDate Is Nothing
        End Function

        Public Sub ActionChangeSubcategory(newSubcategory As ProductSubcategory)
            ProductSubcategoryID = newSubcategory.ProductSubcategoryID
            'Simpler to just set the ProductSubcategory property, but using Id here as a deliberate test
        End Sub

#End Region

        Public Shared Function MenuOrder() As Menu
            Dim main = New Menu("FooBar") 'This should have no effect - the object menu is always labelled 'Actions'
            main.AddAction(NameOf(ActionBestSpecialOffer)) _
            .AddAction(NameOf(ActionAssociateWithSpecialOffer)) _
            .AddAction(NameOf(ActionChangeSubCategory))

            main.AddSubMenu("Work Orders") _
            .AddAction(NameOf(ActionCurrentWorkOrders)) _
            .AddAction(NameOf(ActionCreateNewWorkOrder))

            Return main
        End Function



    End Class

End Namespace