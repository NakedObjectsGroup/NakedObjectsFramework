Namespace AW.Types
    'Test retrieval of all types relating to Product
    Public Class Products

        Public Shared Function ActionRandomProduct() As Product
            Return SimpleRepository.Random(Of Product)()
        End Function

        Public Shared Function ActionRandomProductCostHistory() As ProductCostHistory
            Return SimpleRepository.Random(Of ProductCostHistory)()
        End Function

        Public Shared Function ActionRandomProductInventory() As ProductInventory
            Return SimpleRepository.Random(Of ProductInventory)()
        End Function

        Public Shared Function ActionRandomProductListPriceHistory() As ProductListPriceHistory
            Return SimpleRepository.Random(Of ProductListPriceHistory)()
        End Function

        Public Shared Function ActionRandomProductModel() As ProductModel
            Return SimpleRepository.Random(Of ProductModel)()
        End Function

        Public Shared Function ActionRandomProductModelIllustration() As ProductModelIllustration
            Return SimpleRepository.Random(Of ProductModelIllustration)()
        End Function

        Public Shared Function ActionRandomProductModelProductDescriptionCulture() As ProductModelProductDescriptionCulture
            Return SimpleRepository.Random(Of ProductModelProductDescriptionCulture)()
        End Function

        Public Shared Function ActionRandomProductProductPhoto() As ProductProductPhoto
            Return SimpleRepository.Random(Of ProductProductPhoto)()
        End Function

        Public Shared Function ActionRandomProductVendor() As ProductVendor
            Return SimpleRepository.Random(Of ProductVendor)()
        End Function

    End Class
End Namespace
