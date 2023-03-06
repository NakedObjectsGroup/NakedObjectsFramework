using Newtonsoft.Json.Linq;
using ROSI.Interfaces;

namespace ROSI.Records;

public record Menus(JObject Wrapped, InvokeOptions Options) : IHasExtensions, IHasLinks;