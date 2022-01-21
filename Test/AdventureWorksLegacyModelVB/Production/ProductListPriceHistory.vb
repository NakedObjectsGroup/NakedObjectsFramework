Namespace AW.Types

    Partial Public Class ProductListPriceHistory

        Implements ITitledObject

        Public Property ProductID() As Integer

#Region "StartDate"
        Public Property mappedStartDate As Date
        Friend myStartDate As NODate

        <MemberOrder(1)>
        Public ReadOnly Property StartDate As NODate
            Get
                myStartDate = If(myStartDate, New NODate(mappedStartDate, Sub(v) mappedStartDate = v))
Return myStartDate
            End Get
        End Property

        Public Sub AboutStartDate(a As FieldAbout, StartDate As NODate)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case Else
            End Select
        End Sub
#End Region

#Region "EndDate"
        Public Property mappedEndDate As Date?
        Friend myEndDate As NODateNullable

        <MemberOrder(1)>
        Public ReadOnly Property EndDate As NODateNullable
            Get
                myEndDate = If(myEndDate, New NODateNullable(mappedEndDate, Sub(v) mappedEndDate = v))
Return myEndDate
            End Get
        End Property

        Public Sub AboutEndDate(a As FieldAbout, EndDate As NODateNullable)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case Else
            End Select
        End Sub
#End Region

#Region "ListPrice"
        Public Property mappedListPrice As Decimal
        Friend myListPrice As Money

        <MemberOrder(1)>
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

        Public Overridable Property Product() As Product

#Region "ModifiedDate"
        Public Property mappedModifiedDate As Date
        Friend myModifiedDate As TimeStamp

        <MemberOrder(99)>
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

        Public Function Title() As Title Implements ITitledObject.Title
            Return New Title(ToString())
        End Function

        Public Overrides Function ToString() As String
            Return $"ProductListPriceHistory: {ProductID}"
        End Function
    End Class
End Namespace