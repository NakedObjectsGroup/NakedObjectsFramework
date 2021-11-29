'====================================================================================================
'The Free Edition of Instant VB limits conversion output to 100 lines per file.

'To purchase the Premium Edition, visit our website:
'https://www.tangiblesoftwaresolutions.com/order/order-instant-vb.html
'====================================================================================================


Imports NakedFramework.Value

Namespace AW.Types


    Partial Public Class Product
        Implements IProduct, IHasModifiedDate, IHasRowGuid

#Region "Visible properties"
        <MemberOrder(1)>
        Public Property Name() As String Implements IProduct.Name

        <MemberOrder(2)>
        Public Property ProductNumber() As String = ""

        'INSTANT VB WARNING: Nullable reference types have no equivalent in VB:
        'ORIGINAL LINE: public string? Color {get;set;}
        <MemberOrder(3)>
        Public Property Color() As String

        <MemberOrder(4)>
        Public Overridable ReadOnly Property Photo() As Image
            Get
                Return Nothing 'Product_Functions.Photo(Me)
            End Get
        End Property

        'INSTANT VB WARNING: Nullable reference types have no equivalent in VB:
        'ORIGINAL LINE: public virtual ProductModel? ProductModel {get;set;}
        <MemberOrder(10)>
        Public Overridable Property ProductModel() As ProductModel

        'MemberOrder 11 -  See Product_Functions.Description

        <MemberOrder(12)>
        <Mask("C")>
        Public Property ListPrice() As Decimal

        'INSTANT VB WARNING: Nullable reference types have no equivalent in VB:
        'ORIGINAL LINE: public virtual ProductCategory? ProductCategory
        <MemberOrder(13)>
        Public Overridable ReadOnly Property ProductCategory() As ProductCategory
            Get
                Return Nothing 'Me.ProductCategory()
            End Get
        End Property

        'INSTANT VB WARNING: Nullable reference types have no equivalent in VB:
        'ORIGINAL LINE: public virtual ProductSubcategory? ProductSubcategory {get;set;}
        <MemberOrder(14)>
        Public Overridable Property ProductSubcategory() As ProductSubcategory

        'INSTANT VB WARNING: Nullable reference types have no equivalent in VB:
        'ORIGINAL LINE: public string? ProductLine {get;set;}
        <MemberOrder(15)>
        Public Property ProductLine() As String

        <Named("Size")>
        <MemberOrder(16)>
        Public ReadOnly Property SizeWithUnit() As String
            Get
                Return Nothing 'Me.SizeWithUnit()
            End Get
        End Property

        <Named("Weight")>
        <MemberOrder(17)>
        Public ReadOnly Property WeightWithUnit() As String
            Get
                Return Nothing 'Me.WeightWithUnit()
            End Get
        End Property

        'INSTANT VB WARNING: Nullable reference types have no equivalent in VB:
        'ORIGINAL LINE: public string? Style {get;set;}
        <MemberOrder(18)>
        Public Property Style() As String

        'INSTANT VB WARNING: Nullable reference types have no equivalent in VB:
        'ORIGINAL LINE: public string? @Class {get;set;}
        <MemberOrder(19)>
        Public Property [Class]() As String

        <MemberOrder(20)>
        Public Property Make() As Boolean

        <MemberOrder(21)>
        Public Overridable Property FinishedGoods() As Boolean

        <MemberOrder(22)>
        Public Property SafetyStockLevel() As Short

        <MemberOrder(23)>
        Public Property ReorderPoint() As Short

        <MemberOrder(24)>
        Public Property DaysToManufacture() As Integer

        <MemberOrder(81)>
        <Mask("d")>
        Public Property SellStartDate() As DateTime

        <MemberOrder(82)>
        <Mask("d")>
        Public Property SellEndDate() As DateTime?

        <MemberOrder(83)>
        <Mask("d")>
        Public Property DiscontinuedDate() As DateTime?

        <MemberOrder(90)>
        <Mask("C")>
        Public Property StandardCost() As Decimal

        <MemberOrder(99)>
        Public Property ModifiedDate() As DateTime Implements IHasModifiedDate.ModifiedDate

#End Region

#Region "Visible Collections"

        <MemberOrder(100)>
        <TableView(True, NameOf(ProductReview.Rating), NameOf(ProductReview.Comments))>
        Public Overridable Property ProductReviews() As ICollection(Of ProductReview) = New List(Of ProductReview)()

        <MemberOrder(120)>
        <TableView(False, NameOf(Types.ProductInventory.Quantity), NameOf(Types.ProductInventory.Location), NameOf(Types.ProductInventory.Shelf), NameOf(Types.ProductInventory.Bin))>
        Public Overridable Property ProductInventory() As ICollection(Of ProductInventory) = New List(Of ProductInventory)()

#End Region

#Region "Hidden Properties & Collections"

        <Hidden>
        Public Property ProductID() As Integer Implements IProduct.ProductID

        'INSTANT VB WARNING: Nullable reference types have no equivalent in VB:
        'ORIGINAL LINE: public string? Size {get;set;}
        <Hidden>
        Public Property Size() As String

        'INSTANT VB WARNING: Nullable reference types have no equivalent in VB:
        'ORIGINAL LINE: public string? SizeUnitMeasureCode {get;set;}
        <Hidden>
        Public Property SizeUnitMeasureCode() As String

        <Hidden>
        Public Overridable Property SizeUnit() As UnitMeasure

        <Hidden>
        Public Property Weight() As Decimal?

        'INSTANT VB WARNING: Nullable reference types have no equivalent in VB:
        'ORIGINAL LINE: public string? WeightUnitMeasureCode {get;set;}
        <Hidden>
        Public Property WeightUnitMeasureCode() As String

        <Hidden>
        Public Overridable Property WeightUnit() As UnitMeasure

        <Hidden>
        Public Property ProductModelID() As Integer?

        <Hidden>
        Public Property ProductSubcategoryID() As Integer?

        <Hidden>
        Public Property rowguid() As Guid Implements IHasRowGuid.rowguid

        Public Overridable Property ProductProductPhoto() As ICollection(Of ProductProductPhoto) = New List(Of ProductProductPhoto)()

        <Hidden>
        Public Overridable Property SpecialOfferProduct() As ICollection(Of SpecialOfferProduct) = New List(Of SpecialOfferProduct)()

#End Region

        Public Overrides Function ToString() As String
            Return Name
        End Function

    End Class

End Namespace