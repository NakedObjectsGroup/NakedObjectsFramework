using ROSI.Interfaces;

namespace ROSI.Apis;

public static class ActionApi {
    public static bool HasInvokeLink(this IHasLinks actionRepresentation) => actionRepresentation.GetLinks().HasInvokeLink();
}