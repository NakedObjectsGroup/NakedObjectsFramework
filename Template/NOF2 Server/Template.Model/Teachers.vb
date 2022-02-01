Public Class Teachers

    Public Shared Function ActionCreateNewTeacher() As Teacher
        Return ThreadLocals.Container.CreateTransientInstance(Of Teacher)()
    End Function

    Public Shared Function ActionAllTeachers() As IQueryable(Of Teacher)
        Return ThreadLocals.Container.AllInstances(Of Teacher)()
    End Function

    Public Shared Function ActionFindTeacherByName(ByVal name As TextString) As IQueryable(Of Teacher)
        Dim match = name.Value.ToUpper()
        Return ActionAllTeachers().Where(Function(c) c.mappedFullName.ToUpper().Contains(match))
    End Function

    Public Shared Function SharedMenuOrder() As Menu
        Dim main = New Menu("Teachers")
        main.AddAction(NameOf(ActionFindTeacherByName)) _
        .AddAction(NameOf(ActionAllTeachers)) _
        .AddAction(NameOf(ActionCreateNewTeacher))
        Return main
    End Function

End Class

