Imports System.Reflection

Public Module ModelConfig

    Public Function DomainModelTypes() As Type()
        Return Assembly.GetAssembly(GetType(Student)).GetTypes().
                     Where(Function(t) t.IsPublic AndAlso t.Namespace = "Template.Model").
                     ToArray()
    End Function

    Public Function DomainModelServices() As Type()
        Return New Type() {}
    End Function

    Public Function MainMenus() As Type()
        Return New Type() {GetType(Students), GetType(Sets), GetType(Teachers), GetType(Subjects)}
    End Function

End Module
