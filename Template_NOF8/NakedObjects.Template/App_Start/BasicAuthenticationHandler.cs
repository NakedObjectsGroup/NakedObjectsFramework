// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NakedObjects.Facade;

namespace NakedObjects.Template {
    public class BasicAuthenticationHandler : DelegatingHandler {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
            HttpRequestMessage r = request;

            if (r.Headers.Authorization == null) {
                return Login();
            }

            AuthenticationHeaderValue auth = r.Headers.Authorization;

            byte[] decodedBytes = Convert.FromBase64String(auth.Parameter);
            string decodedUserAndPassword = Encoding.UTF8.GetString(decodedBytes);
            string[] userAndPassword = decodedUserAndPassword.Split(':');

            string user = userAndPassword[0];
            string password = userAndPassword[1];
            UserCredentials credentials = Validate(user, password);
            if (credentials != null) {
                var p = new GenericPrincipal(new GenericIdentity(credentials.User), credentials.Roles.ToArray());
                Thread.CurrentPrincipal = p;
                return base.SendAsync(request, cancellationToken);
            }

            // fail 
            return Login();
        }

        private UserCredentials Validate(string user, string password) {
            //THIS CODE IS A MOCK IMPLEMENTATION FOR ILLUSTRATION ONLY
            //Fail if either the user name or password is null or empty
            if (string.IsNullOrWhiteSpace(user)) return null;
            if (string.IsNullOrEmpty(password)) return null;
            //Otherwise, authenticate any user/password combination, with a standard set of roles
            return new UserCredentials(user, password, new List<string> {"role1", "role2"});
        }

        private static Task<HttpResponseMessage> Login() {
            var taskFactory = new TaskFactory<HttpResponseMessage>();
            return taskFactory.StartNew(() => {
                var response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                response.Headers.WwwAuthenticate.Add(new AuthenticationHeaderValue("Basic", "realm=\"any user and password\""));
                return response;
            });
        }
    }
}