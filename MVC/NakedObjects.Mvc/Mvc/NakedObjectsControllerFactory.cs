// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using System.Web.Mvc;
using System.Web.Routing;
using NakedObjects.Architecture.Reflect;

namespace NakedObjects.Web.Mvc {
    public class NakedObjectsControllerFactory : DefaultControllerFactory {
        private readonly IContainerInjector injector;

        public NakedObjectsControllerFactory(IContainerInjector injector) {
            this.injector = injector;
        }

        private IContainerInjector Injector {
            get {
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