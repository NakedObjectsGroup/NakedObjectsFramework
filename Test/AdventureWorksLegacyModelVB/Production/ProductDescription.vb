
Namespace AW.Types

    Partial Public Class ProductDescription

        Implements ITitledObject

        Public Property ProductDescriptionID() As Integer

#Region "Description"
        Public mappedDescription As String
        Friend myDescription As MultiLineTextString

        '<MemberOrder(2)>
        Public ReadOnly Property Description As TextString
            Get
                Return If(myDescription, New MultiLineTextString(mappedDescription, Function(v) mappedDescription = v))
            End Get
        End Property

        Public Sub AboutDescription(a As FieldAbout, Description As MultiLineTextString)
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

        Public Property RowGuid() As Guid

        Public Function Title() As Title Implements ITitledObject.Title
            Return New Title(ToString())
        End Function

        Public Overrides Function ToString() As String
            Return mappedDescription
        End Function
    End Class
End Namespace