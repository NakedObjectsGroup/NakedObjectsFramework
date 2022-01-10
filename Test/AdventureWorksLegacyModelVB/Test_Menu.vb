Namespace AW.Types
    'Test retrieval of all types
    Public Class Test_Menu

        Public Shared Function ActionRandomEmployeeDepartmentHistory() As EmployeeDepartmentHistory
            Return SimpleRepository.Random(Of EmployeeDepartmentHistory)()
        End Function

        Public Shared Function ActionRandomEmployeePayHistory() As EmployeePayHistory
            Return SimpleRepository.Random(Of EmployeePayHistory)()
        End Function

        Public Shared Function ActionRandomBusinessEntityAddress() As BusinessEntityAddress
            Return SimpleRepository.Random(Of BusinessEntityAddress)()
        End Function

        Public Shared Function ActionRandomBusinessEntityContact() As BusinessEntityContact
            Return SimpleRepository.Random(Of BusinessEntityContact)()
        End Function

        Public Shared Function ActionRandomTransactionHistory() As TransactionHistory
            Return SimpleRepository.Random(Of TransactionHistory)()
        End Function

        Public Shared Function ActionRandomPassword() As Password
            Return SimpleRepository.Random(Of Password)()
        End Function

        Public Shared Function ActionRandomCurrency() As Currency
            Return SimpleRepository.Random(Of Currency)()
        End Function

        Public Shared Function ActionRandomBillOfMaterial() As BillOfMaterial
            Return SimpleRepository.Random(Of BillOfMaterial)()
        End Function

        Public Shared Function ActionRandomSalesTerritoryHistory() As SalesTerritoryHistory
            Return SimpleRepository.Random(Of SalesTerritoryHistory)()
        End Function

        Public Shared Function ActionRandomProduct() As Product
            Return SimpleRepository.Random(Of Product)()
        End Function

        'Lots more Product-related types
        '//AccessInstanceWithTitle("ProductCostHistory--707--632557728000000000", "€ 12.0278 7/1/2005 12:00:00 AM~");
        '//AccessInstanceWithTitle("ProductInventory--1--1", "408 in Tool Crib - A");
        '//AccessInstanceWithTitle("ProductListPriceHistory--707--632557728000000000", "ProductListPriceHistory: 707"); 
        '//AccessInstanceWithTitle("ProductModel--1", "Classic Vest");
        '//AccessInstanceWithTitle("ProductModelIllustration--7--3", "ProductModelIllustration: 7-3"); 
        '//AccessInstanceWithTitle("ProductModelProductDescriptionCulture--1--1199--en%20%20%20%20", "ProductModelProductDescriptionCulture: 1-1199-en");
        '//AccessInstanceWithTitle("ProductProductPhoto--1--1", "ProductProductPhoto: 1-1")
        '//AccessInstanceWithTitle("WorkOrder--1", "LL Road Frame - Black, 58: 7/4/2005 12:00:00 AM");
        '//AccessInstanceWithTitle("WorkOrderRouting--13--747--1", "Frame Forming");
        ' //AccessInstanceWithTitle("ProductVendor--1--1580", "ProductVendor: 1-1580");
        '//AccessInstanceWithTitle("PurchaseOrderDetail--1--1", "4 x Adjustable Race");
        '//AccessInstanceWithTitle("SalesOrderDetail--43659--1", "1 x Mountain-100 Black, 42");
        '//AccessInstanceWithTitle("ShoppingCartItem--2", "3 x Full-Finger Gloves, M");
        '//AccessInstanceWithTitle("SpecialOfferProduct--1--680", "SpecialOfferProduct: 1-680");


    End Class
End Namespace
