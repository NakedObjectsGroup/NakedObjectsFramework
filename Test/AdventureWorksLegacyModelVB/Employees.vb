Namespace AW.Types

    Public Class Employees
        Private Shared Function Employees() As IQueryable(Of Employee)
            Return ThreadLocals.Container.AllInstances(Of Employee)
        End Function

        Public Shared Function ActionRandomEmployee() As Employee
            Return GenericMenuFunctions.Random(Of Employee)()
        End Function

        Public Shared Function ActionAllEmployees() As IQueryable(Of Employee)
            Return GenericMenuFunctions.ListAll(Of Employee)()
        End Function

        Public Shared Function ActionFindEmployeeByName(firstName As TextString, lastName As TextString) As ArrayList
            Return (From e In Employees()
                    Where e.PersonDetails.mappedLastName.ToUpper().StartsWith(lastName.Value.ToUpper()) AndAlso
                       (firstName.IsEmpty() OrElse e.PersonDetails.mappedFirstName.ToUpper().StartsWith(firstName.Value))).ToArrayList()
        End Function

        Public Shared Sub AboutActionFindEmployeeByName(a As ActionAbout, firstName As TextString, lastName As TextString)
            Select Case a.TypeCode
                Case AboutTypeCodes.Name
                Case AboutTypeCodes.Usable
                Case AboutTypeCodes.Valid
                    If lastName.IsEmpty() Then
                        a.Usable = False
                        a.UnusableReason = $"Last Name cannot be empty {a.TypeCode}"
                    End If
                Case Else
            End Select
        End Sub

        Public Shared Function ActionFindEmployeeByNationalIDNumber(nationalIDNumber As TextString) As Employee
            Return (From e In Employees()
                    Where e.mappedNationalIDNumber = nationalIDNumber.Value).FirstOrDefault()
        End Function

        Public Shared Function ActionListAllDepartments() As ArrayList
            Return GenericMenuFunctions.ListAll(Of Department).ToArrayList()
        End Function

        Public Shared Function ActionListNewDepartments() As IQueryable(Of Department)
            Return From d In GenericMenuFunctions.ListAll(Of Department)
                   Order By d.mappedModifiedDate Descending
        End Function

        Public Shared Function ActionCreateNewDepartment(name As TextString, groupName As TextString) As Department
            Dim container = ThreadLocals.Container
            Dim d As Department = CType(container.CreateTransientInstance(GetType(Department)), Department)
            d.Name.Value = name.Value
            d.GroupName.Value = groupName.Value
            d.mappedModifiedDate = Now
            container.MakePersistent(d)
            Return d
        End Function

        Public Shared Function SharedMenuOrder() As Menu
            Dim main = New Menu("Employees")
            main.AddAction(NameOf(ActionRandomEmployee)) _
            .AddAction(NameOf(ActionAllEmployees)) _
            .AddAction(NameOf(ActionFindEmployeeByName).ToLower()) _ 'To test case insensitivity
            .AddAction(NameOf(ActionFindEmployeeByNationalIDNumber))

            main.AddSubMenu("Organisation") _
            .AddAction(NameOf(ActionListAllDepartments)) _
            .AddAction(NameOf(ActionListNewDepartments)) _
            .AddAction(NameOf(ActionCreateNewDepartment))
            Return main
        End Function

    End Class
End Namespace
