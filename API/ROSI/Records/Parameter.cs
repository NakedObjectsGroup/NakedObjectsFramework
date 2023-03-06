using Newtonsoft.Json.Linq;
using ROSI.Interfaces;

namespace ROSI.Records;

public record Parameter(JObject Wrapped, InvokeOptions Options) : IHasExtensions, IHasLinks, IHasChoices;