using Newtonsoft.Json.Linq;
using ROSI.Interfaces;

namespace ROSI.Records;

public record PropertyMember(JObject Wrapped, IInvokeOptions Options) : IProperty;