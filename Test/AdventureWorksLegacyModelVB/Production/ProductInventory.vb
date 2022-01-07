Namespace AW.Types

	Partial Public Class ProductInventory
 Implements ITitledObject

		Public Property ProductID() As Integer

		Public Property LocationID() As Short

#Region "Shelf"
		Public mappedShelf As String
		Friend myShelf As TextString

		'<MemberOrder(40)>
		Public ReadOnly Property Shelf As TextString
			Get
				Return If(myShelf, New TextString(mappedShelf, Function(v) mappedShelf = v))
			End Get
		End Property

		Public Sub AboutShelf(a As FieldAbout, Shelf As TextString)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region

#Region "Bin"
		Public mappedBin As Byte
		Friend myBin As WholeNumber

		'<MemberOrder(50)>
		Public ReadOnly Property Bin As WholeNumber
			Get
				Return If(myBin, New WholeNumber(mappedBin, Function(v) mappedBin = v))
			End Get
		End Property

		Public Sub AboutBin(a As FieldAbout, Bin As WholeNumber)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region

#Region "Quantity"
		Public mappedQuantity As Short
		Friend myQuantity As WholeNumber

		'<MemberOrder(10)>
		Public ReadOnly Property Quantity As WholeNumber
			Get
				Return If(myQuantity, New WholeNumber(mappedQuantity, Function(v) mappedQuantity = v))
			End Get
		End Property

		Public Sub AboutQuantity(a As FieldAbout, Quantity As WholeNumber)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region

		'<MemberOrder(30)>
		Public Overridable Property Location() As Location

		'<MemberOrder(20)>
		Public Overridable Property Product() As Product

#Region "ModifiedDate"
		Public mappedModifiedDate As Date
		Friend myModifiedDate As TimeStamp

		'<MemberOrder(99)>
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

		Public Function Title() As ITitle Implements ITitledObject.Title
			Return New Title(ToString())
		End Function

		Public Overrides Function ToString() As String
			Return $"{Quantity} in {Location} - {Shelf}"
		End Function
	End Class
End Namespace