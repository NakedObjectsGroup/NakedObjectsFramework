﻿Namespace AW.Types

	Partial Public Class ProductModelIllustration
 Implements ITitledObject, INotEditableOncePersistent

        Public Property ProductModelID() As Integer

        Public Property IllustrationID() As Integer

		Public Overridable Property Illustration() As Illustration

		Public Overridable Property ProductModel() As ProductModel

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
            Return $"ProductModelIllustration: {ProductModelID}-{IllustrationID}"
        End Function
    End Class
End Namespace