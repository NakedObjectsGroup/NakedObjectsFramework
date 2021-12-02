// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Linq;
using System.Runtime.Serialization;
using NakedFramework.Facade.Translation;
using NakedFramework.Rest.Snapshot.Constants;
using NakedFramework.Rest.Snapshot.RelTypes;
using NakedFramework.Rest.Snapshot.Utility;

namespace NakedFramework.Rest.Snapshot.Representation;

[DataContract]
public class LinkRepresentation : RefValueRepresentation {
    protected LinkRepresentation(IOidStrategy oidStrategy, RelType relType, RestControlFlags flags)
        : base(oidStrategy, relType, flags) {
        SetScalars(relType);
    }

    [DataMember(Name = JsonPropertyNames.Rel)]
    public string Rel { get; set; }

    [DataMember(Name = JsonPropertyNames.Method)]
    public string Method { get; set; }

    [DataMember(Name = JsonPropertyNames.Type)]
    public string Type { get; set; }

    private void SetScalars(RelType relType) {
        Rel = relType.Name;
        Method = relType.Method.ToString().ToUpper();
        Type = relType.GetMediaType(Flags).ToString();
    }

    public static LinkRepresentation Create(IOidStrategy oidStrategy, RelType relType, RestControlFlags flags, params OptionalProperty[] properties) =>
        properties.Any()
            ? CreateWithOptionals<LinkRepresentation>(new object[] { oidStrategy, relType, flags }, properties)
            : new LinkRepresentation(oidStrategy, relType, flags);
}