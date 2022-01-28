Imports AW.Services
Namespace AW.Types
    Public Class Persons

        Public Shared Function ActionFindPersonByName(firstName As TextString, lastName As TextString, container As IContainer) As IQueryable(Of Person)
            Dim rep = container.DomainService(Of PersonRepository)()
            Return rep.FindContactByName(firstName.Value, lastName.Value)
        End Function

        Public Shared Function ActionAllPersons() As IQueryable(Of Person)
            Return GenericMenuFunctions.ListAll(Of Person)()
        End Function

        Public Shared Function SharedMenuOrder() As Menu
            Dim main = New Menu("Contacts")
            main.AddAction(NameOf(ActionFindPersonByName)) _
            .AddAction(NameOf(ActionAllPersons))
            Return main
        End Function

    End Class
End Namespace
