Namespace AW.Types

	Partial Public Class ProductModelIllustration
 Implements ITitledObject

        Public Property ProductModelID() As Integer

        Public Property IllustrationID() As Integer

		Public Overridable Property Illustration() As Illustration

		Public Overridable Property ProductModel() As ProductModel

#Region "ModifiedDate"
        Public mappedModifiedDate As Date
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

        Public Function Title() As ITitle Implements ITitledObject.Title
            Return New Title(ToString())
        End Function

        Public Overrides Function ToString() As String
            Return $"ProductModelIllustration: {ProductModelID}-{IllustrationID}"
        End Function
    End Class
End Namespace