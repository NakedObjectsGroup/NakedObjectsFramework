﻿Namespace AW.Types

    '<Named("Reason")>
    Partial Public Class SalesOrderHeaderSalesReason
 Implements ITitledObject, INotEditableOncePersistent
        ''<Hidden>
        Public Property SalesOrderID() As Integer

        ''<Hidden>
        Public Property SalesReasonID() As Integer

        Public Overridable Property SalesOrderHeader() As SalesOrderHeader

        Public Overridable Property SalesReason() As SalesReason

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
            Return $"SalesOrderHeaderSalesReason: {SalesOrderID}-{SalesReasonID}"
        End Function
    End Class
End Namespace