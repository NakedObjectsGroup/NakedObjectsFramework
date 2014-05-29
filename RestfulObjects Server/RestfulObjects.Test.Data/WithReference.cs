// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.ComponentModel.DataAnnotations;
using System.Linq;
using NakedObjects;
using Do = NakedObjects.EagerlyAttribute.Do;

namespace RestfulObjects.Test.Data {
    public class WithReference {
        public IDomainObjectContainer Container { set; protected get; }


        [Key, Title]
        public virtual int Id { get; set; }


        public virtual MostSimple AReference { get; set; }

        [Optionally]
        public virtual MostSimple ANullReference { get; set; }

        [Disabled]
        public virtual MostSimple ADisabledReference { get; set; }

        [Hidden]
        [Optionally]
        public virtual MostSimple AHiddenReference { get; set; }


        public virtual MostSimple AChoicesReference { get; set; }

        public virtual MostSimple[] ChoicesAChoicesReference() {
            return Container.Instances<MostSimple>().Where(ms => ms.Id == 1 || ms.Id == 2).ToArray();
        }

        public virtual MostSimple AnAutoCompleteReference { get; set; }

        public virtual IQueryable<MostSimple> AutoCompleteAnAutoCompleteReference([MinLength(2)]string name) {
            return Container.Instances<MostSimple>().Where(ms => name.Contains(ms.Id.ToString()));
        }

        public virtual string Validate(MostSimple aReference, MostSimple aChoicesReference) {
            if (aReference != null && aReference.Id == 1 && aChoicesReference.Id == 2) {
                return "Cross validation failed";
            }
            return "";
        }

        public virtual MostSimple AConditionalChoicesReference { get; set; }

        public virtual MostSimple[] ChoicesAConditionalChoicesReference(MostSimple aReference) {
            return Container.Instances<MostSimple>().Where(ms => ms.Id != aReference.Id).ToArray();
        }

        [Eagerly(Do.Rendering)]
        public virtual MostSimple AnEagerReference { get; set; }
    }
}