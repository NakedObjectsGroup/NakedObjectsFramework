Imports System.Linq

Namespace AW.Types

    Partial Public Class Employee

        Implements ITitledObject

        Public Property BusinessEntityID As Integer 'Not visible on UI

        <MemberOrder(1)>
        Public Overridable Property PersonDetails() As Person

#Region "NationalIDNumber"
        Public Property mappedNationalIDNumber As String
        Friend myNationalIDNumber As TextString

        <MemberOrder(10)>
        Public ReadOnly Property NationalIDNumber As TextString
            Get
                myNationalIDNumber = If(myNationalIDNumber, New TextString(mappedNationalIDNumber, Sub(v) mappedNationalIDNumber = v))
                Return myNationalIDNumber
            End Get
        End Property

        Public Sub AboutNationalIDNumber(a As FieldAbout, NationalIDNumber As TextString)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case Else
            End Select
        End Sub
#End Region

#Region "JobTitle"
        Public Property mappedJobTitle As String
        Friend myJobTitle As TextString

        <MemberOrder(12)>
        Public ReadOnly Property JobTitle As TextString
            Get
                myJobTitle = If(myJobTitle, New TextString(mappedJobTitle, Sub(v) mappedJobTitle = v))
                Return myJobTitle
            End Get
        End Property

        Public Sub AboutJobTitle(a As FieldAbout, JobTitle As TextString)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case Else
            End Select
        End Sub
#End Region

#Region "DateOfBirth"
        Public Property mappedDateOfBirth As DateTime?
        Friend myDateOfBirth As NODateNullable

        <MemberOrder(13)>
        Public ReadOnly Property DateOfBirth As NODateNullable
            Get
                myDateOfBirth = If(myDateOfBirth, New NODateNullable(mappedDateOfBirth, Sub(v) mappedDateOfBirth = v))
                Return myDateOfBirth
            End Get
        End Property

        Public Sub AboutDateOfBirth(a As FieldAbout, DateOfBirth As NODateNullable)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case Else
            End Select
        End Sub
#End Region

#Region "MaritalStatus"
        Public Property mappedMaritalStatus As String
        Friend myMaritalStatus As TextString

        <MemberOrder(14)>
        Public ReadOnly Property MaritalStatus As TextString
            Get
                myMaritalStatus = If(myMaritalStatus, New TextString(mappedMaritalStatus, Sub(v) mappedMaritalStatus = v))
                Return myMaritalStatus
            End Get
        End Property

        Public Sub AboutMaritalStatus(a As FieldAbout, MaritalStatus As TextString)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case Else
            End Select
        End Sub
#End Region

#Region "Gender"
        Public Property mappedGender As String
        Friend myGender As TextString

        <MemberOrder(15)>
        Public ReadOnly Property Gender As TextString
            Get
                myGender = If(myGender, New TextString(mappedGender, Sub(v) mappedGender = v))
                Return myGender
            End Get
        End Property

        Public Sub AboutGender(a As FieldAbout, Gender As TextString)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case Else
            End Select
        End Sub
#End Region

#Region "HireDate"
        Public Property mappedHireDate As DateTime?
        Friend myHireDate As NODateNullable

        <MemberOrder(16)>
        Public ReadOnly Property HireDate As NODateNullable
            Get
                myHireDate = If(myHireDate, New NODateNullable(mappedHireDate, Sub(v) mappedHireDate = v))
                Return myHireDate
            End Get
        End Property

        Public Sub AboutHireDate(a As FieldAbout, HireDate As NODateNullable)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case Else
            End Select
        End Sub
#End Region

#Region "Salaried"
        Public Property mappedSalaried As Boolean
        Friend mySalaried As Logical

        <MemberOrder(17)>
        Public ReadOnly Property Salaried As Logical
            Get
                mySalaried = If(mySalaried, New Logical(mappedSalaried, Sub(v) mappedSalaried = v))
                Return mySalaried
            End Get
        End Property

        Public Sub AboutSalaried(a As FieldAbout, Salaried As Logical)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case Else
            End Select
        End Sub
#End Region

#Region "VacationHours"
        Public Property mappedVacationHours As Short
        Friend myVacationHours As WholeNumber

        <MemberOrder(18)>
        Public ReadOnly Property VacationHours As WholeNumber
            Get
                myVacationHours = If(myVacationHours, New WholeNumber(mappedVacationHours, Sub(v) mappedVacationHours = CType(v, Short)))
                Return myVacationHours
            End Get
        End Property

        Public Sub AboutVacationHours(a As FieldAbout, VacationHours As WholeNumber)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case Else
            End Select
        End Sub
#End Region

