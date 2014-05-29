// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Core.Context;

namespace NakedObjects.Web.Mvc {
    public class NakedObjectsControllerFactory : DefaultControllerFactory {
        private IContainerInjector injector;

        private IContainerInjector Injector {
            get {
                if (injector == null) {
                    var servicesToInject = NakedObjectsContext.ObjectPersistor.GetServices().Select(no => no.Object).ToArray();
                    injector = NakedObjectsContext.Reflector.CreateContainerInjector(servicesToInject);
                }

                return injector;
            }
        }

        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType) {
            IController controller = base.GetControllerInstance(requestContext, controllerType);
            Injector.InitDomainObject(controller);
            return controller;
        }
    }
}