// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using System.Linq;
using Common.Logging;
using Microsoft.Extensions.Logging;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Core.Util;

namespace NakedObjects.Core.Spec {
    public class SpecFactory {
        private static readonly ILog Log = LogManager.GetLogger(typeof(SpecFactory));

        private INakedObjectsFramework framework;
        private ILoggerFactory loggerFactory;

        public void Initialize(INakedObjectsFramework newFramework, ILoggerFactory newLoggerFactory) {
            Assert.AssertNotNull(newFramework);
            framework = newFramework;
            loggerFactory = newLoggerFactory;
        }

        public IActionParameterSpec CreateParameter(IActionParameterSpecImmutable parameterSpecImmutable, IActionSpec actionSpec, int index) {
            Assert.AssertNotNull(framework);
            var specification = parameterSpecImmutable.Specification;
            return specification switch {
                _ when specification.IsParseable => new ActionParseableParameterSpec(framework.MetamodelManager, index, actionSpec, parameterSpecImmutable, framework.NakedObjectManager, framework.Session, framework.Persistor),
                _ when specification.IsObject => new OneToOneActionParameter(framework.MetamodelManager, index, actionSpec, parameterSpecImmutable, framework.NakedObjectManager, framework.Session, framework.Persistor),
                _ when specification.IsCollection => new OneToManyActionParameter(framework.MetamodelManager, index, actionSpec, parameterSpecImmutable, framework.NakedObjectManager, framework.Session, framework.Persistor),
                _ => throw new UnknownTypeException(Log.LogAndReturn($"{specification}"))
            };
        }

        public IAssociationSpec CreateAssociation(IAssociationSpecImmutable specImmutable) {
            Assert.AssertNotNull(framework);
            return specImmutable switch {
                IOneToOneAssociationSpecImmutable oneToOneAssociationSpecImmutable => new OneToOneAssociationSpec(framework.MetamodelManager, oneToOneAssociationSpecImmutable, framework.Session, framework.LifecycleManager, framework.NakedObjectManager, framework.Persistor, framework.TransactionManager),
                IOneToManyAssociationSpecImmutable oneToManyAssociationSpecImmutable => new OneToManyAssociationSpec(framework.MetamodelManager, oneToManyAssociationSpecImmutable, framework.Session, framework.LifecycleManager, framework.NakedObjectManager, framework.Persistor),
                _ => throw new ReflectionException(Log.LogAndReturn($"Unknown spec type: {specImmutable}"))
            };
        }

        public IActionSpec[] CreateActionSpecs(IList<IActionSpecImmutable> specImmutables) => specImmutables.Select(CreateActionSpec).ToArray();

        public IActionSpec CreateActionSpec(IActionSpecImmutable specImmutable) {
            Assert.AssertNotNull(framework);
            return new ActionSpec(this, framework.MetamodelManager, framework.LifecycleManager, framework.Session, framework.ServicesManager, framework.NakedObjectManager, specImmutable, framework.MessageBroker, framework.TransactionManager, loggerFactory);
        }

        public IAssociationSpec CreateAssociationSpec(IAssociationSpecImmutable specImmutable) {
            Assert.AssertNotNull(framework);
            return CreateAssociation(specImmutable);
        }

        public ITypeSpec CreateTypeSpec(ITypeSpecImmutable specImmutable) =>
            specImmutable switch {
                IObjectSpecImmutable osi => CreateObjectSpec(osi),
                IServiceSpecImmutable ssi => CreateServiceSpec(ssi),
                _ => throw new InitialisationException(Log.LogAndReturn($"Unexpected Spec Type {specImmutable.Type}"))
            };

        private IServiceSpec CreateServiceSpec(IServiceSpecImmutable specImmutable) {
            Assert.AssertNotNull(framework);
            return new ServiceSpec(this, framework.MetamodelManager, framework.NakedObjectManager, specImmutable);
        }

        private IObjectSpec CreateObjectSpec(IObjectSpecImmutable specImmutable) {
            Assert.AssertNotNull(framework);
            return new ObjectSpec(this, framework.MetamodelManager, framework.NakedObjectManager, specImmutable);
        }
    }
}