﻿Namespace AW.Types

	<Bounded>
	Partial Public Class ProductCategory

		Public Property ProductCategoryID() As Integer

#Region "Name"
		Friend mappedName As String
		Friend myName As TextString

		<MemberOrder(1)>
		Public ReadOnly Property Name As TextString
			Get
				Return If(myName, New TextString(mappedName, Function(v) mappedName = v))
			End Get
		End Property

		Public Sub AboutName(a As FieldAbout, Name As TextString)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region

#Region "ProductSubcategory (Collection)"
		Public Overridable Property mappedProductSubcategory As ICollection(Of ProductSubcategory) = New List(Of ProductSubcategory)()

		Private myProductSubcategory As InternalCollection

		'<TableView(True)>
		Public ReadOnly Property ProductSubcategory As InternalCollection
			Get
				Return If(myProductSubcategory, New InternalCollection(Of ProductSubcategory)(mappedProductSubcategory))
			End Get
		End Property

		Public Sub AboutProductSubcategory(a As FieldAbout)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
					a.Name = "Subcategories"
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region

#Region "ModifiedDate"
		Friend mappedModifiedDate As Date
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

		Public Property RowGuid() As Guid

		Public Function Title() As Title
			Return New Title(Name)
		End Function
	End Class
End Namespace