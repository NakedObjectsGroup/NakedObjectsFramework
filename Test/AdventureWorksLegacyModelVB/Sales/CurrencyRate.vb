Namespace AW.Types

	Partial Public Class CurrencyRate
		'<Hidden>
		Public Property CurrencyRateID() As Integer

#Region "CurrencyRateDate"
        Friend mappedCurrencyRateDate As Date
        Friend myCurrencyRateDate As NODate

        <MemberOrder(1)>
        Public ReadOnly Property CurrencyRateDate As NODate
            Get
                Return If(myCurrencyRateDate, New NODate(mappedCurrencyRateDate, Function(v) mappedCurrencyRateDate = v))
            End Get
        End Property

        Public Sub AboutCurrencyRateDate(a As FieldAbout, CurrencyRateDate As NODate)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case AboutTypeCodes.Visible
            End Select
        End Sub
#End Region

#Region "AverageRate"
        Friend mappedAverageRate As Decimal
        Friend myAverageRate As Money

        <MemberOrder(1)>
        Public ReadOnly Property AverageRate As Money
            Get
                Return If(myAverageRate, New Money(mappedAverageRate, Function(v) mappedAverageRate = v))
            End Get
        End Property

        Public Sub AboutAverageRate(a As FieldAbout, AverageRate As Money)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case AboutTypeCodes.Visible
            End Select
        End Sub
#End Region

#Region "EndOfDayRate"
        Friend mappedEndOfDayRate As Decimal
        Friend myEndOfDayRate As Money

        <MemberOrder(1)>
        Public ReadOnly Property EndOfDayRate As Money
            Get
                Return If(myEndOfDayRate, New Money(mappedEndOfDayRate, Function(v) mappedEndOfDayRate = v))
            End Get
        End Property

        Public Sub AboutEndOfDayRate(a As FieldAbout, EndOfDayRate As Money)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case AboutTypeCodes.Visible
            End Select
        End Sub
#End Region

        '<Hidden>
        Public Property FromCurrencyCode() As String

        Public Overridable Property Currency() As Currency

        '<Hidden>
        Public Property ToCurrencyCode() As String

        Public Overridable Property Currency1() As Currency

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
            Return New Title(AverageRate)
        End Function
    End Class
End Namespace