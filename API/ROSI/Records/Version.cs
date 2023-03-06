using Newtonsoft.Json.Linq;
using ROSI.Interfaces;

namespace ROSI.Records;

public record Version(JObject Wrapped, IInvokeOptions Options) : IHasExtensions, IHasLinks;