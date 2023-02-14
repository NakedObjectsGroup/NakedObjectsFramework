using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using ROSI.Interfaces;

namespace ROSI.Records;

public record DomainObject(JObject Wrapped, EntityTagHeaderValue? Tag = null) : IHasExtensions, IHasLinks;