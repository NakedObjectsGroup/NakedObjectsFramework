using Newtonsoft.Json.Linq;

namespace ROSI.Records;

public record ActionResult(JObject Wrapped);