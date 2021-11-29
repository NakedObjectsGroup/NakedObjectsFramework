Imports NakedFramework.Value

Namespace AW.Types

	Partial Public Class ProductPhoto

		Public Sub New()
		End Sub

		<Hidden>
		Public Property ProductPhotoID() As Integer

		Public Property ThumbNailPhoto() As Byte()

		'INSTANT VB WARNING: Nullable reference types have no equivalent in VB:
		'ORIGINAL LINE: public string? ThumbnailPhotoFileName {get;set;}
		Public Property ThumbnailPhotoFileName() As String

		Public Property LargePhoto() As Byte()

		'public string? LargePhotoFileName {get;set;}
		Public Property LargePhotoFileName() As String

		Public Overridable ReadOnly Property LargePhotoAsAttachment() As FileAttachment
			Get
				Return Nothing
			End Get
		End Property
		'DispositionType = "inline" ' fake mimetype

		<Hidden>
		Public Property ProductProductPhoto As ICollection(Of ProductProductPhoto) = New List(Of ProductProductPhoto)()

		<MemberOrder(99)>
		Public Property ModifiedDate As DateTime

		'INSTANT VB TODO TASK: Local functions are not converted by Instant VB:
		'		public override string ToString()
		'		{
		'			Return string.Format("Product Photo: {0}", ProductPhotoID, TangibleStringInterpolationMarker);
		'		}
	End Class
End Namespace