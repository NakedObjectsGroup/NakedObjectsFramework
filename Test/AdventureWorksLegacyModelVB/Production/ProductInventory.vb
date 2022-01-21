Namespace AW.Types

	Partial Public Class ProductInventory

		Implements ITitledObject

		Public Property ProductID() As Integer

		Public Property LocationID() As Short

#Region "Shelf"
		Public Property mappedShelf As String
		Friend myShelf As TextString

		<MemberOrder(40)>
		Public ReadOnly Property Shelf As TextString
			Get
				myShelf = If(myShelf, New TextString(mappedShelf, Sub(v) mappedShelf = v))
				Return myShelf
			End Get
		End Property

		Public Sub AboutShelf(a As FieldAbout, Shelf As TextString)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case Else
			End Select
		End Sub
#End Region

#Region "Bin"
		Public Property mappedBin As Byte
		Friend myBin As WholeNumber

		<MemberOrder(50)>
		Public ReadOnly Property Bin As WholeNumber
			Get
				myBin = If(myBin, New WholeNumber(mappedBin, Sub(v) mappedBin = CType(v, Byte)))
				Return myBin
			End Get
		End Property

		Public Sub AboutBin(a As FieldAbout, Bin As WholeNumber)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case Else
			End Select
		End Sub
#End Region

#Region "Quantity"
		Public Property mappedQuantity As Short
		Friend myQuantity As WholeNumber

		<MemberOrder(10)>
		Public ReadOnly Property Quantity As WholeNumber
			Get
				myQuantity = If(myQuantity, New WholeNumber(mappedQuantity, Sub(v) mappedQuantity = CType(v, Short)))
				Return myQuantity
			End Get
		End Property

		Public Sub AboutQuantity(a As FieldAbout, Quantity As WholeNumber)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case Else
			End Select
		End Sub
#End Region

		<MemberOrder(30)>
		Public Overridable Property Location() As Location

		<MemberOrder(20)>
		Public Overridable Property Product() As Product

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
			Return $"{Quantity} in {Location} - {Shelf}"
		End Function
	End Class
End Namespace