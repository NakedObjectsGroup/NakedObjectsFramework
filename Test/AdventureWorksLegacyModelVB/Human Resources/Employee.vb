Imports System.Linq

Namespace AW.Types

    Partial Public Class Employee

        Implements ITitledObject

        Public Property BusinessEntityID As Integer 'Not visible on UI

        '<MemberOrder(1)>
        Public Overridable Property PersonDetails() As Person

#Region "NationalIDNumber"
        Public mappedNationalIDNumber As String
        Friend myNationalIDNumber As TextString

        '<MemberOrder(10)>
        Public ReadOnly Property NationalIDNumber As TextString
            Get
                Return If(myNationalIDNumber, New TextString(mappedNationalIDNumber, Function(v) mappedNationalIDNumber = v))
            End Get
        End Property

        Public Sub AboutNationalIDNumber(a As FieldAbout, NationalIDNumber As TextString)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case AboutTypeCodes.Visible
            End Select
        End Sub
#End Region

#Region "JobTitle"
        Public mappedJobTitle As String
        Friend myJobTitle As TextString

        '<MemberOrder(12)>
        Public ReadOnly Property JobTitle As TextString
            Get
                Return If(myJobTitle, New TextString(mappedJobTitle, Function(v) mappedJobTitle = v))
            End Get
        End Property

        Public Sub AboutJobTitle(a As FieldAbout, JobTitle As TextString)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case AboutTypeCodes.Visible
            End Select
        End Sub
#End Region

#Region "DateOfBirth"
        Public mappedDateOfBirth As DateTime?
        Friend myDateOfBirth As NODateNullable

        '<MemberOrder(13)>
        Public ReadOnly Property DateOfBirth As NODateNullable
            Get
                Return If(myDateOfBirth, New NODateNullable(mappedDateOfBirth, Function(v) mappedDateOfBirth = v))
            End Get
        End Property

        Public Sub AboutDateOfBirth(a As FieldAbout, DateOfBirth As NODateNullable)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case AboutTypeCodes.Visible
            End Select
        End Sub
#End Region

#Region "MaritalStatus"
        Public mappedMaritalStatus As String
        Friend myMaritalStatus As TextString

        '<MemberOrder(14)>
        Public ReadOnly Property MaritalStatus As TextString
            Get
                Return If(myMaritalStatus, New TextString(mappedMaritalStatus, Function(v) mappedMaritalStatus = v))
            End Get
        End Property

        Public Sub AboutMaritalStatus(a As FieldAbout, MaritalStatus As TextString)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case AboutTypeCodes.Visible
            End Select
        End Sub
#End Region

#Region "Gender"
        Public mappedGender As String
        Friend myGender As TextString

        '<MemberOrder(15)>
        Public ReadOnly Property Gender As TextString
            Get
                Return If(myGender, New TextString(mappedGender, Function(v) mappedGender = v))
            End Get
        End Property

        Public Sub AboutGender(a As FieldAbout, Gender As TextString)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case AboutTypeCodes.Visible
            End Select
        End Sub
#End Region

#Region "HireDate"
        Public mappedHireDate As DateTime?
        Friend myHireDate As NODateNullable

        '<MemberOrder(16)>
        Public ReadOnly Property HireDate As NODateNullable
            Get
                Return If(myHireDate, New NODateNullable(mappedHireDate, Function(v) mappedHireDate = v))
            End Get
        End Property

        Public Sub AboutHireDate(a As FieldAbout, HireDate As NODateNullable)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case AboutTypeCodes.Visible
            End Select
        End Sub
#End Region

#Region "Salaried"
        Public mappedSalaried As Boolean
        Friend mySalaried As Logical

        '<MemberOrder(17)>
        Public ReadOnly Property Salaried As Logical
            Get
                Return If(mySalaried, New Logical(mappedSalaried, Function(v) mappedSalaried = v))
            End Get
        End Property

        Public Sub AboutSalaried(a As FieldAbout, Salaried As Logical)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case AboutTypeCodes.Visible
            End Select
        End Sub
