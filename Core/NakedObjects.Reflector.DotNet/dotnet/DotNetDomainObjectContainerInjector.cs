// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Collections.Generic;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Core.Util;

namespace NakedObjects.Reflector.DotNet {
    public class DotNetDomainObjectContainerInjector : IContainerInjector {
        private readonly object container;
        private readonly List<object> services = new List<object>();

        public DotNetDomainObjectContainerInjector(INakedObjectsFramework framework, object[] services) {
            container = new DotNetDomainObjectContainer(framework);
            this.services.AddRange(services);
            this.services.Add(framework);
        }

        #region IContainerInjector Members

        public void InitDomainObject(object obj) {
            Assert.AssertNotNull("no container", container);
            Assert.AssertNotNull("no services", services);
            Methods.InjectContainer(obj, container);
            Methods.InjectServices(obj, services.ToArray());
        }

        public void InitInlineObject(object root, object inlineObject) {
            Assert.AssertNotNull("no root object", root);
            Methods.InjectRoot(root, inlineObject);
        }

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}