using Newtonsoft.Json.Linq;
using ROSI.Interfaces;

namespace ROSI.Records;

public record Argument(JObject Wrapped, InvokeOptions Options) : IWrapped;