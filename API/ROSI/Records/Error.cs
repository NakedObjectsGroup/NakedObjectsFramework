using Newtonsoft.Json.Linq;
using ROSI.Interfaces;

namespace ROSI.Records;

public record Error(JObject Wrapped) : IHasExtensions, IHasLinks;