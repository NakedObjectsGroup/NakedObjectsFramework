// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Net.Http;
using System.Runtime.Serialization;
using NakedObjects.Facade;
using NakedObjects.Facade.Contexts;
using RestfulObjects.Snapshot.Constants;
using RestfulObjects.Snapshot.Utility;

namespace RestfulObjects.Snapshot.Representations {
    [DataContract]
    public class PagedListRepresentation : ListRepresentation {
        protected PagedListRepresentation(IOidStrategy oidStrategy, ObjectContextFacade objectContext, HttpRequestMessage req, RestControlFlags flags, ActionContextFacade actionContext)
            : base(oidStrategy, objectContext, req, flags, actionContext) {
            //IObjectFacade list;

            //if (flags.PageSize > 0 && objectContext.Target.Count() > flags.PageSize) {
            //    warnings.Add(string.Format("Result contains more than {0} objects only returning the first {0}", flags.PageSize));
            //    list = objectContext.Target.Page(1, flags.PageSize);
            //}
            //else {
            //    list = objectContext.Target;
            //}

            SetPagination();
        }

        [DataMember(Name = JsonPropertyNames.Pagination)]
        public MapRepresentation Pagination { get; set; }

        private void SetPagination() {
            Pagination = new MapRepresentation();
        }

        public static ListRepresentation Create(IOidStrategy oidStrategy, ActionResultContextFacade actionResultContext, HttpRequestMessage req, RestControlFlags flags) {
            return new PagedListRepresentation(oidStrategy, actionResultContext.Result, req, flags, actionResultContext.ActionContext);
        }
    }
}