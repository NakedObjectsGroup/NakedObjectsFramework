

Namespace AW.Types

	Partial Public Class Customer
 Implements ITitledObject
		''<Hidden>
		Public Property CustomerID() As Integer

		'<MemberOrder(15)>
		Public ReadOnly Property CustomerType() As TextString
			Get
				Return New TextString("TODO") 'TODO If(Me.IsIndividual(), "Individual", "Store")
			End Get
		End Property

#Region "AccountNumber"
		Public mappedAccountNumber As String
		Friend myAccountNumber As TextString

		'<MemberOrder(10)>
		Public ReadOnly Property AccountNumber As TextString
			Get
				Return If(myAccountNumber, New TextString(mappedAccountNumber, Function(v) mappedAccountNumber = v))
			End Get
		End Property

		Public Sub AboutAccountNumber(a As FieldAbout, AccountNumber As TextString)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region

		'<Hidden>
		Public Property CustomerModifiedDate() As DateTime

		'<Hidden>
		Public Property CustomerRowguid() As Guid

		Public Function Title() As Title Implements ITitledObject.Title
			Return New Title(ToString())
		End Function

		Public Overrides Function ToString() As String
			Return $"{AccountNumber} {(If(Store Is Nothing, Person, Store))}"
		End Function

#Region "Store & Personal customers"

		''<Hidden>
		Public Property StoreID() As Integer?

		'<MemberOrder(20)>
		Public Overridable Property Store() As Store

		''<Hidden>
		Public Property PersonID() As Integer?

		'<MemberOrder(20)>
		Public Overridable Property Person() As Person

#End Region

#Region "Sales Territory"

		''<Hidden>
		Public Property SalesTerritoryID() As Integer?

		'<MemberOrder(30)>
		Public Overridable Property SalesTerritory() As SalesTerritory

#End Region
	End Class
End Namespace