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
using Microsoft.AspNetCore.Http.Headers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Net.Http.Headers;
using NakedObjects.Facade;
using NakedObjects.Facade.Contexts;
using NakedObjects.Rest.Model;
using NakedObjects.Rest.Snapshot.Representations;
using NakedObjects.Rest.Snapshot.Utility;

namespace NakedObjects.Rest.API {
    public static class ControllerHelpers {

        public static string DebugFilter(Func<string> msgFunc) => RestSnapshot.DebugFilter(msgFunc);

        public static ObjectContextFacade GetServiceByName(this IFrameworkFacade frameworkFacade, string serviceName) {
            var oid = frameworkFacade.OidTranslator.GetOidTranslation(serviceName);
            return frameworkFacade.GetService(oid);
        }

        public static IMenuFacade GetMenuByName(this IFrameworkFacade frameworkFacade, string menuName) {
            if (string.IsNullOrEmpty(menuName)) {
                throw new BadRequestNOSException();
            }

            var menu = frameworkFacade.GetMainMenus().List.SingleOrDefault(m => m.Id == menuName);
            return menu ?? throw new MenuResourceNotFoundNOSException(menuName);
        }

        public static ActionContextFacade GetServiceActionByName(this IFrameworkFacade frameworkFacade, string serviceName, string actionName) {
            var oid = frameworkFacade.OidTranslator.GetOidTranslation(serviceName);
            return frameworkFacade.GetServiceAction(oid, actionName);
        }

        public static ActionContextFacade GetMenuActionByName(this IFrameworkFacade frameworkFacade, string menuName, string actionName) {
            return frameworkFacade.GetMenuAction(menuName, actionName);
        }

        public static ObjectContextFacade GetObjectByName(this IFrameworkFacade frameworkFacade, string domainType, string instanceId) {
            var oidTranslation = frameworkFacade.OidTranslator.GetOidTranslation(domainType, instanceId);
            return frameworkFacade.GetObject(oidTranslation);
        }

        public static ActionContextFacade GetObjectActionByName(this IFrameworkFacade frameworkFacade, string domainType, string instanceId, string actionName) {
            var oidTranslation = frameworkFacade.OidTranslator.GetOidTranslation(domainType, instanceId);
            return frameworkFacade.GetObjectAction(oidTranslation, actionName);
        }

        public static PropertyContextFacade GetPropertyByName(this IFrameworkFacade frameworkFacade, string domainType, string instanceId, string propertyName, ArgumentsContextFacade argsContext) {
            var link = frameworkFacade.OidTranslator.GetOidTranslation(domainType, instanceId);
            var obj = frameworkFacade.GetObject(link);
            return frameworkFacade.GetPropertyWithCompletions(obj.Target, propertyName, argsContext);
        }

        public static PropertyContextFacade GetTransientPropertyByName(this IFrameworkFacade frameworkFacade, string domainType, string propertyName, ArgumentsContextFacade persistArgs, ArgumentsContextFacade promptArgs) {
            var obj = frameworkFacade.GetTransient(domainType, persistArgs);
            return frameworkFacade.GetPropertyWithCompletions(obj.Target, propertyName, promptArgs);
        }

        private static ParameterContextFacade GetParameterByName(this IFrameworkFacade frameworkFacade, ActionContextFacade action, string parmName) {
            var parm = action.VisibleParameters.Single(p => p.Id == parmName);
            parm.Target = action.Target;
            return parm;
        }

        public static ParameterContextFacade GetObjectParameterByName(this IFrameworkFacade frameworkFacade, string domainType, string instanceId, string actionName, string parmName, ArgumentsContextFacade argsContext) {
            var link = frameworkFacade.OidTranslator.GetOidTranslation(domainType, instanceId);
            var action = frameworkFacade.GetObjectActionWithCompletions(link, actionName, parmName, argsContext);
            return frameworkFacade.GetParameterByName(action, parmName);
        }

        public static ParameterContextFacade GetServiceParameterByName(this IFrameworkFacade frameworkFacade, string serviceName, string actionName, string parmName, ArgumentsContextFacade argsContext) {
            var link = frameworkFacade.OidTranslator.GetOidTranslation(serviceName);
            var action = frameworkFacade.GetServiceActionWithCompletions(link, actionName, parmName, argsContext);
            return frameworkFacade.GetParameterByName(action, parmName);
        }

