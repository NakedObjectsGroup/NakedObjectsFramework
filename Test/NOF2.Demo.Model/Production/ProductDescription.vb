
Namespace AW.Types

    Partial Public Class ProductDescription

        Implements ITitledObject, INotEditableOncePersistent

        Public Property ProductDescriptionID() As Integer

#Region "Description"
        Public Property mappedDescription As String
        Friend myDescription As MultiLineTextString

        <DemoProperty(Order:=2)>
        Public ReadOnly Property Description As TextString
            Get
                myDescription = If(myDescription, New MultiLineTextString(mappedDescription, Sub(v) mappedDescription = v))
Return myDescription
            End Get
        End Property

        Public Sub AboutDescription(a As FieldAbout, Description As MultiLineTextString)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case Else
            End Select
        End Sub
#End Region

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

        Public Property RowGuid() As Guid

        Public Function Title() As Title Implements ITitledObject.Title
            Return New Title(ToString())
        End Function

        Public Overrides Function ToString() As String
            Return mappedDescription
        End Function
    End Class
End Namespace