using Newtonsoft.Json.Linq;
using ROSI.Interfaces;

namespace ROSI.Records;

public record ActionMember(JObject Wrapped) : IAction;