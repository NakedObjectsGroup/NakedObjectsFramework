// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

namespace RestfulObjects.Snapshot.Constants {
    public class RelValues {
        // IANA values 
        public const string DescribedBy = "describedby";
        public const string Help = "help";
        public const string Icon = "icon";
        public const string Next = "next"; // unused ?
        public const string Previous = "previous"; // unused ?
        public const string Self = "self";
        public const string Up = "up";

        private const string Prfx = "urn:org.restfulobjects:rels/";
        // RO values 
        public const string Action = Prfx + "action";
        public const string ActionParam = Prfx + "action-param";
        public const string AddTo = Prfx + "add-to";
        public const string Attachment = Prfx + "attachment";
        public const string Prompt = Prfx + "prompt";
        public const string Choice = Prfx + "choice";
        public const string Clear = Prfx + "clear";
        public const string Collection = Prfx + "collection";
        public const string CollectionValue = Prfx + "collection-value";
        public const string Default = Prfx + "default";
        public const string Delete = Prfx + "delete";
        public const string Details = Prfx + "details";
        public const string DomainType = Prfx + "domain-type";
        public const string DomainTypes = Prfx + "domain-types";
        public const string Element = Prfx + "element";
        public const string ElementType = Prfx + "element-type";
        public const string Invoke = Prfx + "invoke";
        public const string Modify = Prfx + "modify";
        public const string Persist = Prfx + "persist";
        public const string Property = Prfx + "property";
        public const string RemoveFrom = Prfx + "remove-from";
        public const string ReturnType = Prfx + "return-type";
        public const string Service = Prfx + "service";
        public const string Services = Prfx + "services";
        public const string Update = Prfx + "update";
        public const string User = Prfx + "user";
        public const string Value = Prfx + "value";
        public const string Version = Prfx + "version";
    }
}