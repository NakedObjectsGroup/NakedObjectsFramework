using Newtonsoft.Json.Linq;
using ROSI.Interfaces;

namespace ROSI.Records;

public record DomainObject(JObject Wrapped) : IHasExtensions, IHasLinks;