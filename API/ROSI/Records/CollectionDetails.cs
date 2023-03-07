using Newtonsoft.Json.Linq;
using ROSI.Helpers;
using ROSI.Interfaces;

namespace ROSI.Records;

public record CollectionDetails(JObject Wrapped, IInvokeOptions Options) : IHasValue, ICollection {
    public IInvokeOptions Options { get; } = Options.Copy();
}