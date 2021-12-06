Namespace AW.Types

	Partial Public Class ProductModelIllustration
		

		<Hidden>
		Public Property ProductModelID() As Integer

		<Hidden>
		Public Property IllustrationID() As Integer

		Public Overridable Property Illustration() As Illustration

		Public Overridable Property ProductModel() As ProductModel

#Region "ModifiedDate"
        Friend mappedModifiedDate As Date
        Friend myModifiedDate As TimeStamp

        <MemberOrder(1)>
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

        Public Overrides Function ToString() As String
			Return $"ProductModelIllustration: {ProductModelID}-{IllustrationID}"
		End Function
	End Class
End Namespace