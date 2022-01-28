

Namespace AW.Types

	Partial Public Class Customer

		Implements ITitledObject, INotEditableOncePersistent
		''<Hidden>
		Public Property CustomerID() As Integer

		<DemoProperty(Order:=15)>
		Public ReadOnly Property CustomerType() As TextString
			Get
				Return New TextString(If(StoreID Is Nothing, "Individual", "Store"))
			End Get
		End Property

#Region "AccountNumber"
		Public Property mappedAccountNumber As String
		Friend myAccountNumber As TextString

		<DemoProperty(Order:=10)>
		Public ReadOnly Property AccountNumber As TextString
			Get
				myAccountNumber = If(myAccountNumber, New TextString(mappedAccountNumber, Sub(v) mappedAccountNumber = v))
Return myAccountNumber
			End Get
		End Property

		Public Sub AboutAccountNumber(a As FieldAbout, AccountNumber As TextString)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case Else
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
			Return $"{AccountNumber} {If(Store Is Nothing, Person.ToString(), Store.ToString)}"
		End Function

#Region "Store & Personal customers"

		''<Hidden>
		Public Property StoreID() As Integer?

		<DemoProperty(Order:=20)>
		Public Overridable Property Store() As Store

		''<Hidden>
		Public Property PersonID() As Integer?

		<DemoProperty(Order:=20)>
		Public Overridable Property Person() As Person

#End Region

#Region "Sales Territory"

		''<Hidden>
		Public Property SalesTerritoryID() As Integer?

		<DemoProperty(Order:=30)>
		Public Overridable Property SalesTerritory() As SalesTerritory

#End Region
	End Class
End Namespace