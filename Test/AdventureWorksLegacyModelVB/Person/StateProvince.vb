Namespace AW.Types


	Partial Public Class StateProvince
		Implements ITitledObject, IBounded

		Public Property StateProvinceID() As Integer

#Region "StateProvinceCode"
		Public Property mappedStateProvinceCode As String
		Friend myStateProvinceCode As TextString

		<MemberOrder(1)>
		Public ReadOnly Property StateProvinceCode As TextString
			Get
				myStateProvinceCode = If(myStateProvinceCode, New TextString(mappedStateProvinceCode, Sub(v) mappedStateProvinceCode = v))
Return myStateProvinceCode
			End Get
		End Property

		Public Sub AboutStateProvinceCode(a As FieldAbout, StateProvinceCode As TextString)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region

#Region "IsOnlyStateProvinceFlag"
		Public Property mappedIsOnlyStateProvinceFlag As Boolean
		Friend myIsOnlyStateProvinceFlag As Logical

		<MemberOrder(1)>
		Public ReadOnly Property IsOnlyStateProvinceFlag As Logical
			Get
				myIsOnlyStateProvinceFlag = If(myIsOnlyStateProvinceFlag, New Logical(mappedIsOnlyStateProvinceFlag, Sub(v) mappedIsOnlyStateProvinceFlag = v))
Return myIsOnlyStateProvinceFlag
			End Get
		End Property

		Public Sub AboutIsOnlyStateProvinceFlag(a As FieldAbout, IsOnlyStateProvinceFlag As Logical)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region

#Region "Name"
		Public Property mappedName As String
		Friend myName As TextString

		<MemberOrder(1)>
		Public ReadOnly Property Name As TextString
			Get
				myName = If(myName, New TextString(mappedName, Sub(v) mappedName = v))
Return myName
			End Get
		End Property

		Public Sub AboutName(a As FieldAbout, Name As TextString)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region

		Public Property CountryRegionCode() As String 'Hidden

		Public Overridable Property CountryRegion() As CountryRegion

		Public Property TerritoryID() As Integer

		Public Overridable Property SalesTerritory() As SalesTerritory

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

		Public Property RowGuid() As Guid

		Public Function Title() As Title Implements ITitledObject.Title
			Return New Title(ToString())
		End Function

		Public Overrides Function ToString() As String
			Return mappedName
		End Function
	End Class
End Namespace