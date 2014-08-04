// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using System.Collections.Generic;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Resolve;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core.Persist;

namespace NakedObjects.Testing {
    public class ProgrammableTestSystem {
        private readonly ProgrammableReflector reflector;
        private readonly IDictionary<object, INakedObject> adapters = new Dictionary<object, INakedObject>();
        private ProgrammableContext context;
        private int nextId;

        public ProgrammableTestSystem() {
            reflector = new ProgrammableReflector(this);
            context = new ProgrammableContext(this);
        }

        public INakedObject AdapterFor(Object obj) {
            return AdapterFor(obj, Events.InitializeTransientEvent);
        }

        public INakedObject AdapterFor(Object obj, IResolveEvent resolveEvent) {
            if (adapters.ContainsKey(obj)) {
                return adapters[obj];
            } else {
                IOid oid = SerialOid.CreatePersistent(reflector, nextId++, obj.GetType().FullName);
                INakedObject adapterFor = new ProgrammableNakedObject(obj, reflector.LoadSpecification(obj.GetType()), oid);
                adapterFor.ResolveState.Handle(resolveEvent);
                adapters[obj] = adapterFor;
                return adapterFor;
            }
        }

        public INakedObjectSpecification SpecificationFor(Type type) {
            return reflector.LoadSpecification(type);
        }
    }
}
