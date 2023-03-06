using Newtonsoft.Json.Linq;
using ROSI.Interfaces;

namespace ROSI.Records;

public record Error(JObject Wrapped, IInvokeOptions Options) : IHasExtensions, IHasLinks;