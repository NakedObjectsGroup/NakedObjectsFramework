Public Class Subjects

    Public Shared Function ActionCreateNewSubject() As Subject
        Return ThreadLocals.Container.CreateTransientInstance(Of Subject)()
    End Function

    Public Shared Function ActionAllSubjects() As IQueryable(Of Subject)
        Return ThreadLocals.Container.AllInstances(Of Subject)()
    End Function

    Public Shared Function ActionFindSubjectByName(ByVal name As TextString) As IQueryable(Of Subject)
        Dim match = name.Value.ToUpper()
        Return ActionAllSubjects().Where(Function(c) c.mappedName.ToUpper().Contains(match))
    End Function

    Public Shared Function SharedMenuOrder() As Menu
        Dim main = New Menu("Subjects")
        main.AddAction(NameOf(ActionFindSubjectByName)) _
        .AddAction(NameOf(ActionAllSubjects)) _
        .AddAction(NameOf(ActionCreateNewSubject))
        Return main
    End Function
End Class

