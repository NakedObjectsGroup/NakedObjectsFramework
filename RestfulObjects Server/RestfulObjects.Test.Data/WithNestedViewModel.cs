using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NakedObjects;

namespace RestfulObjects.Test.Data {
    public class WithNestedViewModel : IViewModel {
        public IDomainObjectContainer Container { set; protected get; }

        [Key, Title, ConcurrencyCheck]
        public virtual int Id { get; set; }

        [Hidden]
        public string AggregateKey {
            get { return DeriveKeys().Aggregate("", (s, t) => s + " " + t); } 
        }

        public MostSimple AReference { get; set; }

        public WithReferenceViewModel AViewModelReference { get; set; }

        [NakedObjectsIgnore]
        public string[] DeriveKeys() {
            var keys = new List<string> {AReference.Id.ToString()};
            keys.AddRange(AViewModelReference.DeriveKeys());
            return keys.ToArray();
        }

        [NakedObjectsIgnore]
        public void PopulateUsingKeys(string[] keys) {
            int msId = int.Parse(keys[0]);
            string[] vmKey = keys.Skip(1).ToArray();

            Id = msId;

            AReference = Container.Instances<MostSimple>().FirstOrDefault(ms => ms.Id == msId);

            AViewModelReference = Container.NewViewModel<WithReferenceViewModel>();
            AViewModelReference.PopulateUsingKeys(vmKey);
        }
    }
}