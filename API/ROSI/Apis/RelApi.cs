using System.Text.RegularExpressions;

namespace ROSI.Apis;

public static class RelApi
{
    public enum Rels
    {
        action,
        action_param,
        add_to,
        attachment,
        choice,
        clear,
        collection,
        collection_value,
        @default,
        delete,
        details,
        domain_type,
        domain_types,
        element,
        element_type,
        invoke,
        modify,
        persist,
        prompt,
        property,
        remove_from,
        return_type,
        service,
        services,
        update,
        user,
        value,
        version
    }

    public static Rels GetRelType(this string rel)
    {
        var regex = new Regex(@"urn:org.restfulobjects:rels\/(\w*);.*");
        var relType = regex.Match(rel).Groups.Values.Last().Value;

        return Enum.Parse<Rels>(relType.Replace('-', '_'));
    }
}