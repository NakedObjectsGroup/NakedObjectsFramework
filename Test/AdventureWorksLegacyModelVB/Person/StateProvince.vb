Namespace AW.Types


	Partial Public Class StateProvince
		Implements ITitledObject, IBounded

		Public Property StateProvinceID() As Integer

#Region "StateProvinceCode"
		Public mappedStateProvinceCode As String
		Friend myStateProvinceCode As TextString

		<MemberOrder(1)>
		Public ReadOnly Property StateProvinceCode As TextString
			Get
				Return If(myStateProvinceCode, New TextString(mappedStateProvinceCode, Function(v) mappedStateProvinceCode = v))
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
		Public mappedIsOnlyStateProvinceFlag As Boolean
		Friend myIsOnlyStateProvinceFlag As Logical

		<MemberOrder(1)>
		Public ReadOnly Property IsOnlyStateProvinceFlag As Logical
			Get
				Return If(myIsOnlyStateProvinceFlag, New Logical(mappedIsOnlyStateProvinceFlag, Function(v) mappedIsOnlyStateProvinceFlag = v))
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
		Public mappedName As String
		Friend myName As TextString

		<MemberOrder(1)>
		Public ReadOnly Property Name As TextString
			Get
				Return If(myName, New TextString(mappedName, Function(v) mappedName = v))
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
		Public mappedModifiedDate As Date
		Friend myModifiedDate As TimeStamp

		<MemberOrder(99)>
		Public ReadOnly Property ModifiedDate As TimeStamp
			Get
				Return If(myModifiedDate, New TimeStamp(mappedModifiedDate, Function(v) mappedModifiedDate = v))
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

		Public Function Title() As ITitle Implements ITitledObject.Title
			Return New Title(ToString())
		End Function

		Public Overrides Function ToString() As String
			Return mappedName
		End Function
	End Class
End Namespace