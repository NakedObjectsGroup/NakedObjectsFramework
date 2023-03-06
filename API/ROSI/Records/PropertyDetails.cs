using Newtonsoft.Json.Linq;
using ROSI.Interfaces;

namespace ROSI.Records;

public record PropertyDetails(JObject Wrapped, IInvokeOptions Options) : IProperty, IHasChoices;