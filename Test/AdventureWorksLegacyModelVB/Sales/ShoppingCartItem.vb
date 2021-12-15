Namespace AW.Types

	Partial Public Class ShoppingCartItem
		'<Hidden>
		Public Property ShoppingCartItemID() As Integer

		'<Hidden>
		Public Property ShoppingCartID() As String = ""

#Region "Quantity"
		Friend mappedQuantity As Integer
		Friend myQuantity As WholeNumber

		<MemberOrder(20)>
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

		'<Hidden>
		Public Property ProductID() As Integer

		<MemberOrder(10)>
		Public Overridable Property Product() As Product

		'<Hidden>
		Public Property DateCreated() As DateTime

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

		Public Function Title() As Title
			Return New Title($"{Quantity}  x {Product}")
		End Function
	End Class
End Namespace