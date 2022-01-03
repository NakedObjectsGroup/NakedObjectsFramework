Namespace AW.Types

    '<Bounded>
    Partial Public Class SalesReason
        ''<Hidden>
        Public Property SalesReasonID() As Integer

#Region "Name"
        Public mappedName As String
        Friend myName As TextString

        '<MemberOrder(1)>
        Private ReadOnly Property Name As TextString
            Get
                Return If(myName, New TextString(mappedName, Function(v) mappedName = v))
            End Get
        End Property

        Public Sub AboutName(a As FieldAbout, Name As TextString)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case AboutTypeCodes.Visible
            End Select
        End Sub
#End Region

#Region "ReasonType"
        Public mappedReasonType As String
        Friend myReasonType As TextString

        '<MemberOrder(1)>
        Private ReadOnly Property ReasonType As TextString
            Get
                Return If(myReasonType, New TextString(mappedReasonType, Function(v) mappedReasonType = v))
            End Get
        End Property

        Public Sub AboutReasonType(a As FieldAbout, ReasonType As TextString)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case AboutTypeCodes.Visible
            End Select
        End Sub
#End Region

#Region "ModifiedDate"
        Public mappedModifiedDate As Date
        Friend myModifiedDate As TimeStamp

        '<MemberOrder(99)>
        Private ReadOnly Property ModifiedDate As TimeStamp
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
            Return New Title(Name)
        End Function
    End Class
End Namespace