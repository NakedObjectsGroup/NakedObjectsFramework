Namespace AW.Types

	Partial Public Class PersonCreditCard
 Implements ITitledObject, INotEditableOncePersistent
        '<Hidden>
        Public Property PersonID() As Integer

        '<Hidden>
        Public Property CreditCardID() As Integer

        Public Overridable Property Person() As Person

        Public Overridable Property CreditCard() As CreditCard

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
            Return $"PersonCreditCard: {PersonID}-{CreditCardID}"
        End Function
    End Class
End Namespace