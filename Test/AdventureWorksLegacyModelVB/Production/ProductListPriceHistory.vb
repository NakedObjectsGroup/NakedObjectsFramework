Namespace AW.Types

	Partial Public Class ProductListPriceHistory

        Public Property ProductID() As Integer

#Region "StartDate"
        Friend mappedStartDate As Date
        Friend myStartDate As NODate

        <MemberOrder(1)>
        Public ReadOnly Property StartDate As NODate
            Get
                Return If(myStartDate, New NODate(mappedStartDate, Function(v) mappedStartDate = v))
            End Get
        End Property

        Public Sub AboutStartDate(a As FieldAbout, StartDate As NODate)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case AboutTypeCodes.Visible
            End Select
        End Sub
#End Region

#Region "EndDate"
        Friend mappedEndDate As Date?
        Friend myEndDate As NODate

        <MemberOrder(1)>
        Public ReadOnly Property EndDate As NODate
            Get
                Return If(myEndDate, New NODate(mappedEndDate, Function(v) mappedEndDate = v))
            End Get
        End Property

        Public Sub AboutEndDate(a As FieldAbout, EndDate As NODate)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case AboutTypeCodes.Visible
            End Select
        End Sub
#End Region

#Region "ListPrice"
        Friend mappedListPrice As Decimal
        Friend myListPrice As Money

        <MemberOrder(1)>
        Public ReadOnly Property ListPrice As Money
            Get
                Return If(myListPrice, New Money(mappedListPrice, Function(v) mappedListPrice = v))
            End Get
        End Property

        Public Sub AboutListPrice(a As FieldAbout, ListPrice As Money)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case AboutTypeCodes.Visible
            End Select
        End Sub
#End Region

        Public Overridable Property Product() As Product

#Region "ModifiedDate"
        Friend mappedModifiedDate As Date
        Friend myModifiedDate As TimeStamp

        <MemberOrder(99)>
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

        Public Function Title() As Title
            Return New Title($"ProductListPriceHistory: {ProductID}")
        End Function
    End Class
End Namespace