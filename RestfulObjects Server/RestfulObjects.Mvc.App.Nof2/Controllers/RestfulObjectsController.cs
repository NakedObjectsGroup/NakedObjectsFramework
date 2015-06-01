// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using NakedObjects.Facade;
using NakedObjects.Surface;
using RestfulObjects.Mvc;
using RestfulObjects.Mvc.Model;

namespace MvcTestApp.Controllers {
    public class RestfulObjectsController : RestfulObjectsControllerBase {

        public RestfulObjectsController(IFrameworkFacade surface) : base(surface) { }

        [HttpGet]
        public override HttpResponseMessage GetHome([ModelBinder(typeof (ReservedArgumentsBinder))] ReservedArguments arguments) {
            return base.GetHome(arguments);
        }

        [HttpGet]
        public override HttpResponseMessage GetUser([ModelBinder(typeof(ReservedArgumentsBinder))] ReservedArguments arguments) {
            return base.GetUser(arguments);
        }

        [HttpGet]
        public override HttpResponseMessage GetServices([ModelBinder(typeof (ReservedArgumentsBinder))] ReservedArguments arguments) {
            return base.GetServices(arguments);
        }

        [HttpGet]
        public override HttpResponseMessage GetVersion([ModelBinder(typeof(ReservedArgumentsBinder))] ReservedArguments arguments) {
            return base.GetVersion(arguments);
        }

        [HttpGet]
        public override HttpResponseMessage GetDomainTypes([ModelBinder(typeof(ReservedArgumentsBinder))] ReservedArguments arguments) {
            return base.GetDomainTypes(arguments);
        }

        [HttpGet]
        public override HttpResponseMessage GetDomainType(string typeName, [ModelBinder(typeof(ReservedArgumentsBinder))] ReservedArguments arguments) {
            return base.GetDomainType(typeName, arguments);
        }

        [HttpGet]
        public override HttpResponseMessage GetService(string serviceName, [ModelBinder(typeof (ReservedArgumentsBinder))] ReservedArguments arguments) {
            return base.GetService(serviceName, arguments);
        }

        [HttpGet]
        public override HttpResponseMessage GetServiceAction(string serviceName, string actionName, [ModelBinder(typeof (ReservedArgumentsBinder))] ReservedArguments arguments) {
            return base.GetServiceAction(serviceName, actionName, arguments);
        }

        [HttpGet]
        public override HttpResponseMessage GetImage(string imageId, [ModelBinder(typeof (ReservedArgumentsBinder))] ReservedArguments arguments) {
            return base.GetImage(imageId, arguments);
        }

        [HttpPost]
        public override HttpResponseMessage PostPersist(string domainType, [ModelBinder(typeof (PersistArgumentMapBinder))] ArgumentMap arguments) {
            return base.PostPersist(domainType, arguments);
        }

        [HttpGet]
        public override HttpResponseMessage GetObject(string domainType, string instanceId, [ModelBinder(typeof (ReservedArgumentsBinder))] ReservedArguments arguments) {
            return base.GetObject(domainType, instanceId, arguments);
        }

        [HttpPut]
        public override HttpResponseMessage PutObject(string domainType, string instanceId, [ModelBinder(typeof (ArgumentMapBinder))] ArgumentMap arguments) {
            return base.PutObject(domainType, instanceId, arguments);
        }

        [HttpGet]
        public override HttpResponseMessage GetProperty(string domainType, string instanceId, string propertyName, [ModelBinder(typeof (ReservedArgumentsBinder))] ReservedArguments arguments) {
            return base.GetProperty(domainType, instanceId, propertyName, arguments);
        }

        [HttpGet]
        public override HttpResponseMessage GetPropertyType(string typeName, string propertyName, [ModelBinder(typeof(ReservedArgumentsBinder))] ReservedArguments arguments) {
            return base.GetPropertyType(typeName, propertyName, arguments);
        }

        [HttpGet]
        public override HttpResponseMessage GetCollection(string domainType, string instanceId, string propertyName, [ModelBinder(typeof (ReservedArgumentsBinder))] ReservedArguments arguments) {
            return base.GetCollection(domainType, instanceId, propertyName, arguments);
        }

