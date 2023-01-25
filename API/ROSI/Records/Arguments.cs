using Newtonsoft.Json.Linq;
using ROSI.Interfaces;

namespace ROSI.Records;

public record Arguments(JObject Wrapped) : IWrapped;