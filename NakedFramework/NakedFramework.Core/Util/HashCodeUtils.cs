// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;

namespace NakedFramework.Core.Util {
    /// <summary>
    ///     Collected methods which allow easy implementation of <see cref="object.GetHashCode" />,
    ///     based on Josh Bloch's Effective Java ported to C#.
    /// </summary>
    /// <para>
    ///     Example use case:
    /// </para>
    /// <code>
    /// public override int GetHashCode() {
    ///     int result = HashCodeUtils.Seed;
    ///     //collect the contributions of various fields
    ///     result = HashCodeUtils.Hash(result, fPrimitive);
    ///     result = HashCodeUtils.Hash(result, fObject);
    ///     result = HashCodeUtils.Hash(result, fArray);
    ///     return result;
    /// }
    /// </code>
    /// <para>
    ///     http://www.javapractices.com/Topic28.cjp
    /// </para>
    public static class HashCodeUtils {
        private const int OddPrimeNumber = 37;

        /// <summary>
        ///     An initial value for a <c>HashCode</c>, to which is added
        ///     contributions from fields. Using a non-zero value decreases collisions of
        ///     <c>HashCode</c> values.
        /// </summary>
        private const int SeedConst = 23;

        public static int Seed => SeedConst;

        /// <summary>
        ///     <see cref="bool" />
        /// </summary>
        public static int Hash(int aSeed, bool aBoolean) => FirstTerm(aSeed) + (aBoolean ? 1 : 0);

        /// <summary>
        ///     <see cref="char" />
        /// </summary>
        public static int Hash(int aSeed, char aChar) => FirstTerm(aSeed) + aChar;

        /// <summary>
        ///     <see cref="int" />
        /// </summary>
        /// <para>
        ///     <see cref="byte" /> and <see cref="short" /> are handled by this method, through implicit conversion.
        /// </para>
        public static int Hash(int aSeed, int aInt) => FirstTerm(aSeed) + aInt;

        private static long URShift(long number, int bits) {
            if (number >= 0) {
                return number >> bits;
            }

            return (number >> bits) + (2L << ~bits);
        }

        /// <summary>
        ///     <see cref="long" />
        /// </summary>
        public static int Hash(int aSeed, long aLong) => FirstTerm(aSeed) + (int) (aLong ^ URShift(aLong, 32));

        /// <summary>
        ///     <see cref="float" />
        /// </summary>
        public static int Hash(int aSeed, float aFloat) => Hash(aSeed, BitConverter.DoubleToInt64Bits(aFloat));

        /// <summary>
        ///     <see cref="double" />
        /// </summary>
        public static int Hash(int aSeed, double aDouble) => Hash(aSeed, BitConverter.DoubleToInt64Bits(aDouble));

        /// <summary>
        ///     <c>aObject</c> is a possibly-null object field, and possibly an array.
        /// </summary>
        /// <para>
        ///     If <c>aObject</c> is an array, then each element may be a primitive or a possibly-null object.
        /// </para>
        public static int Hash(int aSeed, object aObject) {
            var result = aSeed;
            if (aObject == null) {
                result = Hash(result, 0);
            }
            else if (!IsArray(aObject)) {
                result = Hash(result, aObject.GetHashCode());
            }
            else {
                result = ((Array) aObject).Cast<object>().Aggregate(result, Hash);
            }

            return result;
        }

        private static int FirstTerm(int aSeed) => OddPrimeNumber * aSeed;

        private static bool IsArray(object aObject) => aObject.GetType().IsArray;
    }
}