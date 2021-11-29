Namespace AW.Types
	Partial Public Class Address
		Implements IHasRowGuid, IHasModifiedDate

		Public Sub New()
		End Sub

		Public Sub New(ByVal cloneFrom As Address)
			AddressID = cloneFrom.AddressID
			AddressLine1 = cloneFrom.AddressLine1
			AddressLine2 = cloneFrom.AddressLine2
			City = cloneFrom.City
			PostalCode = cloneFrom.PostalCode
			StateProvinceID = cloneFrom.StateProvinceID
			StateProvince = cloneFrom.StateProvince
			ModifiedDate = cloneFrom.ModifiedDate
			rowguid = cloneFrom.rowguid
		End Sub

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

		<MemberOrder(99)>
		Public Property ModifiedDate() As DateTime Implements IHasModifiedDate.ModifiedDate

		<Hidden>
		Public Property rowguid() As Guid Implements IHasRowGuid.rowguid

		Public Overrides Function ToString() As String
			Return $"{AddressLine1}..."
		End Function


	End Class
End Namespace