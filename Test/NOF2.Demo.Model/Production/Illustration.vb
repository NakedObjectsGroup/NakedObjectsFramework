Namespace AW.Types

	Partial Public Class Illustration
 Implements ITitledObject, INotEditableOncePersistent
		Public Property IllustrationID() As Integer

#Region "Diagram"
        Public Property mappedDiagram As String
        Friend myDiagram As TextString

        <DemoProperty(Order:=1)>
        Public ReadOnly Property Diagram As TextString
            Get
                myDiagram = If(myDiagram, New TextString(mappedDiagram, Sub(v) mappedDiagram = v))
Return myDiagram
            End Get
        End Property

        Public Sub AboutDiagram(a As FieldAbout, Diagram As TextString)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case Else
            End Select
        End Sub
#End Region

#Region "ProductModelIllustration (Collection)"
        Public Overridable Property mappedProductModelIllustration As ICollection(Of ProductModelIllustration) = New List(Of ProductModelIllustration)()

        Private myProductModelIllustration As InternalCollection

        <DemoProperty(Order:=1)>
        Public ReadOnly Property ProductModelIllustration As InternalCollection
            Get
                myProductModelIllustration = If(myProductModelIllustration, New InternalCollection(Of ProductModelIllustration)(mappedProductModelIllustration))
Return myProductModelIllustration
            End Get
        End Property

        Public Sub AboutProductModelIllustration(a As FieldAbout)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
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

        Public Function Title() As Title Implements ITitledObject.Title
            Return New Title(ToString())
        End Function

        Public Overrides Function ToString() As String
            Return $"Illustration: {IllustrationID}"
        End Function
    End Class
End Namespace