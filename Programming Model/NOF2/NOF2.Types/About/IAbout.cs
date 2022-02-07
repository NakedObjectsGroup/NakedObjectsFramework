// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

namespace NOF2.About {
    public interface IAbout {
        AboutTypeCodes TypeCode { get; }

        string Name { get; set; }
        string Description { get; set; }

        bool Visible { get; set; }

        bool Usable { get; set; }
        string UnusableReason { get; set; }
    }
}