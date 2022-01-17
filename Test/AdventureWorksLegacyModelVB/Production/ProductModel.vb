Namespace AW.Types

    Partial Public Class ProductModel

        Implements ITitledObject

        Public Property ProductModelID() As Integer

#Region "Name"
        Public Property mappedName As String
        Friend myName As TextString

        <MemberOrder(10)>
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

        ''<Hidden>
        Public Property CatalogDescription() As String


        <MemberOrder(20)> '<Named("CatalogDescription"), , MultiLine(10)>
        Public ReadOnly Property FormattedCatalogDescription() As MultiLineTextString
            Get
                Return New MultiLineTextString(CatalogDescription)
                'TODO: ProductModel_Functions.CatalogDescription(Me)
            End Get
        End Property

        <MemberOrder(22)>
        Public ReadOnly Property LocalCultureDescription() As ProductDescription
            Get
                Return Nothing
                'TODO: ProductModel_Functions.LocalCultureDescription(Me)
            End Get
        End Property

#Region "Instructions"
        Public Property mappedInstructions As String
        Friend myInstructions As TextString

        <MemberOrder(30)>
        Public ReadOnly Property Instructions As TextString
            Get
                Return If(myInstructions, New TextString(mappedInstructions, Function(v) mappedInstructions = v))
            End Get
        End Property

        Public Sub AboutInstructions(a As FieldAbout, Instructions As TextString)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case AboutTypeCodes.Visible
            End Select
        End Sub
#End Region

#Region "ProductVariants (Collection)"
        Public Overridable Property mappedProductVariants As ICollection(Of Product) = New List(Of Product)()

        Private myProductVariants As InternalCollection

        ''<TableView(True, "Name", "Number", "Color", "ProductInventory")>
        <MemberOrder(1)>
        Public ReadOnly Property ProductVariants As InternalCollection
            Get
                Return If(myProductVariants, New InternalCollection(Of Product)(mappedProductVariants))
            End Get
        End Property

        Public Sub AboutProductVariants(a As FieldAbout)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Visible
            End Select
        End Sub
#End Region

        ''<Hidden>
        Public Overridable Property ProductModelIllustration() As ICollection(Of ProductModelIllustration) = New List(Of ProductModelIllustration)()

        ''<Hidden>
        Public Overridable Property ProductModelProductDescriptionCulture() As ICollection(Of ProductModelProductDescriptionCulture) = New List(Of ProductModelProductDescriptionCulture)()

#Region "ModifiedDate"
        Public Property mappedModifiedDate As Date
        Friend myModifiedDate As TimeStamp

        <MemberOrder(99)>
        Public ReadOnly Property ModifiedDate As TimeStamp
            Get
                Return If(myModifiedDate, New TimeStamp(mappedModifiedDate, Function(v) mappedModifiedDate = v))
            End Get
        End Property

        Public Sub AboutModifiedDate(a As FieldAbout)
            Select Case a.TypeCode
                Case AboutTypeCodes.Usable
                    a.Usable = False
            End Select
        End Sub
#End Region

        Public Property RowGuid() As Guid

        Public Function Title() As ITitle Implements ITitledObject.Title
            Return New Title(ToString())
        End Function

        Public Overrides Function ToString() As String
            Return mappedName
        End Function
    End Class
End Namespace