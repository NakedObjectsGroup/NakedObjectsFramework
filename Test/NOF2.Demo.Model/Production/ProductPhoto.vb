﻿Imports NakedFramework.Value

Namespace AW.Types

	Partial Public Class ProductPhoto

		Implements ITitledObject, INotEditableOncePersistent

		Public Property ProductPhotoID As Integer

		Public Property ThumbNailPhoto As Byte()

#Region "ThumbnailPhotoFileName"
		Public Property mappedThumbnailPhotoFileName As String
		Friend myThumbnailPhotoFileName As TextString

		<DemoProperty(Order:=1)>
		Public ReadOnly Property ThumbnailPhotoFileName As TextString
			Get
				myThumbnailPhotoFileName = If(myThumbnailPhotoFileName, New TextString(mappedThumbnailPhotoFileName, Sub(v) mappedThumbnailPhotoFileName = v))
Return myThumbnailPhotoFileName
			End Get
		End Property

		Public Sub AboutThumbnailPhotoFileName(a As FieldAbout, ThumbnailPhotoFileName As TextString)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case Else
			End Select
		End Sub
#End Region

		Public Property LargePhoto As Byte()

#Region "LargePhotoFileName"
		Public Property mappedLargePhotoFileName As String
		Friend myLargePhotoFileName As TextString

		<DemoProperty(Order:=1)>
		Public ReadOnly Property LargePhotoFileName As TextString
			Get
				myLargePhotoFileName = If(myLargePhotoFileName, New TextString(mappedLargePhotoFileName, Sub(v) mappedLargePhotoFileName = v))
Return myLargePhotoFileName
			End Get
		End Property

		Public Sub AboutLargePhotoFileName(a As FieldAbout, LargePhotoFileName As TextString)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case Else
			End Select
		End Sub
#End Region

		''<Hidden>
		Public Overridable Property ProductProductPhoto As ICollection(Of ProductProductPhoto) = New List(Of ProductProductPhoto)()

#Region "ModifiedDate"
		Public Property mappedModifiedDate As Date
		Friend myModifiedDate As TimeStamp

		<DemoProperty(Order:=99)>
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
			Return $"Product Photo: {ProductPhotoID}"
		End Function
	End Class
End Namespace