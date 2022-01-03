Namespace AW.Types

	Partial Public Class ProductProductPhoto
		Public Property ProductID() As Integer

		Public Property ProductPhotoID() As Integer

#Region "Primary"
        Public mappedPrimary As Boolean
        Friend myPrimary As Logical

        '<MemberOrder(1)>
        Private ReadOnly Property Primary As Logical
            Get
                Return If(myPrimary, New Logical(mappedPrimary, Function(v) mappedPrimary = v))
            End Get
        End Property

        Public Sub AboutPrimary(a As FieldAbout, Primary As Logical)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case AboutTypeCodes.Visible
            End Select
        End Sub
#End Region

        Public Overridable Property Product() As Product

        Public Overridable Property ProductPhoto() As ProductPhoto

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
            Return New Title($"ProductProductPhoto: {ProductID}-{ProductPhotoID}")
        End Function
    End Class
End Namespace