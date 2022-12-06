using System.Text.RegularExpressions;

namespace ROSI.Apis;

public static class RelApi {
    public enum Rels {
        //IANA
        describedby, 
        help, 
        icon, 
        previous,
        next, 
        self, 
        up,

        // RO
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
        menus,
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

    public static Rels GetRelType(this string rel) {
        if (rel.Contains("urn:org.restfulobjects:rels")) {
            var toMatch = rel.Split(';').First();
            var regex = new Regex(@"urn:org.restfulobjects:rels\/(\w*)");
            rel = regex.Match(rel).Groups.Values.Last().Value;
        }

        return Enum.Parse<Rels>(rel.Replace('-', '_'));
    }
}