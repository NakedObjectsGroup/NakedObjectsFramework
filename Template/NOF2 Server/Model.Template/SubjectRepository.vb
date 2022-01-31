Public Class SubjectRepository
    Public Property Container As IContainer

    Public Function CreateNewSubject() As Subject
        Return Container.CreateTransientInstance(Of Subject)()
    End Function

    Public Function AllSubjects() As IQueryable(Of Subject)
        Return Container.AllInstances(Of Subject)()
    End Function

    Public Function FindSubjectByName(ByVal name As String) As IQueryable(Of Subject)
        Return AllSubjects().Where(Function(c) c.Name.ToUpper().Contains(name.ToUpper()))
    End Function
End Class

