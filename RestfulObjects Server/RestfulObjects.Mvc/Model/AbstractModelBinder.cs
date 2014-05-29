// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.IO;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using Common.Logging;
using Newtonsoft.Json.Linq;
using RestfulObjects.Snapshot.Utility;

namespace RestfulObjects.Mvc.Model {
    public abstract class AbstractModelBinder : ModelBinderProvider, IModelBinder {
        #region IModelBinder Members

        public abstract bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext);

        #endregion

        public override IModelBinder GetBinder(System.Web.Http.HttpConfiguration configuration, Type modelType) {
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
                LogManager.GetCurrentClassLogger().ErrorFormat("Parsing of request arguments failed: {0}", e.Message);
                bindingContext.Model = failFunc();
            }
        }
    }
}