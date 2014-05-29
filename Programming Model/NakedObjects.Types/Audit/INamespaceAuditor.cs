// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

namespace NakedObjects.Audit {
    /// <summary>
    /// Specific sub-type of IAuditor, where the methods will only
    /// be called in relation to types that fall within the specified
    /// namespace.
    /// </summary>
    public interface INamespaceAuditor : IAuditor {
        string NamespaceToAudit { get; }
    }
}