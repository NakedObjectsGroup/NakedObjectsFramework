Public Class StudentRepository
    Public Property Container As IContainer

    Public Function CreateNewStudent() As Student
        Return Container.CreateTransientInstance(Of Student)()
    End Function

    Public Function AllStudents() As IQueryable(Of Student)
        Return Container.AllInstances(Of Student)()
    End Function

    Public Function FindStudentByName(ByVal name As String) As IQueryable(Of Student)
        Return AllStudents().Where(Function(c) c.FullName.ToUpper().Contains(name.ToUpper()))
    End Function
End Class

