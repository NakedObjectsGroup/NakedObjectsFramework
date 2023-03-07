using Newtonsoft.Json.Linq;
using ROSI.Helpers;
using ROSI.Interfaces;

namespace ROSI.Records;

public record PropertyDetails(JObject Wrapped, IInvokeOptions Options) : IProperty, IHasChoices {
    public IInvokeOptions Options { get; } = Options.Copy();
}