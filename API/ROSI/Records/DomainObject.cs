using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using ROSI.Interfaces;

namespace ROSI.Records;

public record DomainObject(JObject Wrapped, IInvokeOptions Options, EntityTagHeaderValue? Tag = null) : IHasExtensions, IHasLinks;