Namespace AW.Types

    Partial Public Class EmployeePayHistory

        Implements ITitledObject

        Public Property EmployeeID() As Integer

#Region "RateChangeDate"
        Public Property mappedRateChangeDate As Date
        Friend myRateChangeDate As NODate

        <MemberOrder(1)>
        Public ReadOnly Property RateChangeDate As NODate
            Get
                myRateChangeDate = If(myRateChangeDate, New NODate(mappedRateChangeDate, Sub(v) mappedRateChangeDate = v))
                Return myRateChangeDate
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
        Public Property mappedRate As Decimal
        Friend myRate As Money

        <MemberOrder(2)>
        Public ReadOnly Property Rate As Money
            Get
                myRate = If(myRate, New Money(mappedRate, Sub(v) mappedRate = v))
                Return myRate
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
        Public Property mappedPayFrequency As Byte
        Friend myPayFrequency As WholeNumber

        <MemberOrder(3)>
        Public ReadOnly Property PayFrequency As WholeNumber
            Get
                myPayFrequency = If(myPayFrequency, New WholeNumber(mappedPayFrequency, Sub(v) mappedPayFrequency = CType(v, Byte)))
                Return myPayFrequency
            End Get
        End Property

        Public Sub AboutPayFrequency(a As FieldAbout, PayFrequency As WholeNumber)
            Select Case a.TypeCode
                Case AboutTypeCodes.Usable
                    a.Usable = False
            End Select
        End Sub
#End Region

        <MemberOrder(4)>
        Public Overridable Property Employee() As Employee

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

        Public Function Title() As ITitle Implements ITitledObject.Title
            Return New Title(ToString())
        End Function

        Public Overrides Function ToString() As String
            Return $"{Rate} from {RateChangeDate}"
        End Function
    End Class
End Namespace