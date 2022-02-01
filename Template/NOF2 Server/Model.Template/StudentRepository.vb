Namespace Services
    Public Class StudentRepository
        Implements IContainerAware

        Public Property Container As IContainer Implements IContainerAware.Container

        Public Function ActionAllStudents() As IQueryable(Of Student)
            Return Container.AllInstances(Of Student)
        End Function

        Public Function ActionFindStudents(match As String) As IQueryable(Of Student)
            Return Container.AllInstances(Of Student)
        End Function

    End Class
End Namespace
