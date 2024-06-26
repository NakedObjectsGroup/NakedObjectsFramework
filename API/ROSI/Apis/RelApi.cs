﻿using System.Text.RegularExpressions;

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
            var regex = new Regex(@"urn:org.restfulobjects:rels\/([\w-]*)");
            rel = regex.Match(rel).Groups.Values.Last().Value;
        }

        return Enum.Parse<Rels>(rel.Replace('-', '_'));
    }

    public static string GetId(string rel, string idType) {
        var regex = new Regex($"{idType}=\"([\\w.]*)\"");
        return regex.Match(rel).Groups.Values.Last().Value;
    }

    public static string GetServiceId(this string rel) => GetId(rel, "serviceId");

    public static string GetMenuId(this string rel) => GetId(rel, "menuId");

    public static string GetTypeAction(this string rel) => GetId(rel, "typeaction");

    public static string GetAction(this string rel) => GetId(rel, "action");
}