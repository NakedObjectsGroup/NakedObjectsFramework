using Newtonsoft.Json.Linq;
using ROSI.Interfaces;

namespace ROSI.Records;

public record Parameters(JObject Wrapped, InvokeOptions Options) : IHasExtensions, IHasLinks;