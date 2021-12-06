Namespace AW.Types

    <Bounded>
    Partial Public Class AddressType

        <Hidden>
        Public Property AddressTypeID() As Integer

        <Hidden>
        Public Property Name() As String = ""

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
        <Hidden>
        Public Property RowGuid() As Guid

        Public Overrides Function ToString() As String
            Return Name
        End Function
    End Class
End Namespace