        private static ObjectContextFacade ValidateObjectContext(ObjectContextFacade objectContext) {
            if (objectContext.VisibleProperties.Any(p => !string.IsNullOrEmpty(p.Reason))) {
                if (objectContext.VisibleProperties.Any(p => p.ErrorCause == Cause.WrongType)) {
                    throw new BadRequestNOSException("Bad Request", objectContext.VisibleProperties.Cast<ContextFacade>().ToList());
                }

                throw new BadArgumentsNOSException("Arguments invalid", objectContext.VisibleProperties.Cast<ContextFacade>().ToList());
            }

            if (!string.IsNullOrEmpty(objectContext.Reason)) {
                if (objectContext.ErrorCause == Cause.WrongType) {
                    throw new BadRequestNOSException("Bad Request", objectContext);
                }

                throw new BadArgumentsNOSException("Arguments invalid", objectContext);
            }

            return objectContext;
        }

        public static ObjectContextFacade PutObjectAndValidate(this IFrameworkFacade frameworkFacade, string domainType, string instanceId, ArgumentsContextFacade argsContext) {
            var link = frameworkFacade.OidTranslator.GetOidTranslation(domainType, instanceId);
            var context = frameworkFacade.PutObject(link, argsContext);
            return ValidateObjectContext(context);
        }

        private static ObjectContextFacade ValidatePersistObjectContext(ObjectContextFacade objectContext, RestControlFlags flags) {
            if (objectContext.VisibleProperties.Any(p => !string.IsNullOrEmpty(p.Reason)) || !string.IsNullOrEmpty(objectContext.Reason)) {
                throw new BadPersistArgumentsException("Arguments invalid", objectContext, objectContext.VisibleProperties.Cast<ContextFacade>().ToList(), flags);
            }

            return objectContext;
        }

        public static ObjectContextFacade PersistObjectAndValidate(this IFrameworkFacade frameworkFacade, string domainType, ArgumentsContextFacade argsContext, RestControlFlags flags) {
            var context = frameworkFacade.Persist(domainType, argsContext);
            return ValidatePersistObjectContext(context, flags);
        }

        public static PropertyContextFacade GetPropertyByName(this IFrameworkFacade frameworkFacade, string domainType, string instanceId, string propertyName) {
            var link = frameworkFacade.OidTranslator.GetOidTranslation(domainType, instanceId);
            var propertyContext = frameworkFacade.GetProperty(link, propertyName);

            // found but a collection 
            if (propertyContext.Property.IsCollection) {
                throw new PropertyResourceNotFoundNOSException(propertyName);
            }

            return propertyContext;
        }

        public static PropertyContextFacade GetCollectionPropertyByName(this IFrameworkFacade frameworkFacade, string domainType, string instanceId, string propertyName) {
            try {
                var link = frameworkFacade.OidTranslator.GetOidTranslation(domainType, instanceId);
                var propertyContext = frameworkFacade.GetProperty(link, propertyName);

                // found but not a collection 
                if (!propertyContext.Property.IsCollection) {
                    throw new CollectionResourceNotFoundNOSException(propertyName);
                }

                return propertyContext;
            }
            catch (PropertyResourceNotFoundNOSException e) {
                throw new CollectionResourceNotFoundNOSException(e.ResourceId);
            }
        }

        private static PropertyContextFacade ValidatePropertyContext(PropertyContextFacade propertyContext) {
            if (!string.IsNullOrEmpty(propertyContext.Reason)) {
                throw new BadArgumentsNOSException("Arguments invalid", propertyContext);
            }

            return propertyContext;
        }

        public static PropertyContextFacade PutPropertyAndValidate(this IFrameworkFacade frameworkFacade, string domainType, string instanceId, string propertyName, ArgumentContextFacade argContext) {
            var link = frameworkFacade.OidTranslator.GetOidTranslation(domainType, instanceId);
            var context = frameworkFacade.PutProperty(link, propertyName, argContext);
            return ValidatePropertyContext(context);
        }

        public static PropertyContextFacade DeletePropertyAndValidate(this IFrameworkFacade frameworkFacade, string domainType, string instanceId, string propertyName, ArgumentContextFacade argContext) {
            var link = frameworkFacade.OidTranslator.GetOidTranslation(domainType, instanceId);
            var context = frameworkFacade.DeleteProperty(link, propertyName, argContext);
            return ValidatePropertyContext(context);
        }