        [HttpGet]
        public override HttpResponseMessage GetCollectionType(string typeName, string propertyName, [ModelBinder(typeof(ReservedArgumentsBinder))] ReservedArguments arguments) {
            return base.GetCollectionType(typeName, propertyName, arguments);
        }

        [HttpGet]
        public override HttpResponseMessage GetAction(string domainType, string instanceId, string actionName, [ModelBinder(typeof (ReservedArgumentsBinder))] ReservedArguments arguments) {
            return base.GetAction(domainType, instanceId, actionName, arguments);
        }

        [HttpGet]
        public override HttpResponseMessage GetActionType(string typeName, string actionName, [ModelBinder(typeof(ReservedArgumentsBinder))] ReservedArguments arguments) {
            return base.GetActionType(typeName, actionName, arguments);
        }

        [HttpGet]
        public override HttpResponseMessage GetActionParameterType(string typeName, string actionName, string parmName, [ModelBinder(typeof(ReservedArgumentsBinder))] ReservedArguments arguments) {
            return base.GetActionParameterType(typeName, actionName, parmName, arguments);
        }

        [HttpPut]
        public override HttpResponseMessage PutProperty(string domainType, string instanceId, string propertyName, [ModelBinder(typeof (SingleValueArgumentBinder))] SingleValueArgument argument) {
            return base.PutProperty(domainType, instanceId, propertyName, argument);
        }

        [HttpDelete]
        public override HttpResponseMessage DeleteProperty(string domainType, string instanceId, string propertyName, [ModelBinder(typeof (ReservedArgumentsBinder))] ReservedArguments arguments) {
            return base.DeleteProperty(domainType, instanceId, propertyName, arguments);
        }

        [HttpPost]
        public override HttpResponseMessage PostCollection(string domainType, string instanceId, string propertyName, [ModelBinder(typeof (SingleValueArgumentBinder))] SingleValueArgument argument) {
            return base.PostCollection(domainType, instanceId, propertyName, argument);
        }

        [HttpDelete]
        public override HttpResponseMessage DeleteCollection(string domainType, string instanceId, string propertyName, [ModelBinder(typeof (SingleValueArgumentUrlBinder))] SingleValueArgument argument) {
            return base.DeleteCollection(domainType, instanceId, propertyName, argument);
        }

        [HttpGet]
        public override HttpResponseMessage GetInvoke(string domainType, string instanceId, string actionName, [ModelBinder(typeof (ArgumentMapUrlBinder))] ArgumentMap arguments) {
            return base.GetInvoke(domainType, instanceId, actionName, arguments);
        }

        [HttpPost]
        public override HttpResponseMessage PostInvoke(string domainType, string instanceId, string actionName, [ModelBinder(typeof (ArgumentMapBinder))] ArgumentMap arguments) {
            return base.PostInvoke(domainType, instanceId, actionName, arguments);
        }

        [HttpPut]
        public override HttpResponseMessage PutInvoke(string domainType, string instanceId, string actionName, [ModelBinder(typeof (ArgumentMapBinder))] ArgumentMap arguments) {
            return base.PutInvoke(domainType, instanceId, actionName, arguments);
        }

        [HttpGet]
        public override HttpResponseMessage GetInvokeOnService(string serviceName, string actionName, [ModelBinder(typeof (ArgumentMapUrlBinder))] ArgumentMap arguments) {
            return base.GetInvokeOnService(serviceName, actionName, arguments);
        }

        [HttpPut]
        public override HttpResponseMessage PutInvokeOnService(string serviceName, string actionName, [ModelBinder(typeof (ArgumentMapBinder))] ArgumentMap arguments) {
            return base.PutInvokeOnService(serviceName, actionName, arguments);
        }

        [HttpPost]
        public override HttpResponseMessage PostInvokeOnService(string serviceName, string actionName, [ModelBinder(typeof (ArgumentMapBinder))] ArgumentMap arguments) {
            return base.PostInvokeOnService(serviceName, actionName, arguments);
        }

        [HttpGet]
        public override HttpResponseMessage GetInvokeIsTypeOf(string typeName, string actionName, [ModelBinder(typeof (ArgumentMapUrlBinder))] ArgumentMap arguments) {
            return base.GetInvokeIsTypeOf(typeName, actionName, arguments);
        }

        public virtual HttpResponseMessage InvalidMethod() {
            throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.MethodNotAllowed));
        }

        
    }
}