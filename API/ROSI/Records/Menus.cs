using Newtonsoft.Json.Linq;
using ROSI.Interfaces;

namespace ROSI.Records;

public record Menus(JObject Wrapped, IInvokeOptions Options) : IHasExtensions, IHasLinks;