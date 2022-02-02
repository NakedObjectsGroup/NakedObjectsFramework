Public Class Students

    Public Shared Function ActionCreateNewStudent() As Student
        Return ThreadLocals.Container.CreateTransientInstance(Of Student)()
    End Function

    Public Shared Function ActionAllStudents(container As IContainer) As IQueryable(Of Student)
        Return container.AllInstances(Of Student)()
    End Function

    Public Shared Function ActionFindStudentByName(ByVal name As TextString, container As IContainer) As IQueryable(Of Student)
        Dim match = name.Value.ToUpper()
        Return From s In ActionAllStudents(container)
               Where s.mappedFullName.ToUpper().Contains(match)
    End Function

    Public Shared Function SharedMenuOrder() As Menu
        Dim main = New Menu("Students")
        main.AddAction(NameOf(ActionFindStudentByName)) _
        .AddAction(NameOf(ActionAllStudents)) _
        .AddAction(NameOf(ActionCreateNewStudent))
        Return main
    End Function

End Class

