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

        Public Function Title() As Title Implements ITitledObject.Title
            Return New Title($"SalesOrderHeaderSalesReason: {SalesOrderID}-{SalesReasonID}")
        End Function
    End Class
End Namespace