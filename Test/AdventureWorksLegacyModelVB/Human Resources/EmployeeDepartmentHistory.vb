﻿Namespace AW.Types

	Partial Public Class EmployeeDepartmentHistory

		Public Property EmployeeID() As Integer

		Public Property DepartmentID() As Short

		Public Property ShiftID() As Byte

#Region "StartDate"
		Friend mappedStartDate As Date
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
		Friend mappedEndDate As Date?
		Friend myEndDate As NODate

		<MemberOrder(5)>
		Public ReadOnly Property EndDate As NODate
			Get
				Return If(myEndDate, New NODate(If(mappedEndDate, Nothing), Function(v) mappedEndDate = v))
			End Get
		End Property

		Public Sub AboutEndDate(a As FieldAbout, EndDate As DateTime)
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
		Friend mappedModifiedDate As Date
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

		Public Function Title() As Title
			Return New Title($"{Department} {StartDate.ToString("d")}")
		End Function
	End Class

End Namespace