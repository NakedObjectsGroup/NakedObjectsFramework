﻿using AW.Functions;

namespace AW.Types;

[ViewModel(typeof(CustomerCollectionViewModel_Functions))]
public class CustomerCollectionViewModel
{
    public CustomerCollectionViewModel(IList<Customer> customers) => Customers = customers;

    [Hidden]
    public IList<Customer> Customers { get; init; }
}