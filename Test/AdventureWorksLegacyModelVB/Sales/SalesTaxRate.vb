Namespace AW.Types

    Partial Public Class SalesTaxRate

        Implements ITitledObject
        '<Hidden>
        Public Property SalesTaxRateID() As Integer

#Region "TaxType"
        Public mappedTaxType As Byte
        Friend myTaxType As WholeNumber

        <MemberOrder(1)>
        Public ReadOnly Property TaxType As WholeNumber
            Get
                Return If(myTaxType, New WholeNumber(mappedTaxType, Function(v) mappedTaxType = v))
            End Get
        End Property

        Public Sub AboutTaxType(a As FieldAbout, TaxType As WholeNumber)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case AboutTypeCodes.Visible
            End Select
        End Sub
#End Region

#Region "TaxRate"
        Public mappedTaxRate As Decimal
        Friend myTaxRate As FloatingPointNumber

        <MemberOrder(1)>
        Public ReadOnly Property TaxRate As FloatingPointNumber
            Get
                Return If(myTaxRate, New FloatingPointNumber(mappedTaxRate, Function(v) mappedTaxRate = v))
            End Get
        End Property

        Public Sub AboutTaxRate(a As FieldAbout, TaxRate As FloatingPointNumber)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case AboutTypeCodes.Visible
            End Select
        End Sub
#End Region

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

        ''<Hidden>
        Public Property StateProvinceID() As Integer

        Public Overridable Property StateProvince() As StateProvince

#Region "ModifiedDate"
        Public mappedModifiedDate As Date
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
            Return $"Sales Tax Rate for {StateProvince}"
        End Function
    End Class
End Namespace