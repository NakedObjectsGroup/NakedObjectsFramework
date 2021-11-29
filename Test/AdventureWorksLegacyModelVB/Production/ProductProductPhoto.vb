Namespace AW.Types

	Partial Public Class ProductProductPhoto
		Public Property ProductID() As Integer

		Public Property ProductPhotoID() As Integer

		Public Property Primary() As Boolean

		Public Overridable Property Product() As Product

		Public Overridable Property ProductPhoto() As ProductPhoto

		<MemberOrder(99)>
		Public Property ModifiedDate() As DateTime

		Public Overrides Function ToString() As String
			Return $"ProductProductPhoto: {ProductID}-{ProductPhotoID}"
		End Function
	End Class
End Namespace