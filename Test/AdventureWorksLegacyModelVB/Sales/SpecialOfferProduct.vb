Namespace AW.Types

	Partial Public Class SpecialOfferProduct

		Implements ITitledObject
		''<Hidden>
		Public Property SpecialOfferID() As Integer


		<MemberOrder(1)>
		Public Overridable Property SpecialOffer() As SpecialOffer


		''<Hidden>
		Public Property ProductID() As Integer

		<MemberOrder(2)>
		Public Overridable Property Product() As Product


		''<Hidden>
		Public Property RowGuid() As Guid

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

		Public Sub AboutModifiedDate(a As IFieldAbout)
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
			Return $"SpecialOfferProduct: {SpecialOfferID}-{ProductID}"
		End Function
	End Class
End Namespace