using Newtonsoft.Json.Linq;
using ROSI.Interfaces;

namespace ROSI.Records;

public record Services(JObject Wrapped) : IHasExtensions, IHasLinks;