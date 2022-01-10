Namespace AW.Types

    Partial Public Class SalesPersonQuotaHistory

        Implements ITitledObject
        '<Hidden>
        Public Property BusinessEntityID() As Integer

#Region "QuotaDate"
        Public Property mappedQuotaDate As Date
        Friend myQuotaDate As NODate

        '<MemberOrder(1)>
        Public ReadOnly Property QuotaDate As NODate
            Get
                Return If(myQuotaDate, New NODate(mappedQuotaDate, Function(v) mappedQuotaDate = v))
            End Get
        End Property

        Public Sub AboutQuotaDate(a As FieldAbout, QuotaDate As NODate)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case AboutTypeCodes.Visible
            End Select
        End Sub
#End Region

#Region "SalesQuota"
        Public mappedSalesQuota As Decimal
        Friend mySalesQuota As Money

        '<MemberOrder(2)>
        Public ReadOnly Property SalesQuota As Money
            Get
                Return If(mySalesQuota, New Money(mappedSalesQuota, Function(v) mappedSalesQuota = v))
            End Get
        End Property

        Public Sub AboutSalesQuota(a As FieldAbout, SalesQuota As Money)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case AboutTypeCodes.Visible
            End Select
        End Sub
#End Region

        '<MemberOrder(3)>
        Public Overridable Property SalesPerson() As SalesPerson

#Region "ModifiedDate"
        Public mappedModifiedDate As Date
        Friend myModifiedDate As TimeStamp

        '<MemberOrder(99)>
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

        '<Hidden>
        Public Property RowGuid() As Guid

        Public Function Title() As ITitle Implements ITitledObject.Title
            Return New Title(ToString())
        End Function

        Public Overrides Function ToString() As String
            Return $"{QuotaDate} {SalesQuota}"
        End Function
    End Class
End Namespace