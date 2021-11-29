Namespace AW.Types

	Partial Public Class EmployeeDepartmentHistory

		Public Sub New()
		End Sub

		Public Sub New(ByVal cloneFrom As EmployeeDepartmentHistory)
			EmployeeID = cloneFrom.EmployeeID
			DepartmentID = cloneFrom.DepartmentID
			ShiftID = cloneFrom.ShiftID
			StartDate = cloneFrom.StartDate
			EndDate = cloneFrom.EndDate
			Department = cloneFrom.Department
			Employee = cloneFrom.Employee
			Shift = cloneFrom.Shift
			ModifiedDate = cloneFrom.ModifiedDate
		End Sub

		<Hidden>
		Public Property EmployeeID() As Integer

		<Hidden>
		Public Property DepartmentID() As Short

		<Hidden>
		Public Property ShiftID() As Byte

		<MemberOrder(4)>
		<Mask("d")>
		Public Property StartDate() As DateTime

		<MemberOrder(5)>
		<Mask("d")>
		Public Property EndDate() As DateTime?

		<MemberOrder(2)>
		Public Overridable Property Department() As Department

		<MemberOrder(1)>
		Public Overridable Property Employee() As Employee

		<MemberOrder(3)>
		Public Overridable Property Shift() As Shift

		<MemberOrder(99)>
		Public Property ModifiedDate() As DateTime

		Public Overrides Function ToString() As String
			Return $"{Department} {StartDate.ToString("d")}"
		End Function
	End Class

End Namespace