#End Region

#Region "VacationHours"
        Public mappedVacationHours As Short
        Friend myVacationHours As WholeNumber

        '<MemberOrder(18)>
        Public ReadOnly Property VacationHours As WholeNumber
            Get
                Return If(myVacationHours, New WholeNumber(mappedVacationHours, Function(v) mappedVacationHours = v))
            End Get
        End Property

        Public Sub AboutVacationHours(a As FieldAbout, VacationHours As WholeNumber)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case AboutTypeCodes.Visible
            End Select
        End Sub
#End Region

#Region "SickLeaveHours"
        Public mappedSickLeaveHours As Short
        Friend mySickLeaveHours As WholeNumber

        '<MemberOrder(19)>
        Public ReadOnly Property SickLeaveHours As WholeNumber
            Get
                Return If(mySickLeaveHours, New WholeNumber(mappedSickLeaveHours, Function(v) mappedSickLeaveHours = v))
            End Get
        End Property

        Public Sub AboutSickLeaveHours(a As FieldAbout, SickLeaveHours As WholeNumber)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case AboutTypeCodes.Visible
            End Select
        End Sub
#End Region

#Region "Current"
        Public mappedCurrent As Boolean
        Friend myCurrent As Logical

        '<MemberOrder(20)>
        Public ReadOnly Property Current As Logical
            Get
                Return If(myCurrent, New Logical(mappedCurrent, Function(v) mappedCurrent = v))
            End Get
        End Property

        Public Sub AboutCurrent(a As FieldAbout, Current As Logical)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case AboutTypeCodes.Visible
            End Select
        End Sub
#End Region

        Public Property ManagerID As Integer?
        '<MemberOrder(30)>
        Public Overridable Property Manager() As Employee

#Region "LoginID"
        Public mappedLoginID As String
        Friend myLoginID As TextString

        '<MemberOrder(11)>
        Public ReadOnly Property LoginID As TextString
            Get
                Return If(myLoginID, New TextString(mappedLoginID, Function(v) mappedLoginID = v))
            End Get
        End Property

        Public Sub AboutLoginID(a As FieldAbout, LoginID As TextString)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                Case AboutTypeCodes.Visible
            End Select
        End Sub
#End Region

        '<Hidden>
        Public Overridable Property SalesPerson() As SalesPerson

#Region "DepartmentHistory (Collection)"
        Public Overridable Property mappedDepartmentHistory As ICollection(Of EmployeeDepartmentHistory) = New List(Of EmployeeDepartmentHistory)()

        Private myDepartmentHistory As InternalCollection

        '<MemberOrder(1)>
        Public ReadOnly Property DepartmentHistory As InternalCollection
            Get
                Return If(myDepartmentHistory, New InternalCollection(Of EmployeeDepartmentHistory)(mappedDepartmentHistory))
            End Get
        End Property

        Public Sub AboutDepartmentHistory(a As FieldAbout)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Visible
            End Select
        End Sub
#End Region

#Region "PayHistory (Collection)"
        Public Overridable Property mappedPayHistory As ICollection(Of EmployeePayHistory) = New List(Of EmployeePayHistory)()

        Private myPayHistory As InternalCollection

        '<MemberOrder(1)>
        Public ReadOnly Property PayHistory As InternalCollection
            Get
                Return If(myPayHistory, New InternalCollection(Of EmployeePayHistory)(mappedPayHistory))
            End Get
        End Property

        Public Sub AboutPayHistory(a As FieldAbout)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Visible
            End Select
        End Sub
#End Region


#Region "ModifiedDate"
        Public mappedModifiedDate As Date
        Friend myModifiedDate As TimeStamp

        '<MemberOrder(99)>
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

        Public Property RowGuid() As Guid 'Not visible on UI

#Region "Title"
        Public Function Title() As ITitle Implements ITitledObject.Title
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
#End Region


    End Class
End Namespace