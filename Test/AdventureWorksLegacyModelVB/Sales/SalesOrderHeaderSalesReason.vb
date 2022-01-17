Namespace AW.Types

    '<Named("Reason")>
    Partial Public Class SalesOrderHeaderSalesReason
 Implements ITitledObject
        ''<Hidden>
        Public Property SalesOrderID() As Integer

        ''<Hidden>
        Public Property SalesReasonID() As Integer

        Public Overridable Property SalesOrderHeader() As SalesOrderHeader

        Public Overridable Property SalesReason() As SalesReason

#Region "ModifiedDate"
        Public Property mappedModifiedDate As Date
        Friend myModifiedDate As TimeStamp

        <MemberOrder(99)>
        Public ReadOnly Property ModifiedDate As TimeStamp
            Get
                Return If(myModifiedDate, New TimeStamp(mappedModifiedDate, Function(v) mappedModifiedDate = v))
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
            Return $"SalesOrderHeaderSalesReason: {SalesOrderID}-{SalesReasonID}"
        End Function
    End Class
End Namespace