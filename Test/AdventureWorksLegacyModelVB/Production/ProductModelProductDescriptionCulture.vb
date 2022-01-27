Namespace AW.Types

	Partial Public Class ProductModelProductDescriptionCulture
 Implements ITitledObject, INotEditableOncePersistent

		Public Property ProductModelID() As Integer

		Public Property ProductDescriptionID() As Integer

		Public Property CultureID() As String

		Public Overridable Property Culture() As Culture

		Public Overridable Property ProductDescription() As ProductDescription

		Public Overridable Property ProductModel() As ProductModel

		Public Sub AboutProductModel(a As FieldAbout, pm As ProductModel)
			Select Case a.TypeCode
				Case Else
					a.Visible = False
			End Select
		End Sub

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
			Return $"ProductModelProductDescriptionCulture: {ProductModelID}-{ProductDescriptionID}-{CultureID}"
		End Function
	End Class
End Namespace