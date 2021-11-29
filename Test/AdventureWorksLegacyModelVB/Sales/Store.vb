

Namespace AW.Types

	Partial Public Class Store
		Inherits BusinessEntity
		Implements IBusinessEntityWithContacts, IHasModifiedDate

		Public Sub New()
		End Sub

		Public Sub New(ByVal cloneFrom As Store)
			MyBase.New(cloneFrom)
			Name = cloneFrom.Name
			Demographics = cloneFrom.Demographics
			SalesPersonID = cloneFrom.SalesPersonID
			SalesPerson = cloneFrom.SalesPerson
			ModifiedDate = cloneFrom.ModifiedDate
			rowguid = cloneFrom.rowguid
		End Sub

		<Named("Store Name")>
		<MemberOrder(20)>
		Public Property Name() As String = ""

		'INSTANT VB WARNING: Nullable reference types have no equivalent in VB:
		'ORIGINAL LINE: public string? Demographics {get;set;}
		<Hidden>
		Public Property Demographics() As String

		<Named("Demographics")>
		<MemberOrder(30)>
		<MultiLine(10)>
		Public ReadOnly Property FormattedDemographics() As String
			Get
				Return Utilities.FormatXML(Demographics)
			End Get
		End Property

		<Hidden>
		Public Property SalesPersonID() As Integer?

		'INSTANT VB WARNING: Nullable reference types have no equivalent in VB:
		'ORIGINAL LINE: public virtual SalesPerson? SalesPerson {get;set;}
		<MemberOrder(40)>
		Public Overridable Property SalesPerson() As SalesPerson

		<MemberOrder(99)>
		Public Property ModifiedDate() As DateTime Implements IHasModifiedDate.ModifiedDate

		<Hidden>
		Public Property rowguid() As Guid

		Public Overrides Function ToString() As String
			Return Name
		End Function
	End Class
End Namespace