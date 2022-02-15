// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NOF2.About;

namespace NOF2.Reflector.Component;

public class FieldAboutImpl : FieldAbout {
    public FieldAboutImpl(AboutTypeCodes typeCode) => TypeCode = typeCode;

    public AboutTypeCodes TypeCode { get; }

    public bool Usable { get; set; } = true;
    public bool Visible { get; set; } = true;

    public bool IsPersistent { get; set; }
    public object[] Options { get; set; } = { };
    public string Name { get; set; }
    public string Description { get; set; }
    public string UnusableReason { get; set; }

    public bool IsValid { get; set; } = true;
    public string InvalidReason { get; set; }
}