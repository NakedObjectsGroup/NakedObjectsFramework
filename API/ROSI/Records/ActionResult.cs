using Newtonsoft.Json.Linq;
using ROSI.Interfaces;

namespace ROSI.Records;

public record ActionResult(JObject Wrapped) : IHasExtensions, IHasLinks;