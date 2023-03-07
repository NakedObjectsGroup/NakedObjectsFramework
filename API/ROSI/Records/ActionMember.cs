using Newtonsoft.Json.Linq;
using ROSI.Helpers;
using ROSI.Interfaces;

namespace ROSI.Records;

public record ActionMember(JObject Wrapped, IInvokeOptions Options) : IAction {
    public IInvokeOptions Options { get; } = Options.Copy();
}