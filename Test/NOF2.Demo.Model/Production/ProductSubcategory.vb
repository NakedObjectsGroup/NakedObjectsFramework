Namespace AW.Types


	Partial Public Class ProductSubcategory
		Implements ITitledObject, INotEditableOncePersistent, IBounded

		Public Property ProductSubcategoryID() As Integer

#Region "Name"
		Public Property mappedName As String
		Friend myName As TextString

		<DemoProperty(Order:=1)>
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

		Public Property ProductCategoryID() As Integer

		Public Overridable Property ProductCategory() As ProductCategory

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

		Public Property RowGuid() As Guid

		Public Function Title() As Title Implements ITitledObject.Title
			Return New Title(ToString())
		End Function

		Public Overrides Function ToString() As String
			Return mappedName
		End Function
	End Class
End Namespace