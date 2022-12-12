using Newtonsoft.Json.Linq;
using ROSI.Interfaces;
using ROSI.Records;

namespace ROSI.Apis;

public static class PropertyApi {
    public static T GetValue<T>(this IProperty propertyRepresentation) => propertyRepresentation.Wrapped["value"].Value<T>();

    public static Link GetLinkValue(this IProperty propertyRepresentation) => new(propertyRepresentation.Wrapped["value"] as JObject);
}