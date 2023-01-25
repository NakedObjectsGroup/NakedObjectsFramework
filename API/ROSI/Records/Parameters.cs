using Newtonsoft.Json.Linq;
using ROSI.Interfaces;

namespace ROSI.Records;

public record Parameters(JObject Wrapped) : IHasExtensions, IHasLinks;