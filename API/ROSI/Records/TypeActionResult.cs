using Newtonsoft.Json.Linq;
using ROSI.Helpers;
using ROSI.Interfaces;

namespace ROSI.Records;

public record TypeActionResult(JObject Wrapped, IInvokeOptions Options) : IHasExtensions, IHasLinks {
    public IInvokeOptions Options { get; } = Options.Copy();
}