        private static ActionResultContextFacade ValidateActionResult(ActionResultContextFacade result) {
            if (result.ActionContext.VisibleProperties.Any(p => !string.IsNullOrEmpty(p.Reason))) {
                if (result.ActionContext.VisibleProperties.Any(p => p.ErrorCause == Cause.WrongType)) {
                    throw new BadRequestNOSException("Bad Request", result.ActionContext.VisibleProperties.Cast<ContextFacade>().ToList());
                }

                throw new BadArgumentsNOSException("Arguments invalid", result.ActionContext.VisibleProperties.Cast<ContextFacade>().ToList());
            }

            if (result.ActionContext.VisibleParameters.Any(p => !string.IsNullOrEmpty(p.Reason))) {
                if (result.ActionContext.VisibleParameters.Any(p => p.ErrorCause == Cause.WrongType)) {
                    throw new BadRequestNOSException("Bad Request", result.ActionContext.VisibleParameters.Cast<ContextFacade>().ToList());
                }

                throw new BadArgumentsNOSException("Arguments invalid", result.ActionContext.VisibleParameters.Cast<ContextFacade>().ToList());
            }

            if (!string.IsNullOrEmpty(result.Reason)) {
                if (result.ErrorCause == Cause.WrongType) {
                    throw new BadRequestNOSException("Bad Request", result.ActionContext);
                }

                throw new BadArgumentsNOSException("Arguments invalid", result);
            }

            return result;
        }

        public static ActionResultContextFacade ExecuteActionAndValidate(this IFrameworkFacade frameworkFacade, string domainType, string instanceId, string actionName, ArgumentsContextFacade argsContext) {
            var link = frameworkFacade.OidTranslator.GetOidTranslation(domainType, instanceId);
            var context = frameworkFacade.ExecuteObjectAction(link, actionName, argsContext);
            return ValidateActionResult(context);
        }

        public static ActionResultContextFacade ExecuteServiceActionAndValidate(this IFrameworkFacade frameworkFacade, string serviceName, string actionName, ArgumentsContextFacade argsContext) {
            var link = frameworkFacade.OidTranslator.GetOidTranslation(serviceName);
            var context = frameworkFacade.ExecuteServiceAction(link, actionName, argsContext);
            return ValidateActionResult(context);
        }

        public static ActionResultContextFacade ExecuteMenuActionAndValidate(this IFrameworkFacade frameworkFacade, string menuName, string actionName, ArgumentsContextFacade argsContext) {
            var context = frameworkFacade.ExecuteMenuAction(menuName, actionName, argsContext);
            return ValidateActionResult(context);
        }

        public static RestSnapshot SnapshotOrNoContent((Func<RestSnapshot> ss, bool validateOnly) t) => t.validateOnly ? throw new NoContentNOSException() : t.ss();

        public static MethodType GetExpectedMethodType(HttpMethod method) =>
            method switch {
                _ when method == HttpMethod.Get => MethodType.QueryOnly,
                _ when method == HttpMethod.Put => MethodType.Idempotent,
                _ => MethodType.NonIdempotent
            };

        public static void SetCaching(ResponseHeaders responseHeaders, RestSnapshot ss, (int, int, int) cacheSettings) {
            var (transactional, userInfo, nonExpiring) = cacheSettings;
            var cacheTime = ss.Representation.GetCaching() switch {
                CacheType.Transactional => transactional,
                CacheType.UserInfo => userInfo,
                CacheType.NonExpiring => nonExpiring,
                _ => 0
            };

            if (cacheTime == 0) {
                responseHeaders.CacheControl = new CacheControlHeaderValue {NoCache = true};
                responseHeaders.Append(HeaderNames.Pragma, "no-cache");
            }
            else {
                responseHeaders.CacheControl = new CacheControlHeaderValue {MaxAge = new TimeSpan(0, 0, 0, cacheTime)};
            }

            var now = DateTime.UtcNow;

            responseHeaders.Date = new DateTimeOffset(now);
            responseHeaders.Expires = new DateTimeOffset(now).Add(new TimeSpan(0, 0, 0, cacheTime));
        }

        public static void AppendWarningHeader(ResponseHeaders responseHeaders, string warning) {
            if (!string.IsNullOrWhiteSpace(warning)) {
                responseHeaders.Append(HeaderNames.Warning, warning);
            }
        }

