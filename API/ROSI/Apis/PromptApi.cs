using ROSI.Helpers;
using ROSI.Records;

namespace ROSI.Apis;

public static class PromptApi {
    public static string GetId(this Prompt promptRepresentation) => promptRepresentation.GetMandatoryProperty(JsonConstants.Id).ToString();
}