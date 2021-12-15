Namespace AW.Types

	Partial Public Class SalesPersonQuotaHistory
        '<Hidden>
        Public Property BusinessEntityID() As Integer

#Region "QuotaDate"
        Friend mappedQuotaDate As Date
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
        Friend mappedSalesQuota As Decimal
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
        Friend mappedModifiedDate As Date
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
        Public Property rowguid() As Guid

        Public Function Title() As Title
            Return New Title($"{QuotaDate.ToString("d")} {SalesQuota.ToString("C")}")
        End Function
    End Class
End Namespace