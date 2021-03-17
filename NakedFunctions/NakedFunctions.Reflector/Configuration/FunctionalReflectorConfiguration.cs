﻿// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using NakedFramework.Architecture.Configuration;

namespace NakedFunctions.Reflector.Configuration {
    public class FunctionalReflectorConfiguration : IFunctionalReflectorConfiguration {
        public FunctionalReflectorConfiguration(Type[] types,
                                                Type[] functions,
                                                bool concurrencyChecking = true) {
            Types = types;
            Functions = functions;
            ConcurrencyChecking = concurrencyChecking;
            IgnoreCase = false;
        }

        public bool HasConfig() => Types?.Any() == true || Functions?.Any() == true;

        #region IFunctionalReflectorConfiguration Members

        public Type[] Types { get; }
        public Type[] Functions { get; }

        public static Type[] Services => Array.Empty<Type>();
        public bool ConcurrencyChecking { get; }
        public bool IgnoreCase { get; }

        #endregion
    }
}