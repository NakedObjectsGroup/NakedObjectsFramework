using System;
using System.Linq;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Reflector.Spec;

namespace NakedObjects.Managers {
    public class MetamodelManager : IMetamodelManager {
        private readonly IMetamodel metamodel;

        public MetamodelManager(IMetamodel metamodel) {
            this.metamodel = metamodel;
        }

        public virtual INakedObjectSpecification[] AllSpecifications {
            get { return metamodel.AllSpecifications.Select(s => new NakedObjectSpecification(this, s)).Cast<INakedObjectSpecification>().ToArray(); }
        }

        public INakedObjectSpecification GetSpecification(Type type) {
            return new NakedObjectSpecification(this, metamodel.GetSpecification(type));
        }

        public INakedObjectSpecification GetSpecification(string name) {
            return new NakedObjectSpecification(this, metamodel.GetSpecification(name));
        }

        public INakedObjectSpecification GetSpecification(IIntrospectableSpecification spec) {
            return new NakedObjectSpecification(this, metamodel.GetSpecification(spec.Type));
        }
    }
}