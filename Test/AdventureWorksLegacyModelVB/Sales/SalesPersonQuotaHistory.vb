Namespace AW.Types

    Partial Public Class SalesPersonQuotaHistory

        Implements ITitledObject, INotEditableOncePersistent
        '<Hidden>
        Public Property BusinessEntityID() As Integer

#Region "QuotaDate"
        Public Property mappedQuotaDate As Date
        Friend myQuotaDate As NODate

        <AWProperty(Order:=1)>
        Public ReadOnly Property QuotaDate As NODate
            Get
                myQuotaDate = If(myQuotaDate, New NODate(mappedQuotaDate, Sub(v) mappedQuotaDate = v))
Return myQuotaDate
            End Get
        End Property

        Public Sub AboutQuotaDate(a As FieldAbout, QuotaDate As NODate)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case Else
            End Select
        End Sub
#End Region

#Region "SalesQuota"
        Public Property mappedSalesQuota As Decimal
        Friend mySalesQuota As Money

        <AWProperty(Order:=2)>
        Public ReadOnly Property SalesQuota As Money
            Get
                mySalesQuota = If(mySalesQuota, New Money(mappedSalesQuota, Sub(v) mappedSalesQuota = v))
Return mySalesQuota
            End Get
        End Property

        Public Sub AboutSalesQuota(a As FieldAbout, SalesQuota As Money)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case Else
            End Select
        End Sub
#End Region

        <AWProperty(Order:=3)>
        Public Overridable Property SalesPerson() As SalesPerson

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

        '<Hidden>
        Public Property RowGuid() As Guid

        Public Function Title() As Title Implements ITitledObject.Title
            Return New Title(ToString())
        End Function

        Public Overrides Function ToString() As String
            Return $"{QuotaDate} {SalesQuota}"
        End Function
    End Class
End Namespace