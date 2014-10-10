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

        public INakedObjectActionParameter CreateParameter(INakedObjectActionParamPeer paramPeer, INakedObjectAction action, int index) {
            Assert.AssertNotNull(framework);
            var specification = paramPeer.Specification;

            if (specification.IsParseable) {
                return new NakedObjectActionParameterParseable(framework.Metamodel, index, action, paramPeer, framework.Manager, framework.Session, framework.Persistor);
            }
            if (specification.IsObject) {
                return new OneToOneActionParameterImpl(framework.Metamodel, index, action, paramPeer, framework.Manager, framework.Session, framework.Persistor);
            }
            if (specification.IsCollection) {
                return new OneToManyActionParameterImpl(framework.Metamodel, index, action, paramPeer, framework.Manager, framework.Session, framework.Persistor);
            }
            throw new UnknownTypeException(specification);
        }

        public INakedObjectAssociation CreateAssociation(INakedObjectAssociationPeer peer) {
            Assert.AssertNotNull(framework);
            if (peer.IsOneToOne) {
                return new OneToOneAssociationImpl(framework.Metamodel, peer, framework.Session, framework.LifecycleManager, framework.Manager, framework.Persistor);
            }
            if (peer.IsOneToMany) {
                return new OneToManyAssociationImpl(framework.Metamodel, peer, framework.Session, framework.LifecycleManager, framework.Manager, framework.Persistor);
            }
            throw new ReflectionException("Unknown peer type: " + peer);
        }

        public INakedObjectAction[] OrderActions(IOrderSet<INakedObjectActionPeer> order) {
            Assert.AssertNotNull(framework);
            var actions = new List<INakedObjectAction>();
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

        public INakedObjectAction[] OrderActions(IList<Tuple<string, string, IOrderSet<INakedObjectActionPeer>>> order) {
            Assert.AssertNotNull(framework);
            return order.Select(element => CreateNakedObjectActionSet(element.Item1, element.Item2, element.Item3)).Cast<INakedObjectAction>().ToArray();
        }

        private NakedObjectActionSet CreateNakedObjectActionSet(IOrderSet<INakedObjectActionPeer> orderSet) {
            return new NakedObjectActionSet(orderSet.GroupFullName.Replace(" ", ""), orderSet.GroupFullName, OrderActions(orderSet), framework.Services);
        }

        private NakedObjectActionSet CreateNakedObjectActionSet(string id, string name, IOrderSet<INakedObjectActionPeer> orderSet) {
            return new NakedObjectActionSet(id, name, OrderActions(orderSet), framework.Services);
        }

        private NakedObjectActionImpl CreateNakedObjectAction(INakedObjectActionPeer peer) {
            
            return new NakedObjectActionImpl(this, framework.Metamodel, framework.LifecycleManager, framework.Session, peer);
        }

        public INakedObjectAssociation CreateNakedObjectField(INakedObjectAssociationPeer peer) {
            Assert.AssertNotNull(framework);
            return CreateAssociation(peer);
        }
    }
}