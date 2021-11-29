Namespace AW.Types

	Partial Public Class ProductModelIllustration
		Implements IHasModifiedDate

		<Hidden>
		Public Property ProductModelID() As Integer

		<Hidden>
		Public Property IllustrationID() As Integer

		Public Overridable Property Illustration() As Illustration

		Public Overridable Property ProductModel() As ProductModel

		<MemberOrder(99)>
		Public Property ModifiedDate() As DateTime Implements IHasModifiedDate.ModifiedDate

		Public Overrides Function ToString() As String
			Return $"ProductModelIllustration: {ProductModelID}-{IllustrationID}"
		End Function
	End Class
End Namespace