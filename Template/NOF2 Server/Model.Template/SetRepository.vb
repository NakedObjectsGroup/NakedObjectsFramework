Public Class SetRepository
    Public Property Container As IContainer

    Public Function CreateNewSet() As [Set]
        Return Container.CreateTransientInstance(Of [Set])()
    End Function

    Public Function ListSets(
 ByVal subject As Subject,
 ByVal yearGroup As Integer?) As IQueryable(Of [Set])
        Dim sets = Container.AllInstances(Of [Set])()

        If subject IsNot Nothing Then
            Dim id As Integer = subject.Id
            sets = sets.Where(Function(s) s.Subject.Id = id)
        End If

        If yearGroup IsNot Nothing Then
            sets = sets.Where(Function(s) s.YearGroup = yearGroup.Value)
        End If

        Return sets.OrderBy(Function(s) s.YearGroup).ThenBy(Function(s) s.Subject.Name)
    End Function
End Class

