using Newtonsoft.Json.Linq;
using ROSI.Interfaces;

namespace ROSI.Records;

public record Error(JObject Wrapped, InvokeOptions Options) : IHasExtensions, IHasLinks;