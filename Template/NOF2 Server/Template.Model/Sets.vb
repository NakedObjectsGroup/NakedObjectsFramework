Public Class Sets

    Public Shared Function ActionCreateNewSet() As TeachingSet
        Return ThreadLocals.Container.CreateTransientInstance(Of TeachingSet)()
    End Function

    Public Shared Function ActionListSets(subject As Subject, yearGroup As WholeNumberNullable) _
            As IQueryable(Of TeachingSet)

        Dim sets = ThreadLocals.Container.AllInstances(Of TeachingSet)()
        If subject IsNot Nothing Then
            Dim id As Integer = subject.Id
            sets = sets.Where(Function(s) s.Subject.Id = id)
        End If
        Dim yg = yearGroup.Value
        If yg IsNot Nothing Then
            sets = sets.Where(Function(s) s.mappedYearGroup = yg)
        End If
        Return sets.OrderBy(Function(s) s.mappedYearGroup).ThenBy(Function(s) s.Subject.mappedName)
    End Function

    Public Shared Function SharedMenuOrder() As Menu
        Dim main = New Menu("Sets")
        main.AddAction(NameOf(ActionListSets)) _
        .AddAction(NameOf(ActionCreateNewSet))
        Return main
    End Function


End Class

