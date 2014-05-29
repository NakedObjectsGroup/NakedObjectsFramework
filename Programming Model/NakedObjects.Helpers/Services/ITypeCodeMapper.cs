// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;

namespace NakedObjects.Services {
    /// <summary>
    ///     Defines a service that can convert between a Type and a string code where you don't wish to use the fully-qualified type name as the string representation.
    ///     Possible uses include:
    ///     - To create compound keys for defining polymorphic associations
    ///     - To create Oids for use in URLs
    /// </summary>
    public interface ITypeCodeMapper {
        Type TypeFromCode(string code);

        string CodeFromType(Type type);
    }
}