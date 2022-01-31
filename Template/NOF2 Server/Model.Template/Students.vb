Public Class Students

    Public Shared Function ActionCreateNewStudent() As Student
        Return ThreadLocals.Container.CreateTransientInstance(Of Student)()
    End Function

    Public Shared Function ActionAllStudents() As IQueryable(Of Student)
        Return ThreadLocals.Container.AllInstances(Of Student)()
    End Function

    Public Shared Function ActionFindStudentByName(ByVal name As String) As IQueryable(Of Student)
        Return From s In ActionAllStudents()
               Where s.mappedFullName.ToUpper().Contains(name.ToUpper())
    End Function

    Public Shared Function SharedMenuOrder() As Menu
        Dim main = New Menu("Students")
        main.AddAction(NameOf(ActionFindStudentByName)) _
        .AddAction(NameOf(ActionAllStudents)) _
        .AddAction(NameOf(ActionCreateNewStudent))
        Return main
    End Function

End Class

