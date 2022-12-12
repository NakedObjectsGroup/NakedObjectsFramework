using Newtonsoft.Json.Linq;
using ROSI.Interfaces;

namespace ROSI.Records;

public record Property(JObject Wrapped) : IHasExtensions, IHasLinks;