// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using System.Linq;
using NakedObjects.Architecture;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Core.Util;

namespace NakedObjects.Core.Spec {
    public class SpecFactory {
        private INakedObjectsFramework framework;

// ReSharper disable once ParameterHidesMember
        public void Initialize(INakedObjectsFramework framework) {
            Assert.AssertNotNull(framework);
            this.framework = framework;
        }

        public IActionParameterSpec CreateParameter(IActionParameterSpecImmutable parameterSpecImmutable, IActionSpec actionSpec, int index) {
            Assert.AssertNotNull(framework);
            IObjectSpecImmutable specification = parameterSpecImmutable.Specification;

            if (specification.IsParseable) {
                return new ActionParseableParameterSpec(framework.MetamodelManager, index, actionSpec, parameterSpecImmutable, framework.NakedObjectManager, framework.Session, framework.Persistor);
            }
            if (specification.IsObject) {
                return new OneToOneActionParameter(framework.MetamodelManager, index, actionSpec, parameterSpecImmutable, framework.NakedObjectManager, framework.Session, framework.Persistor);
            }
            if (specification.IsCollection) {
                return new OneToManyActionParameter(framework.MetamodelManager, index, actionSpec, parameterSpecImmutable, framework.NakedObjectManager, framework.Session, framework.Persistor);
            }
            throw new UnknownTypeException(specification);
        }

        public IAssociationSpec CreateAssociation(IAssociationSpecImmutable specImmutable) {
            Assert.AssertNotNull(framework);
            if (specImmutable.IsOneToOne) {
                return new OneToOneAssociationSpec(framework.MetamodelManager, specImmutable, framework.Session, framework.LifecycleManager, framework.NakedObjectManager, framework.Persistor, framework.TransactionManager);
            }
            if (specImmutable.IsOneToMany) {
                return new OneToManyAssociationSpec(framework.MetamodelManager, specImmutable, framework.Session, framework.LifecycleManager, framework.NakedObjectManager, framework.Persistor);
            }
            throw new ReflectionException("Unknown peer type: " + specImmutable);
        }

        //TODO: rename to CreateActionSpecs?
        public IActionSpec[] OrderActions(IList<IActionSpecImmutable> order) {
            Assert.AssertNotNull(framework);
            return order.Select(CreateActionSpec).Cast<IActionSpec>().ToArray();
        }

        public ActionSpec CreateActionSpec(IActionSpecImmutable specImmutable) {
            return new ActionSpec(this, framework.MetamodelManager, framework.LifecycleManager, framework.Session, framework.ServicesManager, framework.NakedObjectManager, specImmutable);
        }

        public IAssociationSpec CreateAssociationSpec(IAssociationSpecImmutable specImmutable) {
            Assert.AssertNotNull(framework);
            return CreateAssociation(specImmutable);
        }
    }
}