#Region "SickLeaveHours"
        Public Property mappedSickLeaveHours As Short
        Friend mySickLeaveHours As WholeNumber

        <MemberOrder(19)>
        Public ReadOnly Property SickLeaveHours As WholeNumber
            Get
                mySickLeaveHours = If(mySickLeaveHours, New WholeNumber(mappedSickLeaveHours, Sub(v) mappedSickLeaveHours = CType(v, Short)))
                Return mySickLeaveHours
            End Get
        End Property

        Public Sub AboutSickLeaveHours(a As FieldAbout, SickLeaveHours As WholeNumber)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case Else
            End Select
        End Sub
#End Region

#Region "Current"
        Public Property mappedCurrent As Boolean
        Friend myCurrent As Logical

        <MemberOrder(20)>
        Public ReadOnly Property Current As Logical
            Get
                myCurrent = If(myCurrent, New Logical(mappedCurrent, Sub(v) mappedCurrent = v))
                Return myCurrent
            End Get
        End Property

        Public Sub AboutCurrent(a As FieldAbout, Current As Logical)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case Else
            End Select
        End Sub
#End Region

        Public Property ManagerID As Integer?
        <MemberOrder(30)>
        Public Overridable Property Manager() As Employee

#Region "LoginID"
        Public Property mappedLoginID As String
        Friend myLoginID As TextString

        <MemberOrder(11)>
        Public ReadOnly Property LoginID As TextString
            Get
                myLoginID = If(myLoginID, New TextString(mappedLoginID, Sub(v) mappedLoginID = v))
                Return myLoginID
            End Get
        End Property

        Public Sub AboutLoginID(a As FieldAbout, LoginID As TextString)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case Else
            End Select
        End Sub
#End Region

        '<Hidden>
        Public Overridable Property SalesPerson() As SalesPerson

#Region "DepartmentHistory (Collection)"
        Public Overridable Property mappedDepartmentHistory As ICollection(Of EmployeeDepartmentHistory) = New List(Of EmployeeDepartmentHistory)()

        Private myDepartmentHistory As InternalCollection

        <MemberOrder(1)>
        Public ReadOnly Property DepartmentHistory As InternalCollection
            Get
                myDepartmentHistory = If(myDepartmentHistory, New InternalCollection(Of EmployeeDepartmentHistory)(mappedDepartmentHistory))
                Return myDepartmentHistory
            End Get
        End Property

        Public Sub AboutDepartmentHistory(a As FieldAbout)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case Else
            End Select
        End Sub
#End Region

#Region "PayHistory (Collection)"
        Public Overridable Property mappedPayHistory As ICollection(Of EmployeePayHistory) = New List(Of EmployeePayHistory)()

        Private myPayHistory As InternalCollection

        <MemberOrder(1)>
        Public ReadOnly Property PayHistory As InternalCollection
            Get
                myPayHistory = If(myPayHistory, New InternalCollection(Of EmployeePayHistory)(mappedPayHistory))
                Return myPayHistory
            End Get
        End Property

        Public Sub AboutPayHistory(a As FieldAbout)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case Else
            End Select
        End Sub
#End Region


#Region "ModifiedDate"
        Public Property mappedModifiedDate As Date
        Friend myModifiedDate As TimeStamp

        <MemberOrder(99)>
        Public ReadOnly Property ModifiedDate As TimeStamp
            Get
                myModifiedDate = If(myModifiedDate, New TimeStamp(mappedModifiedDate, Sub(v) mappedModifiedDate = v))
                Return myModifiedDate
            End Get
        End Property

        Public Sub AboutModifiedDate(a As FieldAbout)
            Select Case a.TypeCode
                Case AboutTypeCodes.Usable
                    a.Usable = False
            End Select
        End Sub
#End Region

        Public Property RowGuid() As Guid 'Not visible on UI

#Region "Title"
        Public Function Title() As Title Implements ITitledObject.Title
            Return New Title(ToString())
        End Function

        Public Overrides Function ToString() As String
            Return PersonDetails.ToString()
        End Function
#End Region

#Region "Actions"
        Public Sub ActionChangeDepartmentOrShift(department As Department, shift As Shift)
            Throw New NotImplementedException
        End Sub

        Public Sub ActionChangeStatus(newStatus As TextString)
            Me.MaritalStatus.Value = newStatus.Value
        End Sub

        Public Sub AboutActionChangeStatus(a As ActionAbout, status As TextString)
            Dim s = New TextString("S")
            Dim m = New TextString("M")
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                    a.Name = "Change Marital Status"
                Case AboutTypeCodes.Parameters
                    a.ParamLabels() = New String() {"New Marital Status"}
                    a.ParamDefaultValues() = New Object() {If(MaritalStatus.Value = "S", m, s)}
                    a.ParamOptions() = New Object()() {New TextString() {s, m}}
                Case AboutTypeCodes.Valid
                    Throw New Exception($"{MaritalStatus}, {status}, {MaritalStatus.Value = status.Value}")
                    If status.Value = MaritalStatus.Value Then
                        a.Usable = False
                        a.UnusableReason = "New Status cannot be the same as current"
                    End If
                Case Else
            End Select
        End Sub
#End Region


    End Class
End Namespace