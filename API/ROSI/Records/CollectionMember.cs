using Newtonsoft.Json.Linq;
using ROSI.Interfaces;

namespace ROSI.Records;

public record CollectionMember(JObject Wrapped) : ICollection;