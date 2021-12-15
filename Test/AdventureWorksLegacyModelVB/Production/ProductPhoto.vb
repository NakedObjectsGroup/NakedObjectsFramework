Imports NakedFramework.Value

Namespace AW.Types

	Partial Public Class ProductPhoto

		Public Property ProductPhotoID As Integer

		Public Property ThumbNailPhoto As Byte()

#Region "ThumbnailPhotoFileName"
		Friend mappedThumbnailPhotoFileName As String
		Friend myThumbnailPhotoFileName As TextString

		'<MemberOrder(1)>
		Public ReadOnly Property ThumbnailPhotoFileName As TextString
			Get
				Return If(myThumbnailPhotoFileName, New TextString(mappedThumbnailPhotoFileName, Function(v) mappedThumbnailPhotoFileName = v))
			End Get
		End Property

		Public Sub AboutThumbnailPhotoFileName(a As FieldAbout, ThumbnailPhotoFileName As TextString)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region

		Public Property LargePhoto As Byte()

#Region "LargePhotoFileName"
		Friend mappedLargePhotoFileName As String
		Friend myLargePhotoFileName As TextString

		'<MemberOrder(1)>
		Public ReadOnly Property LargePhotoFileName As TextString
			Get
				Return If(myLargePhotoFileName, New TextString(mappedLargePhotoFileName, Function(v) mappedLargePhotoFileName = v))
			End Get
		End Property

		Public Sub AboutLargePhotoFileName(a As FieldAbout, LargePhotoFileName As TextString)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region

		Public Overridable ReadOnly Property LargePhotoAsAttachment() As FileAttachment
			Get
				Return Nothing
			End Get
		End Property
		'DispositionType = "inline" ' fake mimetype

		''<Hidden>
		Public Property ProductProductPhoto As ICollection(Of ProductProductPhoto) = New List(Of ProductProductPhoto)()

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
			Return New Title("Product Photo: {ProductPhotoID}")
		End Function
	End Class
End Namespace