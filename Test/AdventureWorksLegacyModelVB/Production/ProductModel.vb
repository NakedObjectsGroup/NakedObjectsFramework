

Namespace AW.Types

	Partial Public Class ProductModel

		<Hidden>
		Public Property ProductModelID() As Integer

		<MemberOrder(10)>
		Public Property Name() As String = ""

		'INSTANT VB WARNING: Nullable reference types have no equivalent in VB:
		'ORIGINAL LINE: public string? CatalogDescription {get;set;}
		<Hidden>
		Public Property CatalogDescription() As String

		<Named("CatalogDescription"), MemberOrder(20), MultiLine(10)>
		Public ReadOnly Property FormattedCatalogDescription() As String
			Get
				Return Nothing 'ProductModel_Functions.CatalogDescription(Me)
			End Get
		End Property

		<MemberOrder(22)>
		Public ReadOnly Property LocalCultureDescription() As ProductDescription
			Get
				Return Nothing 'ProductModel_Functions.LocalCultureDescription(Me)
			End Get
		End Property

		'INSTANT VB WARNING: Nullable reference types have no equivalent in VB:
		'ORIGINAL LINE: public string? Instructions {get;set;}
		<MemberOrder(30)>
		Public Property Instructions() As String

		<TableView(True, "Name", "Number", "Color", "ProductInventory")>
		Public Overridable Property ProductVariants() As ICollection(Of Product) = New List(Of Product)()

		<Hidden>
		Public Overridable Property ProductModelIllustration() As ICollection(Of ProductModelIllustration) = New List(Of ProductModelIllustration)()

		<Hidden>
		Public Overridable Property ProductModelProductDescriptionCulture() As ICollection(Of ProductModelProductDescriptionCulture) = New List(Of ProductModelProductDescriptionCulture)()

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

		<Hidden>
		Public Property RowGuid() As Guid

		Public Overrides Function ToString() As String
			Return Name
		End Function
	End Class
End Namespace