Namespace AW.Types


	Partial Public Class ProductCategory
		Implements ITitledObject, IBounded

		Public Property ProductCategoryID() As Integer

#Region "Name"
		Public Property mappedName As String
		Friend myName As TextString

		<MemberOrder(1)>
		Public ReadOnly Property Name As TextString
			Get
				myName = If(myName, New TextString(mappedName, Sub(v) mappedName = v))
Return myName
			End Get
		End Property

		Public Sub AboutName(a As FieldAbout, Name As TextString)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case Else
			End Select
		End Sub
#End Region

#Region "ProductSubcategory (Collection)"
		Public Overridable Property mappedProductSubcategory As ICollection(Of ProductSubcategory) = New List(Of ProductSubcategory)()

		Private myProductSubcategory As InternalCollection

		''<TableView(True)>
		Public ReadOnly Property ProductSubcategory As InternalCollection
			Get
				myProductSubcategory = If(myProductSubcategory, New InternalCollection(Of ProductSubcategory)(mappedProductSubcategory))
Return myProductSubcategory
			End Get
		End Property

		Public Sub AboutProductSubcategory(a As FieldAbout)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
					a.Name = "Subcategories"
				Case Else
			End Select
		End Sub
#End Region

#Region "ModifiedDate"
		Public Property mappedModifiedDate As Date
		Friend myModifiedDate As TimeStamp

		<MemberOrder(99)>
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

		Public Property RowGuid() As Guid

		Public Function Title() As Title Implements ITitledObject.Title
			Return New Title(ToString())
		End Function

		Public Overrides Function ToString() As String
			Return mappedName
		End Function
	End Class
End Namespace