using Newtonsoft.Json.Linq;
using ROSI.Helpers;
using ROSI.Interfaces;

namespace ROSI.Records;

public record Prompt(JObject Wrapped, IInvokeOptions Options) : IHasChoices, IHasLinks, IHasExtensions {
    public IInvokeOptions Options { get; } = Options.Copy();
}