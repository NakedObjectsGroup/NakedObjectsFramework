// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;

namespace NakedObjects; 

/// <summary>
///     This attribute indicates that transient instances of this class may be created but may not be
///     persisted, or that properties within a class are not persisted.
///     Attempting to persist such an object programmatically would throw an exception.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
[Obsolete("Use EF6/EFCore")]
public class NotPersistedAttribute : Attribute { }