// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.ComponentModel.DataAnnotations;
using NakedObjects;
using NakedObjects.Redirect;

namespace RestfulObjects.Test.Data {
    public class RedirectedObject : IRedirectedObject {
        [Key, Title, ConcurrencyCheck]
        public virtual int Id { get; set; }

        [Hidden]
        public string ServerName { get; set; }
        [Hidden]
        public string Oid { get; set; }
    }
}