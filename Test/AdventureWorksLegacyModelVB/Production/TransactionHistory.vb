Namespace AW.Types

	Partial Public Class TransactionHistory
 Implements ITitledObject

		Public Property TransactionID() As Integer

		Public Property ReferenceOrderID() As Integer

		Public Property ReferenceOrderLineID() As Integer

#Region "TransactionDate"
        Public mappedTransactionDate As Date
        Friend myTransactionDate As NODate

        '<MemberOrder(1)>
        Public ReadOnly Property TransactionDate As NODate
            Get
                Return If(myTransactionDate, New NODate(mappedTransactionDate, Function(v) mappedTransactionDate = v))
            End Get
        End Property

        Public Sub AboutTransactionDate(a As FieldAbout, TransactionDate As NODate)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case AboutTypeCodes.Visible
            End Select
        End Sub
#End Region

#Region "TransactionType"
        Public mappedTransactionType As String
        Friend myTransactionType As TextString

        '<MemberOrder(1)>
        Public ReadOnly Property TransactionType As TextString
            Get
                Return If(myTransactionType, New TextString(mappedTransactionType, Function(v) mappedTransactionType = v))
            End Get
        End Property

        Public Sub AboutTransactionType(a As FieldAbout, TransactionType As TextString)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case AboutTypeCodes.Visible
            End Select
        End Sub
#End Region

#Region "Quantity"
        Public mappedQuantity As Integer
        Friend myQuantity As WholeNumber

        '<MemberOrder(1)>
        Public ReadOnly Property Quantity As WholeNumber
            Get
                Return If(myQuantity, New WholeNumber(mappedQuantity, Function(v) mappedQuantity = v))
            End Get
        End Property

        Public Sub AboutQuantity(a As FieldAbout, Quantity As WholeNumber)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case AboutTypeCodes.Visible
            End Select
        End Sub
#End Region

#Region "ActualCost"
        Public mappedActualCost As Decimal
        Friend myActualCost As Money

        '<MemberOrder(1)>
        Public ReadOnly Property ActualCost As Money
            Get
                Return If(myActualCost, New Money(mappedActualCost, Function(v) mappedActualCost = v))
            End Get
        End Property

        Public Sub AboutActualCost(a As FieldAbout, ActualCost As Money)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case AboutTypeCodes.Visible
            End Select
        End Sub
#End Region

        Public Property ProductID() As Integer

        Public Overridable Property Product() As Product

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

        Public Function Title() As ITitle Implements ITitledObject.Title
            Return New Title(ToString())
        End Function

        Public Overrides Function ToString() As String
            Return $"TransactionHistory: {TransactionID}"
        End Function
    End Class
End Namespace