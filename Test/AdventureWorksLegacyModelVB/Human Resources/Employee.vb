Namespace AW.Types

	Public Interface IEmployee
		Inherits IBusinessEntity

	End Interface 'Interface is for testing purposes

	Partial Public Class Employee
		Implements IEmployee, IHasRowGuid, IHasModifiedDate

		Public Sub New()
		End Sub

		Public Sub New(ByVal cloneFrom As Employee)
			BusinessEntityID = cloneFrom.BusinessEntityID
			PersonDetails = cloneFrom.PersonDetails
			NationalIDNumber = cloneFrom.NationalIDNumber
			JobTitle = cloneFrom.JobTitle
			DateOfBirth = cloneFrom.DateOfBirth
			MaritalStatus = cloneFrom.MaritalStatus
			Gender = cloneFrom.Gender
			HireDate = cloneFrom.HireDate
			Salaried = cloneFrom.Salaried
			VacationHours = cloneFrom.VacationHours
			SickLeaveHours = cloneFrom.SickLeaveHours
			Current = cloneFrom.Current
			Manager = cloneFrom.Manager
			LoginID = cloneFrom.LoginID
			SalesPerson = cloneFrom.SalesPerson
			ModifiedDate = cloneFrom.ModifiedDate
			rowguid = cloneFrom.rowguid
			DepartmentHistory = cloneFrom.DepartmentHistory
			PayHistory = cloneFrom.PayHistory
		End Sub

		<Hidden>
		Public Property BusinessEntityID() As Integer Implements AW.Types.IBusinessEntity.BusinessEntityID

		<MemberOrder(1)>
		Public Overridable Property PersonDetails() As Person

		<MemberOrder(10)>
		Public Property NationalIDNumber() As String = ""

		<MemberOrder(12)>
		Public Property JobTitle() As String = ""

		<MemberOrder(13), Mask("d")>
		Public Property DateOfBirth() As DateTime?

		<MemberOrder(14)>
		Public Property MaritalStatus() As String = ""

		<MemberOrder(15)>
		Public Property Gender() As String = ""

		<MemberOrder(16)>
		<Mask("d")>
		Public Property HireDate() As DateTime?

		<MemberOrder(17)>
		Public Property Salaried() As Boolean

		<MemberOrder(18)>
		Public Property VacationHours() As Short

		<MemberOrder(19)>
		Public Property SickLeaveHours() As Short

		<MemberOrder(20)>
		Public Property Current() As Boolean

		'INSTANT VB WARNING: Nullable reference types have no equivalent in VB:
		'ORIGINAL LINE: public virtual Employee? Manager {get;set;}
		<MemberOrder(30)>
		Public Overridable Property Manager() As Employee

		<MemberOrder(11)>
		Public Property LoginID() As String = ""

		'INSTANT VB WARNING: Nullable reference types have no equivalent in VB:
		'ORIGINAL LINE: public virtual SalesPerson? SalesPerson {get;set;}
		<Hidden>
		Public Overridable Property SalesPerson() As SalesPerson

		<TableView(True, NameOf(EmployeeDepartmentHistory.StartDate), NameOf(EmployeeDepartmentHistory.EndDate), NameOf(EmployeeDepartmentHistory.Department), NameOf(EmployeeDepartmentHistory.Shift))>
		Public Overridable Property DepartmentHistory() As ICollection(Of EmployeeDepartmentHistory) = New List(Of EmployeeDepartmentHistory)()

		<TableView(True, NameOf(EmployeePayHistory.RateChangeDate), NameOf(EmployeePayHistory.Rate))>
		Public Overridable Property PayHistory() As ICollection(Of EmployeePayHistory) = New List(Of EmployeePayHistory)()


		<MemberOrder(99)>
		Public Property ModifiedDate() As DateTime Implements IHasModifiedDate.ModifiedDate

		<Hidden>
		Public Property rowguid() As Guid Implements IHasRowGuid.rowguid

		Public Overrides Function ToString() As String
			Return $"{PersonDetails}"
		End Function


	End Class
End Namespace