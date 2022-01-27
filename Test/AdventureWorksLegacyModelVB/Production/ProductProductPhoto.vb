Namespace AW.Types

	Partial Public Class ProductProductPhoto
 Implements ITitledObject, INotEditableOncePersistent
		Public Property ProductID() As Integer

		Public Property ProductPhotoID() As Integer

#Region "Primary"
        Public Property mappedPrimary As Boolean
        Friend myPrimary As Logical

        <AWProperty(Order:=1)>
        Public ReadOnly Property Primary As Logical
            Get
                myPrimary = If(myPrimary, New Logical(mappedPrimary, Sub(v) mappedPrimary = v))
Return myPrimary
            End Get
        End Property

        Public Sub AboutPrimary(a As FieldAbout, Primary As Logical)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case Else
            End Select
        End Sub
#End Region

        Public Overridable Property Product() As Product

        Public Overridable Property ProductPhoto() As ProductPhoto

#Region "ModifiedDate"
		Public Property mappedModifiedDate As Date
		Friend myModifiedDate As TimeStamp

		<AWProperty(Order:=99)>
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
            Return $"ProductProductPhoto: {ProductID}-{ProductPhotoID}"
        End Function
    End Class
End Namespace