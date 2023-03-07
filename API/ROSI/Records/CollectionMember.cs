using Newtonsoft.Json.Linq;
using ROSI.Helpers;
using ROSI.Interfaces;

namespace ROSI.Records;

public record CollectionMember(JObject Wrapped, IInvokeOptions Options) : ICollection {
    public IInvokeOptions Options { get; } = Options.Copy();
}