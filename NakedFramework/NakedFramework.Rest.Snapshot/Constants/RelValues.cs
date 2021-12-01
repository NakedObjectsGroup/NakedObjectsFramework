// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

namespace NakedFramework.Rest.Snapshot.Constants; 

public class RelValues {
    // IANA values 
    public const string Help = "help";
    public const string Icon = "icon";
    public const string Next = "next"; // unused ?
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
    public const string Element = Prfx + "element";
    public const string ElementType = Prfx + "element-type";
    public const string Invoke = Prfx + "invoke";
    public const string Modify = Prfx + "modify";
    public const string Persist = Prfx + "persist";
    public const string Property = Prfx + "property";
    public const string RemoveFrom = Prfx + "remove-from";
    public const string Service = Prfx + "service";
    public const string Services = Prfx + "services";
    public const string Update = Prfx + "update";
    public const string User = Prfx + "user";
    public const string Value = Prfx + "value";
    public const string Version = Prfx + "version";

    // custom extension 
    public const string Menus = Prfx + "menus";
    public const string Menu = Prfx + "menu";
}