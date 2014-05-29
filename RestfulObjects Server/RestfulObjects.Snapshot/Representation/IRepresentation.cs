// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;

namespace RestfulObjects.Snapshot.Representations {
    public enum CacheType {
        Transactional,
        UserInfo,
        NonExpiring
    };

    public interface IRepresentation {
        MediaTypeHeaderValue GetContentType();
        EntityTagHeaderValue GetEtag();
        CacheType GetCaching();
        string[] GetWarnings();
        HttpResponseMessage GetAsMessage(MediaTypeFormatter formatter, Tuple<int, int, int> cacheSettings);
        Uri GetLocation();
    }
}