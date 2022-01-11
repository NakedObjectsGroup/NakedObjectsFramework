Namespace AW.Types

	Partial Public Class EmployeeDepartmentHistory

		Implements ITitledObject

		Public Property EmployeeID() As Integer

		Public Property DepartmentID() As Short

		Public Property ShiftID() As Byte

#Region "StartDate"
		Public Property mappedStartDate As Date
		Friend myStartDate As NODate

		<MemberOrder(4)>
		Public ReadOnly Property StartDate As NODate
			Get
				Return If(myStartDate, New NODate(mappedStartDate, Function(v) mappedStartDate = v))
			End Get
		End Property

		Public Sub AboutStartDate(a As FieldAbout, StartDate As NODate)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region

#Region "EndDate"
		Public mappedEndDate As Date?
		Friend myEndDate As NODateNullable

		<MemberOrder(5)>
		Public ReadOnly Property EndDate As NODateNullable
			Get
				Return If(myEndDate, New NODateNullable(mappedEndDate, Function(v) mappedEndDate = v))
			End Get
		End Property

		Public Sub AboutEndDate(a As FieldAbout, EndDate As NODateNullable)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case AboutTypeCodes.Visible
			End Select
		End Sub
#End Region

		<MemberOrder(2)>
		Public Overridable Property Department() As Department

		<MemberOrder(1)>
		Public Overridable Property Employee() As Employee

		<MemberOrder(3)>
		Public Overridable Property Shift() As Shift

#Region "ModifiedDate"
		Public mappedModifiedDate As Date
		Friend myModifiedDate As TimeStamp

		<MemberOrder(99)>
		Public ReadOnly Property ModifiedDate As TimeStamp
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

		Public Function Title() As ITitle Implements ITitledObject.Title
			Return New Title(ToString())
		End Function

		Public Overrides Function ToString() As String
			Return $"{Department} {StartDate}"
		End Function
	End Class

End Namespace