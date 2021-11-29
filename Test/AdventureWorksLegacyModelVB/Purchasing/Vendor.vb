Namespace AW.Types

    Partial Public Class Vendor
        Implements IBusinessEntity

        <MemberOrder(10)>
        Public Property AccountNumber() As String = ""

        <MemberOrder(20)>
        Public Property Name() As String = ""

        <MemberOrder(30)>
        Public Property CreditRating() As Byte

        <MemberOrder(40)>
        Public Overridable Property PreferredVendorStatus() As Boolean

        <MemberOrder(50)>
        Public Property ActiveFlag() As Boolean

        'INSTANT VB WARNING: Nullable reference types have no equivalent in VB:
        'ORIGINAL LINE: public string? PurchasingWebServiceURL {get;set;}
        <MemberOrder(60)>
        Public Property PurchasingWebServiceURL() As String

        <Named("Product - Order Info")>
        <AWNotCounted>
        Public Overridable Property Products() As ICollection(Of ProductVendor) = New List(Of ProductVendor)()

        <MemberOrder(99)>
        Public Property ModifiedDate() As DateTime

        <Hidden>
        Public Property BusinessEntityID() As Integer Implements IBusinessEntity.BusinessEntityID

        Public Overridable Function AutoCompletePurchasingWebServiceURL(<MinLength(2)> ByVal value As String) As IQueryable(Of String)
            Dim matchingNames = New List(Of String) From {"http://www.store1.com", "http://www.store2.com", "http://www.store3.com"}
            Return From p In matchingNames.AsQueryable()
                   Select p.Trim()
        End Function

        Public Overrides Function ToString() As String
            Return $"{Name}"
        End Function
    End Class
End Namespace