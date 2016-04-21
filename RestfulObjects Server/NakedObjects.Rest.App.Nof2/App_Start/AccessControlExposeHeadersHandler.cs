// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace MvcTestApp.App_Start {
    public class AccessControlExposeHeadersHandler : DelegatingHandler {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
            return base.SendAsync(request, cancellationToken).ContinueWith(
                task => {
                    HttpResponseMessage response = task.Result;
                    response.Headers.Add("Access-Control-Expose-Headers", "Content-Type");
                    return response;
                });
        }
    }
}