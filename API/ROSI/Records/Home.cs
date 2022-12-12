using Newtonsoft.Json.Linq;
using ROSI.Interfaces;

namespace ROSI.Records;

public record Home(JObject Wrapped) : IHasExtensions;