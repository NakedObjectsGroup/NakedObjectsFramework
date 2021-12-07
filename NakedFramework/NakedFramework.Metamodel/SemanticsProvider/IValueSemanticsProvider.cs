// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedFramework.Architecture.Adapter;

namespace NakedFramework.Metamodel.SemanticsProvider;

public interface IValueSemanticsProvider {
    bool IsImmutable { get; }

    void AddValueFacets();
}

public interface IValueSemanticsProvider<T> : IValueSemanticsProvider {
    T DefaultValue { get; }

    /// <summary>
    ///     Parses a string to an instance of the object
    /// </summary>
    /// <para>
    ///     Here the implementing class is acting as a factory for itself
    /// </para>
    /// <param name="entry"></param>
    object ParseTextEntry(string entry);

    /// <summary>
    ///     The title of the object
    /// </summary>
    string DisplayTitleOf(T obj);

    /// <summary>
    ///     The title of the object, with mask applied
    /// </summary>
    string TitleWithMaskOf(string mask, T obj);

    object Value(INakedObjectAdapter adapter, string format = null);
}