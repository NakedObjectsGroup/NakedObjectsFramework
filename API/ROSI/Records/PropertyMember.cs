using Newtonsoft.Json.Linq;
using ROSI.Helpers;
using ROSI.Interfaces;

namespace ROSI.Records;

public record PropertyMember(JObject Wrapped, IInvokeOptions Options) : IProperty {
    public IInvokeOptions Options { get; } = Options.Copy();
}