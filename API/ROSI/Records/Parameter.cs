using Newtonsoft.Json.Linq;
using ROSI.Helpers;
using ROSI.Interfaces;

namespace ROSI.Records;

public record Parameter(JObject Wrapped, IInvokeOptions Options) : IHasExtensions, IHasLinks, IHasChoices {
    public IInvokeOptions Options { get; } = Options.Copy();
}