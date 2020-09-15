using System.ComponentModel.DataAnnotations;

namespace NakedFunctions.Rest.Test.Data {
    public record SimpleRecord {
        [Key]
        public int Id { get; init; }
        public string Name { get; init; }
    }
}