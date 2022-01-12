Imports AW.Services
Namespace AW.Types
    Public Class Persons

        Public Shared Function ActionFindPersonByName(firstName As TextString, lastName As TextString) As IQueryable(Of Person)
            Dim rep = CType(ThreadLocals.Container.Repository(GetType(PersonRepository)), PersonRepository)
            Return rep.FindContactByName(firstName.Value, lastName.Value)
        End Function

    End Class
End Namespace
