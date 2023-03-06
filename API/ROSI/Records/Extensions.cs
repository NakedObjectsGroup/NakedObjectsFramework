using Newtonsoft.Json.Linq;
using ROSI.Interfaces;

namespace ROSI.Records;

public record Extensions(JObject Wrapped, IInvokeOptions Options);