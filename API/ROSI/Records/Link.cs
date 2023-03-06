using Newtonsoft.Json.Linq;
using ROSI.Interfaces;

namespace ROSI.Records;

public record Link(JObject Wrapped, InvokeOptions Options) : IWrapped;