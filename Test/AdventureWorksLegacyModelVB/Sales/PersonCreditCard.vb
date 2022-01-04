Namespace AW.Types

	Partial Public Class PersonCreditCard
        '<Hidden>
        Public Property PersonID() As Integer

        '<Hidden>
        Public Property CreditCardID() As Integer

        Public Overridable Property Person() As Person

        Public Overridable Property CreditCard() As CreditCard

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
            Return New Title($"PersonCreditCard: {PersonID}-{CreditCardID}")
        End Function
    End Class
End Namespace