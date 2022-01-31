




Public Class TeacherRepository
        Public Property Container As IContainer

        Public Function CreateNewTeacher() As Teacher
        Return Container.CreateTransientInstance(Of Teacher)()
    End Function

        Public Function AllTeachers() As IQueryable(Of Teacher)
        Return Container.AllInstances(Of Teacher)()
    End Function

        Public Function FindTeacherByName(ByVal name As String) As IQueryable(Of Teacher)
            Return AllTeachers().Where(Function(c) c.FullName.ToUpper().Contains(name.ToUpper()))
        End Function
    End Class

