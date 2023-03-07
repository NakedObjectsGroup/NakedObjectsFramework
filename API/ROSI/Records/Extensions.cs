using Newtonsoft.Json.Linq;
using ROSI.Helpers;
using ROSI.Interfaces;

namespace ROSI.Records;

public record Extensions(JObject Wrapped, IInvokeOptions Options) {
    public IInvokeOptions Options { get; } = Options.Copy();
}