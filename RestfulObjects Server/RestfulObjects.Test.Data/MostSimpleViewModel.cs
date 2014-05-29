using System.ComponentModel.DataAnnotations;
using System.Linq;
using NakedObjects;

namespace RestfulObjects.Test.Data {
    public class MostSimpleViewModel : IViewModel {
        public IDomainObjectContainer Container { set; protected get; }

        [Hidden]
        public string AggregateKey {
            get { return DeriveKeys().Aggregate("", (s, t) => s + " " + t); }
        }

        [Title, ConcurrencyCheck]
        public virtual int Id { get; set; }

        [NakedObjectsIgnore]
        public string[] DeriveKeys() {
            return new[] {Id.ToString()};
        }

        [NakedObjectsIgnore]
        public void PopulateUsingKeys(string[] keys) {
            Id =  int.Parse( keys.First());
        }
    }
}