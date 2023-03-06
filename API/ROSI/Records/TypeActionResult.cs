using Newtonsoft.Json.Linq;
using ROSI.Interfaces;

namespace ROSI.Records;

public record TypeActionResult(JObject Wrapped, IInvokeOptions Options) : IHasExtensions, IHasLinks;