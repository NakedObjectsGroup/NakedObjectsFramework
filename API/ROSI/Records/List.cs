using Newtonsoft.Json.Linq;
using ROSI.Helpers;
using ROSI.Interfaces;

namespace ROSI.Records;

public record List(JObject Wrapped, IInvokeOptions Options) : IHasValue, IHasExtensions, IHasLinks {
    public IInvokeOptions Options { get; } = Options.Copy();
}