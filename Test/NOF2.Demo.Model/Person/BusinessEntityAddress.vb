﻿Namespace AW.Types

	'<Named("Address")>
	Partial Public Class BusinessEntityAddress

		Implements ITitledObject, INotEditableOncePersistent

		Public Property BusinessEntityID() As Integer

		<DemoProperty(Order:=3)>
		Public Overridable Property BusinessEntity() As BusinessEntity

		Public Property AddressTypeID() As Integer

		<DemoProperty(Order:=1)>
		Public Overridable Property AddressType() As AddressType

		'<Hidden>
		Public Property AddressID() As Integer

		<DemoProperty(Order:=2)>
		Public Overridable Property Address() As Address

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

		Public Property RowGuid() As Guid

		Public Function Title() As Title Implements ITitledObject.Title
			Return New Title(ToString())
		End Function

		Public Overrides Function ToString() As String
			Return $"{AddressType}: {Address}"
		End Function
	End Class
End Namespace