// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedObjects.Architecture.Reflect;

namespace NakedObjects.Architecture.SpecImmutable {
    public interface ITypeSpecBuilder : ITypeSpecImmutable {
        /// <summary>
        ///     Discovers what attributes and behaviour the type specified by this specification. As specification are
        ///     cyclic (specifically a class will reference its subclasses, which in turn reference their superclass)
        ///     they need be created first, and then later work out its internals. This allows for cyclic references to
        ///     the be accommodated as there should always a specification available even though it might not be
        ///     complete.
        /// </summary>
        void Introspect(IFacetDecoratorSet decorator, IIntrospector introspector);

        void AddSubclass(ITypeSpecImmutable subclass);
    }
}