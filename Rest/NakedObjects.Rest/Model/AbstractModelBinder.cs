// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.IO;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using Common.Logging;
using Newtonsoft.Json.Linq;
using NakedObjects.Rest.Snapshot.Utility;

namespace NakedObjects.Rest.Model {
    public abstract class AbstractModelBinder : ModelBinderProvider, IModelBinder {
        #region IModelBinder Members

        public abstract bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext);

        #endregion

        public override IModelBinder GetBinder(HttpConfiguration configuration, Type modelType) {
            return this;
        }

        protected static object DeserializeContent(HttpActionContext actionContext) {
            if (RestUtils.IsJsonMediaType(actionContext.Request.Content.Headers.ContentType.MediaType)) {
                return DeserializeJsonContent(actionContext);
            }
            return DeserializeBinaryContent(actionContext);
        }

        protected static object DeserializeBinaryContent(HttpActionContext actionContext) {
            Stream stream = actionContext.Request.Content.ReadAsStreamAsync().Result;
            return ModelBinderUtils.DeserializeBinaryStream(stream);
        }

        protected static JObject DeserializeJsonContent(HttpActionContext actionContext) {
            Stream stream = actionContext.Request.Content.ReadAsStreamAsync().Result;
            return ModelBinderUtils.DeserializeJsonStream(stream);
        }

        protected static JObject DeserializeQueryString(HttpActionContext actionContext) {
            using (Stream stream = ModelBinderUtils.QueryStringToStream(actionContext.Request.RequestUri.Query)) {
                return ModelBinderUtils.DeserializeJsonStream(stream);
            }
        }

        protected static void BindModelOnSuccessOrFail(ModelBindingContext bindingContext, Func<object> parseFunc, Func<object> failFunc) {
            try {
                bindingContext.Model = parseFunc();
            }
            catch (Exception e) {
                LogManager.GetLogger<AbstractModelBinder>().ErrorFormat("Parsing of request arguments failed: {0}", e.Message);
                bindingContext.Model = failFunc();
            }
        }
    }
}