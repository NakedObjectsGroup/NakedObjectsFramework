using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using ROSI.Helpers;
using ROSI.Interfaces;

namespace ROSI.Records;

public record ActionResult(JObject Wrapped, IInvokeOptions Options, EntityTagHeaderValue? Tag) : IHasExtensions, IHasLinks {
    public IInvokeOptions Options { get; } = Options.Copy();
}