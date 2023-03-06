using Newtonsoft.Json.Linq;
using ROSI.Interfaces;

namespace ROSI.Records;

public record Parameters(JObject Wrapped, IInvokeOptions Options) : IHasExtensions, IHasLinks;