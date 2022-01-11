Namespace AW.Types

	Partial Public Class ProductModelProductDescriptionCulture
 Implements ITitledObject

		Public Property ProductModelID() As Integer

		Public Property ProductDescriptionID() As Integer

		Public Property CultureID() As String

		Public Overridable Property Culture() As Culture

		Public Overridable Property ProductDescription() As ProductDescription

		Public Overridable Property ProductModel() As ProductModel

		Public Sub AboutProductModel(a As FieldAbout, pm As ProductModel)
			Select Case a.TypeCode
				Case AboutTypeCodes.Visible
					a.Visible = False
			End Select
		End Sub

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
			Return $"ProductModelProductDescriptionCulture: {ProductModelID}-{ProductDescriptionID}-{CultureID}"
		End Function
	End Class
End Namespace