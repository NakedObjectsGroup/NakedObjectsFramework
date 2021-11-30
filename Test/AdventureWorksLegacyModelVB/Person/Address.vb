Namespace AW.Types
	Partial Public Class Address
		Implements IHasRowGuid, IHasModifiedDate

		<Hidden>
		Public Property AddressID() As Integer

		<MemberOrder(11)>
		Public Property AddressLine1() As String = ""

		'INSTANT VB WARNING: Nullable reference types have no equivalent in VB:
		'ORIGINAL LINE: public string? AddressLine2 {get;set;}
		<MemberOrder(12)>
		Public Property AddressLine2() As String

		<MemberOrder(13)>
		Public Property City() As String = ""

		<MemberOrder(14)>
		Public Property PostalCode() As String = ""

		<Hidden>
		Public Property StateProvinceID() As Integer

		<MemberOrder(15)>
		Public Overridable Property StateProvince() As StateProvince
#Region "ModifiedDate"
		Friend mappedModifiedDate As Date
		Friend myModifiedDate As TimeStamp

		<MemberOrder(1)>
		Public ReadOnly Property ModifiedDate As TimeStamp Implements IHasModifiedDate.ModifiedDate
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
		<Hidden>
		Public Property rowguid() As Guid Implements IHasRowGuid.rowguid

		Public Overrides Function ToString() As String
			Return $"{AddressLine1}..."
		End Function


	End Class
End Namespace