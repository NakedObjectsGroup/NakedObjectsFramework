// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Runtime.Serialization;
using NakedObjects.Architecture.Menu;
using NakedObjects.Architecture.SpecImmutable;

namespace NakedObjects.Architecture.Component {
    public interface ISpecificationCache {
        ITypeSpecImmutable GetSpecification(string key);
        void Clear();
        ITypeSpecImmutable[] AllSpecifications();
        void Cache(string key, ITypeSpecImmutable spec);
        void Cache(IMenuImmutable mainMenu);
        IMenuImmutable[] MainMenus();
        void Serialize(string file);
        void Serialize(string file, IFormatter formatter);
    }

    // Copyright (c) Naked Objects Group Ltd.
}