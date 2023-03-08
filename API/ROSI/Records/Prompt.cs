using Newtonsoft.Json.Linq;
using ROSI.Interfaces;

namespace ROSI.Records;

public record Prompt(JObject Wrapped, InvokeOptions Options) : IHasChoices, IHasLinks, IHasExtensions;