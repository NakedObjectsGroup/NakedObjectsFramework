using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NakedObjects;
using Do = NakedObjects.EagerlyAttribute.Do;

namespace RestfulObjects.Test.Data {
    public class WithReferenceViewModel : IViewModel {
        public IDomainObjectContainer Container { set; protected get; }

        [Hidden]
        public string AggregateKey {
            get { return DeriveKeys().Aggregate("", (s, t) => s + " " + t); } 
        }

        [NakedObjectsIgnore]
        public string[] DeriveKeys() {
            var keys = new List<string> {
                AReference.Id.ToString(),
                ADisabledReference.Id.ToString(),
                AHiddenReference.Id.ToString(),
                AChoicesReference.Id.ToString()
            };

            return keys.ToArray();
        }

        [NakedObjectsIgnore]
        public void PopulateUsingKeys(string[] keys) {
           

            int rId = int.Parse(keys[0]);
            int drId = int.Parse(keys[1]);
            int hrId = int.Parse(keys[2]);
            int crId = int.Parse(keys[3]);

            Id = rId;

            AReference = Container.Instances<MostSimple>().FirstOrDefault(ms => ms.Id == rId);
            ADisabledReference = Container.Instances<MostSimple>().FirstOrDefault(vs => vs.Id == drId);
            AHiddenReference = Container.Instances<MostSimple>().FirstOrDefault(vs => vs.Id == hrId);
            AChoicesReference = Container.Instances<MostSimple>().FirstOrDefault(vs => vs.Id == crId);
            AnEagerReference = Container.Instances<MostSimple>().FirstOrDefault(ms => ms.Id == rId);
        }


        [Key, Title, ConcurrencyCheck]
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

        public virtual string Validate(MostSimple aReference, MostSimple aChoicesReference) {
            if (aReference != null && aReference.Id == 1 && aChoicesReference.Id == 2) {
                return "Cross validation failed";
            }
            return "";
        }

        [Eagerly(Do.Rendering)]
        public virtual MostSimple AnEagerReference { get; set; }
    }
}