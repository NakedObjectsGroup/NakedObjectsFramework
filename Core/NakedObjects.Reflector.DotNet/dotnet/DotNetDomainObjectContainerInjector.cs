// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections.Generic;
using System.Linq;
using NakedObjects.Core.Reflect;
using NakedObjects.Core.Util;

namespace NakedObjects.Reflector.DotNet {
    public class DotNetDomainObjectContainerInjector : IContainerInjector {
        private object container;
        private bool initialized;
        private List<object> services;

        public INakedObjectsFramework Framework { private get; set; }
        public Type[] ServiceTypes { set; private get; }

        private List<object> Services {
            get {
                if (services == null) {
                    services = ServiceTypes.Select(Activator.CreateInstance).ToList();
                    services.Add(Framework);
                    services.ForEach(InitDomainObject);           
                }
                return services;
            }
        }

        #region IContainerInjector Members

        public void InitDomainObject(object obj) {
            Initialize();
            Assert.AssertNotNull("no container", container);
            Assert.AssertNotNull("no services", Services);
            Methods.InjectContainer(obj, container);
            Methods.InjectServices(obj, Services.ToArray());
        }

        public void InitInlineObject(object root, object inlineObject) {
            Initialize();
            Assert.AssertNotNull("no root object", root);
            Methods.InjectRoot(root, inlineObject);
        }

        #endregion

        private void Initialize() {
            if (!initialized) {
                Assert.AssertNotNull(Framework);
                container = new DotNetDomainObjectContainer(Framework);
                initialized = true;
            }
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}