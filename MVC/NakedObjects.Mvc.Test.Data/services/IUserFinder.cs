// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
namespace Expenses.Services {
    /// <summary> Defines a service for getting hold of the current user as an object.  Will typically
    /// be implemented by a service that manages a type of object that represents users
    /// (e.g. an EmployeeRepository or OrganisationRepository).
    /// 
    /// </summary>
    /// <author>  Richard
    /// 
    /// </author>
    public interface IUserFinder {
        object CurrentUserAsObject();
    }
}