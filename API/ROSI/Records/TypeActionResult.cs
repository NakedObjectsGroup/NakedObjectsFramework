using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using ROSI.Interfaces;

namespace ROSI.Records;

public record TypeActionResult(JObject Wrapped) : IHasExtensions, IHasLinks;