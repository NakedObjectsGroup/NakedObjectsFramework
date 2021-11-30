Namespace AW.Types

	Partial Public Class ProductInventory
		Implements IHasRowGuid, IHasModifiedDate

		<Hidden>
		Public Property ProductID() As Integer

		<Hidden>
		Public Property LocationID() As Short

		<MemberOrder(40)>
		Public Property Shelf() As String = ""

		<MemberOrder(50)>
		Public Property Bin() As Byte

		<MemberOrder(10)>
		Public Property Quantity() As Short

		<MemberOrder(30)>
		Public Overridable Property Location() As Location

		<MemberOrder(20)>
		Public Overridable Property Product() As Product

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
			Return $"{Quantity} in {Location} - {Shelf}"
		End Function
	End Class
End Namespace