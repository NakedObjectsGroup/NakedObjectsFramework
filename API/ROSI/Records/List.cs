using Newtonsoft.Json.Linq;
using ROSI.Interfaces;

namespace ROSI.Records;

public record List(JObject Wrapped, IInvokeOptions Options) : IHasValue, IHasExtensions, IHasLinks;