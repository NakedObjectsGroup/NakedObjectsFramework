// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;

namespace NakedFunctions {
    /// <summary>
    ///     Tells framework that the property (typically a DateTime or an integer) represents
    ///     the version of the instance. This is so that the framework can test that the user is
    ///     making changes to the current version - given that the user view may be minutes, even
    ///     hours, old. This attribute may be applied to the same property that is also used
    ///     for the EntityFramework 'concurrency check' (which may be specified by the
    ///     System.ComponentModel.DataAnnotations.ConcurrencyCheck attribute, or using the
    ///     fluent data mapping API) - but these are two separate checks, and both are typically .
    ///     necessary in a multi-user system.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class VersionedAttribute : Attribute { }
}