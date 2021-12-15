Namespace AW.Types

	Partial Public Class SpecialOfferProduct
		''<Hidden>
		Public Property SpecialOfferID() As Integer


		'<MemberOrder(1)>
		Public Overridable Property SpecialOffer() As SpecialOffer


		''<Hidden>
		Public Property ProductID() As Integer

		'<MemberOrder(2)>
		Public Overridable Property Product() As Product


		''<Hidden>
		Public Property rowguid() As Guid

#Region "ModifiedDate"
		Friend mappedModifiedDate As Date
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

		Public Function Title() As Title
			Return New Title($"SpecialOfferProduct: {SpecialOfferID}-{ProductID}")
		End Function
	End Class
End Namespace