// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using RestfulObjects.Snapshot.Constants;
using RestfulObjects.Snapshot.Representations;

namespace RestfulObjects.Snapshot.Utility {
    public abstract class RelType {
        private static readonly HashSet<string> HasRelParameterSet = new HashSet<string> {
            RelValues.AddTo, RelValues.Attachment, RelValues.Choice, RelValues.Clear, RelValues.Collection,
            RelValues.Default, RelValues.Details, RelValues.Invoke, RelValues.Modify, RelValues.RemoveFrom,
            RelValues.Service, RelValues.Value, RelValues.Prompt, RelValues.CollectionValue
        };

        protected readonly UriMtHelper helper;

        protected RelType(string name, UriMtHelper helper) {
            this.helper = helper;
            Name = name;
            Method = RelMethod.Get;
            HasRelParameter = HasRelParameterSet.Contains(name);
        }

        public virtual string Name { get; private set; }
        public RelMethod Method { get; set; }
        protected bool HasRelParameter { get; private set; }

        public abstract Uri GetUri();

        public abstract MediaTypeHeaderValue GetMediaType(RestControlFlags flags);
    }
}