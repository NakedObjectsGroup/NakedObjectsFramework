Namespace AW.Types

	<Bounded>
	Partial Public Class ProductSubcategory
		Implements IHasRowGuid, IHasModifiedDate

		<Hidden>
		Public Property ProductSubcategoryID() As Integer

		Public Property Name() As String = ""

		<Hidden>
		Public Property ProductCategoryID() As Integer

		Public Overridable Property ProductCategory() As ProductCategory

#Region "ModifiedDate"
		Friend mappedModifiedDate As Date
		Friend myModifiedDate As TimeStamp

		<MemberOrder(1)>
		Public ReadOnly Property ModifiedDate As TimeStamp Implements IHasModifiedDate.ModifiedDate
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
		Public Property rowguid() As Guid Implements IHasRowGuid.rowguid

		Public Overrides Function ToString() As String
			Return Name
		End Function
	End Class
End Namespace