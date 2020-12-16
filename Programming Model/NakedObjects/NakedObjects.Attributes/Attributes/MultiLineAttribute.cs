// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;

namespace NakedObjects {
    /// <summary>
    ///     Specifies that a <see cref="string" /> property or action parameter may contain carriage returns, and an optional default number of lines and width,
    ///     which may be used by the display.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Parameter, Inherited = true, AllowMultiple = false)]
    public class MultiLineAttribute : Attribute {
        public MultiLineAttribute() : this(6,0)
        {
        }

        public MultiLineAttribute(int numberOfLines) : this(numberOfLines, 0)
        {
        }

        public MultiLineAttribute(int numberOfLines, int width)
        {
            NumberOfLines = numberOfLines;
            Width = width;
        }

        public int NumberOfLines { get; set; }

        public int Width { get; set; }
    }
}