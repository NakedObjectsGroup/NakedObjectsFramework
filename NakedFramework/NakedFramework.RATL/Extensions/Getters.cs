using ROSI.Apis;
using ROSI.Records;

namespace NakedFramework.RATL.Extensions;

public static class Getters {
    public static async Task<Parameter> GetParameter(this ActionMember? am, int index) {
        Assert.IsNotNull(am);
        return (await am.GetParameters()).Parameters().ToArray()[index].Value;
    }
}