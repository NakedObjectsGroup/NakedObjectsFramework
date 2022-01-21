Namespace AW.Types

	Partial Public Class TransactionHistory
 Implements ITitledObject

		Public Property TransactionID() As Integer

		Public Property ReferenceOrderID() As Integer

		Public Property ReferenceOrderLineID() As Integer

#Region "TransactionDate"
        Public Property mappedTransactionDate As Date
        Friend myTransactionDate As NODate

        <MemberOrder(1)>
        Public ReadOnly Property TransactionDate As NODate
            Get
                myTransactionDate = If(myTransactionDate, New NODate(mappedTransactionDate, Sub(v) mappedTransactionDate = v))
Return myTransactionDate
            End Get
        End Property

        Public Sub AboutTransactionDate(a As FieldAbout, TransactionDate As NODate)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case Else
            End Select
        End Sub
#End Region

#Region "TransactionType"
        Public Property mappedTransactionType As String
        Friend myTransactionType As TextString

        <MemberOrder(1)>
        Public ReadOnly Property TransactionType As TextString
            Get
                myTransactionType = If(myTransactionType, New TextString(mappedTransactionType, Sub(v) mappedTransactionType = v))
Return myTransactionType
            End Get
        End Property

        Public Sub AboutTransactionType(a As FieldAbout, TransactionType As TextString)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case Else
            End Select
        End Sub
#End Region

#Region "Quantity"
        Public Property mappedQuantity As Integer
        Friend myQuantity As WholeNumber

        <MemberOrder(1)>
        Public ReadOnly Property Quantity As WholeNumber
            Get
                myQuantity = If(myQuantity, New WholeNumber(mappedQuantity, Sub(v) mappedQuantity = v))
Return myQuantity
            End Get
        End Property

        Public Sub AboutQuantity(a As FieldAbout, Quantity As WholeNumber)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case Else
            End Select
        End Sub
#End Region

#Region "ActualCost"
        Public Property mappedActualCost As Decimal
        Friend myActualCost As Money

        <MemberOrder(1)>
        Public ReadOnly Property ActualCost As Money
            Get
                myActualCost = If(myActualCost, New Money(mappedActualCost, Sub(v) mappedActualCost = v))
Return myActualCost
            End Get
        End Property

        Public Sub AboutActualCost(a As FieldAbout, ActualCost As Money)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case Else
            End Select
        End Sub
#End Region

        Public Property ProductID() As Integer

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
            Return $"TransactionHistory: {TransactionID}"
        End Function
    End Class
End Namespace