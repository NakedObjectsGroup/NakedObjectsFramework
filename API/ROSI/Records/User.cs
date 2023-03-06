using Newtonsoft.Json.Linq;
using ROSI.Interfaces;

namespace ROSI.Records;

public record User(JObject Wrapped, InvokeOptions Options) : IHasExtensions, IHasLinks;