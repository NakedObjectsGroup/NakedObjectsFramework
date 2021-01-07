// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;

namespace NakedFunctions {
    /// <summary>
    ///     Provides a statically-defined default value for an action parameter.
    ///     A DefaultValue of type Int may be applied to a DateTime parameter, where
    ///     the value 0 means 'today', -1 means 'yesterday', 30 means '30 days from today'.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class DefaultValueAttribute : Attribute {
        public DefaultValueAttribute(bool value) => Value = value;

        public DefaultValueAttribute(byte value) => Value = value;

        public DefaultValueAttribute(char value) => Value = value;

        public DefaultValueAttribute(double value) => Value = value;

        public DefaultValueAttribute(float value) => Value = value;

        public DefaultValueAttribute(int value) => Value = value;

        public DefaultValueAttribute(long value) => Value = value;

        public DefaultValueAttribute(short value) => Value = value;

        public DefaultValueAttribute(string value) => Value = value;

        public object Value { get; }
    }
}