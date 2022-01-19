Namespace AW.Types

	Partial Public Class ShoppingCartItem

		Implements ITitledObject
		''<Hidden>
		Public Property ShoppingCartItemID() As Integer

		''<Hidden>
		Public Property ShoppingCartID() As String = ""

#Region "Quantity"
		Public Property mappedQuantity As Integer
		Friend myQuantity As WholeNumber

		<MemberOrder(20)>
		Public ReadOnly Property Quantity As WholeNumber
			Get
				myQuantity = If(myQuantity, New WholeNumber(mappedQuantity, Sub(v) mappedQuantity = v))
Return myQuantity
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

		''<Hidden>
		Public Property ProductID() As Integer

		<MemberOrder(10)>
		Public Overridable Property Product() As Product

		''<Hidden>
		Public Property DateCreated() As DateTime

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

		Public Function Title() As Title Implements ITitledObject.Title
			Return New Title(ToString())
		End Function

		Public Overrides Function ToString() As String
			Return $"{Quantity}  x {Product}"
		End Function
	End Class
End Namespace