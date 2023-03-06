// // Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// // Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// // You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// // Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// // WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// // See the License for the specific language governing permissions and limitations under the License.

using System.Net;
using Newtonsoft.Json.Linq;
using ROSI.Interfaces;
using ROSI.Records;

namespace ROSI.Exceptions;

public class HttpErrorRosiException : HttpRosiException {
    public HttpErrorRosiException(HttpStatusCode statusCode, string body, string message, IInvokeOptions options) : base(statusCode, message) {
        Body = body;
        Error = string.IsNullOrWhiteSpace(body) ? null : new Error(JObject.Parse(body), options);
    }

    public string Body { get; }
    public Error? Error { get; set; }
}