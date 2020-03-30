// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Linq;
using NakedObjects.Facade;
using NakedObjects.Facade.Contexts;
using NakedObjects.Rest.Snapshot.Utility;

namespace NakedObjects.Rest.API {
    public static class ControllerHelpers {
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
    }
}