﻿Namespace AW.Types

    Partial Public Class CurrencyRate

        Implements ITitledObject, INotEditableOncePersistent
        ''<Hidden>
        Public Property CurrencyRateID() As Integer

#Region "CurrencyRateDate"
        Public Property mappedCurrencyRateDate As Date
        Friend myCurrencyRateDate As NODate

        <DemoProperty(Order:=1)>
        Public ReadOnly Property CurrencyRateDate As NODate
            Get
                myCurrencyRateDate = If(myCurrencyRateDate, New NODate(mappedCurrencyRateDate, Sub(v) mappedCurrencyRateDate = v))
Return myCurrencyRateDate
            End Get
        End Property

        Public Sub AboutCurrencyRateDate(a As FieldAbout, CurrencyRateDate As NODate)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case Else
            End Select
        End Sub
#End Region

#Region "AverageRate"
        Public Property mappedAverageRate As Decimal
        Friend myAverageRate As Money

        <DemoProperty(Order:=1)>
        Public ReadOnly Property AverageRate As Money
            Get
                myAverageRate = If(myAverageRate, New Money(mappedAverageRate, Sub(v) mappedAverageRate = v))
Return myAverageRate
            End Get
        End Property

        Public Sub AboutAverageRate(a As FieldAbout, AverageRate As Money)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case Else
            End Select
        End Sub
#End Region

#Region "EndOfDayRate"
        Public Property mappedEndOfDayRate As Decimal
        Friend myEndOfDayRate As Money

        <DemoProperty(Order:=1)>
        Public ReadOnly Property EndOfDayRate As Money
            Get
                myEndOfDayRate = If(myEndOfDayRate, New Money(mappedEndOfDayRate, Sub(v) mappedEndOfDayRate = v))
Return myEndOfDayRate
            End Get
        End Property

        Public Sub AboutEndOfDayRate(a As FieldAbout, EndOfDayRate As Money)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case Else
            End Select
        End Sub
#End Region

        ''<Hidden>
        Public Property FromCurrencyCode() As String

        Public Overridable Property Currency() As Currency

        ''<Hidden>
        Public Property ToCurrencyCode() As String

        Public Overridable Property Currency1() As Currency

#Region "ModifiedDate"
        Public Property mappedModifiedDate As Date
        Friend myModifiedDate As TimeStamp

        <DemoProperty(Order:=99)>
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
            Return mappedAverageRate.ToString()
        End Function
    End Class
End Namespace