// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Configuration;
using NakedObjects.Core.Container;
using NakedObjects.Core.Util;

namespace NakedObjects.Core.Component {
    public sealed class DomainObjectContainerInjector : IDomainObjectInjector {
        private readonly ILoggerFactory loggerFactory;
        private readonly List<Type> serviceTypes;
        private IDomainObjectContainer container;
        private bool initialized;
        private List<object> services;

        public DomainObjectContainerInjector(IReflectorConfiguration config,
                                             ILoggerFactory loggerFactory,
                                             ILogger<DomainObjectContainerInjector> logger) {
            this.loggerFactory = loggerFactory;
            Assert.AssertNotNull(config);

            serviceTypes = config.Services.ToList();
        }

        private List<object> Services => services ?? SetServices();

        #region IDomainObjectInjector Members

        public INakedObjectsFramework Framework { private get; set; }

        public void InjectInto(object obj) {
            Initialize();
            Assert.AssertNotNull("no container", container);
            Assert.AssertNotNull("no services", Services);
            Methods.InjectContainer(obj, container);
            Methods.InjectServices(obj, Services.ToArray());
            Methods.InjectLogger(obj, loggerFactory);
        }

        public void InjectIntoInline(object root, object inlineObject) {
            Initialize();
            Assert.AssertNotNull("no root object", root);
            Methods.InjectRoot(root, inlineObject);
        }

        #endregion

        private List<object> SetServices() {
            services = serviceTypes.Select(Activator.CreateInstance).ToList();
            services.Add(Framework);
            services.ForEach(InjectInto);
            return services;
        }

        private void Initialize() {
            if (!initialized) {
                Assert.AssertNotNull(Framework);
                container = new DomainObjectContainer(Framework, loggerFactory.CreateLogger<DomainObjectContainer>());
                initialized = true;
            }
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}