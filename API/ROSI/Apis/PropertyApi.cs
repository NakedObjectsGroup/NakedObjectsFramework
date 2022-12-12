using Newtonsoft.Json.Linq;
using ROSI.Interfaces;

namespace ROSI.Apis;

public static class PropertyApi {
    public static T GetValue<T>(this IProperty propertyRepresentation) => propertyRepresentation.Wrapped["value"].Value<T>();
}