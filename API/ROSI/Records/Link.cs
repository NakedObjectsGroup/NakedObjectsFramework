using Newtonsoft.Json.Linq;
using ROSI.Helpers;
using ROSI.Interfaces;

namespace ROSI.Records;

public record Link(JObject Wrapped, IInvokeOptions Options) : IWrapped {
    public IInvokeOptions Options { get; } = Options.Copy();
}