        public static IDictionary<string, string> GetOptionalCapabilities() =>
            new Dictionary<string, string> {
                {"protoPersistentObjects", "yes"},
                {"deleteObjects", "no"},
                {"validateOnly", "yes"},
                {"domainModel", "simple"},
                {"blobsClobs", "attachments"},
                {"inlinedMemberRepresentations", "yes"}
            };

        public static T HandleMalformed<T>(Func<T> f) {
            try {
                return f();
            }
            catch (BadRequestNOSException) {
                throw;
            }
            catch (ResourceNotFoundNOSException) {
                throw;
            }
            catch (Exception) {
                throw new BadRequestNOSException("Malformed arguments");
            }
        }

        public static void ValidateDomainModel(string domainModel) {
            if (domainModel != null && domainModel != RestControlFlags.DomainModelType.Simple.ToString().ToLower() && domainModel != RestControlFlags.DomainModelType.Formal.ToString().ToLower()) {
                var msg = $"Invalid domainModel: {domainModel}";
                throw new ValidationException((int) HttpStatusCode.BadRequest, msg);
            }
        }

        public static void ValidateBinding(ModelStateDictionary modelState) {
            if (modelState.ErrorCount > 0) {
                throw new BadRequestNOSException("Malformed arguments");
            }
        }

        public static void RejectRequestIfReadOnly() {
            if (RestfulObjectsControllerBase.IsReadOnly) {
                var msg = DebugFilter(() => "In readonly mode");
                throw new ValidationException((int) HttpStatusCode.Forbidden, msg);
            }
        }

        public static void ValidateArguments(ArgumentMap arguments, bool errorIfNone = true) {
            if (arguments.IsMalformed) {
                var msg = $"Malformed arguments : {DebugFilter(() => arguments.MalformedReason)}";
                throw new BadRequestNOSException(msg);
            }

            if (!arguments.HasValue && errorIfNone) {
                throw new BadRequestNOSException("Missing arguments"); // todo i18n
            }
        }

        public static void ValidateArguments(SingleValueArgument arguments, bool errorIfNone = true) {
            if (arguments.IsMalformed) {
                var msg = $"Malformed arguments : {DebugFilter(() => arguments.MalformedReason)}";
                throw new BadRequestNOSException(msg);
            }

            if (!arguments.HasValue && errorIfNone) {
                throw new BadRequestNOSException("Missing arguments"); // todo i18n
            }
        }

        public static RestControlFlags GetFlags(RestfulObjectsControllerBase controller) =>
            RestControlFlags.FlagsFromArguments(controller.ValidateOnly,
                controller.Page,
                controller.PageSize,
                controller.DomainModel,
                RestfulObjectsControllerBase.InlineDetailsInActionMemberRepresentations,
                RestfulObjectsControllerBase.InlineDetailsInCollectionMemberRepresentations,
                controller.InlinePropertyDetails ?? RestfulObjectsControllerBase.InlineDetailsInPropertyMemberRepresentations,
                controller.InlineCollectionItems.HasValue && controller.InlineCollectionItems.Value,
                RestfulObjectsControllerBase.AllowMutatingActionOnImmutableObject);

        public static RestControlFlags GetFlags(Arguments arguments) {
            if (arguments.IsMalformed || arguments.ReservedArguments == null) {
                var msg = $"Malformed arguments : {DebugFilter(() => arguments.IsMalformed ? arguments.MalformedReason : "Reserved args = null")}"; // todo i18n
                throw new BadRequestNOSException(msg);
            }

            return GetFlagsFromArguments(arguments.ReservedArguments);
        }

        public static RestControlFlags GetFlagsFromArguments(ReservedArguments reservedArguments) =>
            RestControlFlags.FlagsFromArguments(reservedArguments.ValidateOnly,
                reservedArguments.Page,
                reservedArguments.PageSize,
                reservedArguments.DomainModel,
                RestfulObjectsControllerBase.InlineDetailsInActionMemberRepresentations,
                RestfulObjectsControllerBase.InlineDetailsInCollectionMemberRepresentations,
                reservedArguments.InlinePropertyDetails ?? RestfulObjectsControllerBase.InlineDetailsInPropertyMemberRepresentations,
                reservedArguments.InlineCollectionItems.HasValue && reservedArguments.InlineCollectionItems.Value,
                RestfulObjectsControllerBase.AllowMutatingActionOnImmutableObject);
    }
}