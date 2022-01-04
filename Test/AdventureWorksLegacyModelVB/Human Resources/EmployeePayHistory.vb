Namespace AW.Types

	Partial Public Class EmployeePayHistory

		Public Property EmployeeID() As Integer

#Region "RateChangeDate"
        Public mappedRateChangeDate As Date
        Friend myRateChangeDate As NODate

        '<MemberOrder(1)>
        Public ReadOnly Property RateChangeDate As NODate
            Get
                Return If(myRateChangeDate, New NODate(mappedRateChangeDate, Function(v) mappedRateChangeDate = v))
            End Get
        End Property

        Public Sub AboutRateChangeDate(a As FieldAbout, RateChangeDate As NODate)
            Select Case a.TypeCode
                Case AboutTypeCodes.Usable
                    a.Usable = False
            End Select
        End Sub
#End Region

#Region "Rate"
        Public mappedRate As Decimal
        Friend myRate As Money

        '<MemberOrder(2)>
        Public ReadOnly Property Rate As Money
            Get
                Return If(myRate, New Money(mappedRate, Function(v) mappedRate = v))
            End Get
        End Property

        Public Sub AboutRate(a As FieldAbout, Rate As Money)
            Select Case a.TypeCode
                Case AboutTypeCodes.Usable
                    a.Usable = False
            End Select
        End Sub
#End Region

#Region "PayFrequency"
        Public mappedPayFrequency As Byte
        Friend myPayFrequency As WholeNumber

        '<MemberOrder(3)>
        Public ReadOnly Property PayFrequency As WholeNumber
            Get
                Return If(myPayFrequency, New WholeNumber(mappedPayFrequency, Function(v) mappedPayFrequency = v))
            End Get
        End Property

        Public Sub AboutPayFrequency(a As FieldAbout, PayFrequency As WholeNumber)
            Select Case a.TypeCode
                Case AboutTypeCodes.Usable
                    a.Usable = False
            End Select
        End Sub
#End Region

        '<MemberOrder(4)>
        Public Overridable Property Employee() As Employee

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

        Public Function Title() As Title
            Return New Title($"{Rate.ToString("C")} from {RateChangeDate.ToString("d")}")
        End Function
    End Class
End Namespace