using Newtonsoft.Json.Linq;
using ROSI.Interfaces;

namespace ROSI.Records;

public record Parameter(JObject Wrapped) : IHasExtensions, IHasLinks, IHasChoices;