// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;

namespace NakedObjects.Services {
    /// <summary>
    ///     Defines a service that can convert between a key and a string code
    ///     Possible uses include:
    ///     - key encryption
    ///     - custom key separators
    /// </summary>
    public interface IKeyCodeMapper {
        string[] KeyFromCode(string code, Type type);

        string CodeFromKey(string[] key, Type type);
    }
}