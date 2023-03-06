using Newtonsoft.Json.Linq;
using ROSI.Interfaces;

namespace ROSI.Records;

public record ActionDetails(JObject Wrapped, IInvokeOptions Options) : IHasExtensions, IHasLinks;