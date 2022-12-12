using Newtonsoft.Json.Linq;
using ROSI.Interfaces;

namespace ROSI.Records;

public record Action(JObject Wrapped) : IMember;
