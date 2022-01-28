Namespace AW.Services

    Public Class PersonRepository
        Implements IContainerAware

        Public Property Container As IContainer Implements IContainerAware.Container

#Region "FindContactByName"

        Public Function FindContactByName(ByVal firstName As String, ByVal lastName As String) As IQueryable(Of Person)
            Return From obj In Container.AllInstances(Of Person)()
                   Where (firstName Is Nothing OrElse obj.mappedFirstName.ToUpper().StartsWith(firstName.ToUpper())) AndAlso
                       obj.mappedLastName.ToUpper().StartsWith(lastName.ToUpper())
                   Order By obj.mappedLastName
        End Function

#End Region

#Region "ValidCountries"

        '         This method is needed because the AW database insists that every address has a StateProvince (silly design!), yet
        '         * many Countries in the database have no associated StateProvince.
        '         
        Public Function ValidCountries() As List(Of CountryRegion)
            Dim query As IQueryable(Of CountryRegion) = From state In Container.AllInstances(Of StateProvince)()
                                                        Select state.CountryRegion

            Return query.Distinct().ToList()
        End Function

#End Region

        Friend Function AddressesFor(ByVal entity As BusinessEntity) As IQueryable(Of Address)
            Dim id As Integer = entity.BusinessEntityID
            Return Container.AllInstances(Of BusinessEntityAddress)().Where(Function(bae) bae.BusinessEntityID = id).Select(Function(bae) bae.Address)
        End Function

        Public Function RecentAddresses() As IList(Of Address)
            Return Container.AllInstances(Of Address)().OrderByDescending(Function(a) a.ModifiedDate).Take(10).ToList()
        End Function

        Public Function RecentAddressLinks() As IList(Of BusinessEntityAddress)
            Return Container.AllInstances(Of BusinessEntityAddress)().OrderByDescending(Function(a) a.ModifiedDate).Take(10).ToList()
        End Function

        Public Function AllAddressTypes() As IQueryable(Of AddressType)
            Return Container.AllInstances(Of AddressType)()
        End Function
    End Class
End Namespace
