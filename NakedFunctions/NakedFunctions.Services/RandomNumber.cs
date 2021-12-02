// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("NakedFunctions.Services.Test")]

namespace NakedFunctions.Services;

public class RandomNumber : IRandom {
    internal readonly uint U;
    internal readonly uint V;

    internal RandomNumber(uint u, uint v) {
        U = u;
        V = v;
        Value = CalculateValue(u, v);
    }

    public int Value { get; init; }

    public int ValueInRange(int max) => Value % max;

    //Result will be greater than or equal to the min specified, and less than the max
    public int ValueInRange(int min, int max) => min + ValueInRange(max - min);

    public IRandom Next() => new RandomNumber(NewU(), NewV());

    private uint NewU() => 36969 * (U & 65535) + (U >> 16);

    private uint NewV() => 18000 * (V & 65535) + (V >> 16);

    private static double CalculateDoubleValue(uint u, uint v) => ((u << 16) + v + 1.0) * 2.328306435454494e-10;

    private static int CalculateValue(uint u, uint v) => (int)(CalculateDoubleValue(u, v) * int.MaxValue);
}