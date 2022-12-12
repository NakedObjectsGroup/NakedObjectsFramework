using Newtonsoft.Json.Linq;
using ROSI.Records;

namespace ROSI.Apis;

public static class PropertyApi {
    public static T GetValue<T>(this Property propertyRepresentation) => propertyRepresentation.Wrapped["value"].Value<T>();
}