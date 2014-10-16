// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Core.Util;
using NakedObjects.Reflector.DotNet.Facets.Ordering;
using NakedObjects.Reflector.Peer;
using NakedObjects.Reflector.Spec;

namespace NakedObjects.Core.spec {
    public class MemberFactory {
        private  INakedObjectsFramework framework;

        public void Initialize(INakedObjectsFramework framework) {
            this.framework = framework;
        }

        public IActionParameterSpec CreateParameter(IActionParameterSpecImmutable parameterSpecImmutable, IActionSpec actionSpec, int index) {
            Assert.AssertNotNull(framework);
            var specification = parameterSpecImmutable.Specification;

            if (specification.IsParseable) {
                return new ActionParseableParameterSpec(framework.Metamodel, index, actionSpec, parameterSpecImmutable, framework.Manager, framework.Session, framework.Persistor);
            }
            if (specification.IsObject) {
                return new OneToOneActionParameterImpl(framework.Metamodel, index, actionSpec, parameterSpecImmutable, framework.Manager, framework.Session, framework.Persistor);
            }
            if (specification.IsCollection) {
                return new OneToManyActionParameterImpl(framework.Metamodel, index, actionSpec, parameterSpecImmutable, framework.Manager, framework.Session, framework.Persistor);
            }
            throw new UnknownTypeException(specification);
        }

        public IAssociationSpec CreateAssociation(IAssociationSpecImmutable specImmutable) {
            Assert.AssertNotNull(framework);
            if (specImmutable.IsOneToOne) {
                return new OneToOneAssociationSpec(framework.Metamodel, specImmutable, framework.Session, framework.LifecycleManager, framework.Manager, framework.Persistor, framework.TransactionManager);
            }
            if (specImmutable.IsOneToMany) {
                return new OneToManyAssociationSpec(framework.Metamodel, specImmutable, framework.Session, framework.LifecycleManager, framework.Manager, framework.Persistor);
            }
            throw new ReflectionException("Unknown peer type: " + specImmutable);
        }

        public IActionSpec[] OrderActions(IOrderSet<IActionSpecImmutable> order) {
            Assert.AssertNotNull(framework);
            var actions = new List<IActionSpec>();
            foreach (var element in order) {
                if (element.Peer != null) {
                    actions.Add(CreateNakedObjectAction(element.Peer));
                }
                else if (element.Set != null) {
                    actions.Add(CreateNakedObjectActionSet(element.Set));
                }
                else {
                    throw new UnknownTypeException(element);
                }
            }

            return actions.ToArray();
        }

        public IActionSpec[] OrderActions(IList<Tuple<string, string, IOrderSet<IActionSpecImmutable>>> order) {
            Assert.AssertNotNull(framework);
            return order.Select(element => CreateNakedObjectActionSet(element.Item1, element.Item2, element.Item3)).Cast<IActionSpec>().ToArray();
        }

        private ActionSpecSet CreateNakedObjectActionSet(IOrderSet<IActionSpecImmutable> orderSet) {
            return new ActionSpecSet(orderSet.GroupFullName.Replace(" ", ""), orderSet.GroupFullName, OrderActions(orderSet), framework.Services);
        }

        private ActionSpecSet CreateNakedObjectActionSet(string id, string name, IOrderSet<IActionSpecImmutable> orderSet) {
            return new ActionSpecSet(id, name, OrderActions(orderSet), framework.Services);
        }

        private ActionSpec CreateNakedObjectAction(IActionSpecImmutable specImmutable) {
            
            return new ActionSpec(this, framework.Metamodel, framework.LifecycleManager, framework.Session, framework.Services, framework.TransactionManager, framework.Manager, specImmutable);
        }

        public IAssociationSpec CreateNakedObjectField(IAssociationSpecImmutable specImmutable) {
            Assert.AssertNotNull(framework);
            return CreateAssociation(specImmutable);
        }
    }
}