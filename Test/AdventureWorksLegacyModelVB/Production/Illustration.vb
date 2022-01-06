Namespace AW.Types

	Partial Public Class Illustration
 Implements ITitledObject
		Public Property IllustrationID() As Integer

#Region "Diagram"
        Public mappedDiagram As String
        Friend myDiagram As TextString

        '<MemberOrder(1)>
        Public ReadOnly Property Diagram As TextString
            Get
                Return If(myDiagram, New TextString(mappedDiagram, Function(v) mappedDiagram = v))
            End Get
        End Property

        Public Sub AboutDiagram(a As FieldAbout, Diagram As TextString)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case AboutTypeCodes.Visible
            End Select
        End Sub
#End Region

#Region "ProductModelIllustration (Collection)"
        Public Overridable Property mappedProductModelIllustration As ICollection(Of ProductModelIllustration) = New List(Of ProductModelIllustration)()

        Private myProductModelIllustration As InternalCollection

        '<MemberOrder(1)>
        Public ReadOnly Property ProductModelIllustration As InternalCollection
            Get
                Return If(myProductModelIllustration, New InternalCollection(Of ProductModelIllustration)(mappedProductModelIllustration))
            End Get
        End Property

        Public Sub AboutProductModelIllustration(a As FieldAbout)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
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

        Public Function Title() As Title Implements ITitledObject.Title
            Return New Title(ToString())
        End Function

        Public Overrides Function ToString() As String
            Return $"Illustration: {IllustrationID}"
        End Function
    End Class
End Namespace