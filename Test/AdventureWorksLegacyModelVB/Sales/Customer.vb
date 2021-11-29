

Namespace AW.Types

	Partial Public Class Customer
		<Hidden>
		Public Property CustomerID() As Integer

		<MemberOrder(15)>
		Public ReadOnly Property CustomerType() As String
			Get
				Return Nothing 'If(Me.IsIndividual(), "Individual", "Store")
			End Get
		End Property

		<DescribedAs("xxx")>
		<MemberOrder(10)>
		Public Property AccountNumber() As String = ""

		<Hidden>
		Public Property CustomerModifiedDate() As DateTime

		<Hidden>
		Public Property CustomerRowguid() As Guid

		Public Overrides Function ToString() As String
			Return $"{AccountNumber} {(If(Store Is Nothing, Person, Store))}"
		End Function

#Region "Store & Personal customers"

		<Hidden>
		Public Property StoreID() As Integer?

		'INSTANT VB WARNING: Nullable reference types have no equivalent in VB:
		'ORIGINAL LINE: public virtual Store? Store {get;set;}
		<MemberOrder(20)>
		Public Overridable Property Store() As Store

		<Hidden>
		Public Property PersonID() As Integer?

		'INSTANT VB WARNING: Nullable reference types have no equivalent in VB:
		'ORIGINAL LINE: public virtual Person? Person {get;set;}
		<MemberOrder(20)>
		Public Overridable Property Person() As Person

#End Region

#Region "Sales Territory"

		<Hidden>
		Public Property SalesTerritoryID() As Integer?

		'INSTANT VB WARNING: Nullable reference types have no equivalent in VB:
		'ORIGINAL LINE: public virtual SalesTerritory? SalesTerritory {get;set;}
		<MemberOrder(30)>
		Public Overridable Property SalesTerritory() As SalesTerritory

#End Region
	End Class
End Namespace