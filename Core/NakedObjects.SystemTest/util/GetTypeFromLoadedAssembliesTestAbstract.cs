// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local

namespace NakedObjects.SystemTest.Util {
    public class GetTypeFromLoadedAssembliesTestAbstract {
        private static readonly IList<string> MasterTypeList = new List<string>();
        private static readonly IDictionary<string, Runs> Results = new Dictionary<string, Runs>();

        private static ModuleBuilder CreateModuleBuilder(string name) {
            //AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName {Name = name}, AssemblyBuilderAccess.Run);
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName {Name = name}, AssemblyBuilderAccess.Run);

            return assemblyBuilder.DefineDynamicModule($"{name}Module");
        }

        private static void AddClass(ModuleBuilder moduleBuilder, string name) {
            var typeBuilder = moduleBuilder.DefineType(name, TypeAttributes.Public);
            var constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, null);
            var ilGenerator = constructorBuilder.GetILGenerator();

            ilGenerator.EmitWriteLine($"{name} instantiated!");
            ilGenerator.Emit(OpCodes.Ret);

            typeBuilder.CreateType();
        }

        // randomizing code from stack overflow !
        private static IList<T> Shuffle<T>(IList<T> list) {
            var shuffled = list.Select(i => i).ToList();
            var n = shuffled.Count;
            while (n > 1) {
                n--;
                var k = ThreadSafeRandom.ThisThreadsRandom.Next(n + 1);
                var value = shuffled[k];
                shuffled[k] = shuffled[n];
                shuffled[n] = value;
            }

            return shuffled;
        }

        private static IList<T> RandomSelection<T>(IList<T> list) {
            var randomSelection = list.Select(i => i).ToList();
            var n = randomSelection.Count;
            while (n > 1) {
                n--;
                var k = ThreadSafeRandom.ThisThreadsRandom.Next(n + 1);
                var value = list[k];
                randomSelection[n] = value;
            }

            return randomSelection;
        }

        public static void SetupTypeData() {
            for (var i = 0; i < 100; i++) {
                var mb = CreateModuleBuilder($"Assembly{i}");

                for (var j = 0; j < 100; j++) {
                    var classname = $"Class{i.ToString(CultureInfo.InvariantCulture)}{j}";

                    AddClass(mb, classname);

                    MasterTypeList.Add(classname);
                }
            }

            Results.Clear();
        }

        private static void DisplayResults() {
            foreach (var (key, value) in Results) {
                var indRuns = value.IndividualRuns.Select(ts => ts.ToString()).Aggregate("", (s, t) => s + (string.IsNullOrEmpty(s) ? "\t" : "\r\n\t\t") + t);
                var shortName = key.Replace("TestHarnessFindTypeFromLoadedAssemblies", "");

                Console.WriteLine("Name: {0}\t\tTotal : {1}\r\n\tRuns :{2}", shortName, value.TotalRun, indRuns);
            }
        }

        public static void OutputCsv(string name) {
            var fileName = name + DateTime.Now.Ticks;

            const string dir = @"C:\LoadAssemblyTestRuns";
            var filePath = $@"{dir}\{fileName}.csv";

            Directory.CreateDirectory(dir);
            using var fs = File.Create(filePath);
            using var sw = new StreamWriter(fs);
            const string header = "Test, Total Time, Time Run 1,Time Run 2,Time Run 3,Time Run 4,Time Run 5,Time Run 6,Time Run 7,Time Run 8,Time Run 9,Time Run 10";

            sw.WriteLine(header);

            foreach (var result in Results) {
                var indRuns = result.Value.IndividualRuns.Select(ts => ts.ToString()).Aggregate("", (s, t) => s + (string.IsNullOrEmpty(s) ? "" : ",") + t);
                var shortName = result.Key.Replace("TestHarnessFindTypeFromLoadedAssemblies", "");

                var line = $"{shortName}, {result.Value.TotalRun}, {indRuns}";
                sw.WriteLine(line);
            }
        }

        private static void CollateResults(string testName, Runs runs) {
            lock (Results) {
                Results[testName] = runs;
            }

            DisplayResults();
        }

        private static void CollateResults(string testName, Runs[] runs) {
            lock (Results) {
                for (var i = 0; i < runs.Length; i++) {
                    var name = $"{testName}x{i}";
                    Results[name] = runs[i];
                }
            }

            DisplayResults();
        }

        private static long FindTypeFromLoadedAssemblies(Func<string, Type> funcUnderTest, IList<string> typeList) {
            var sw = new Stopwatch();

            foreach (var s in typeList) {
                sw.Start();
                var t = funcUnderTest(s);
                sw.Stop();
                Assert.IsNotNull(t);
                Assert.AreEqual(s, t.FullName);
            }

            return sw.ElapsedMilliseconds;
        }

        private static string GetCurrentMethod() {
            var st = new StackTrace();
            var sf = st.GetFrame(1);
            return sf.GetMethod().Name;
        }

        private static long FindTypeFromLoadedAssembliesOnce(Func<string, Type> funcUnderTest, IList<string> typeList) => FindTypeFromLoadedAssemblies(funcUnderTest, typeList);

        private static Runs FindTypeFromLoadedAssembliesTenTimes(Func<string, Type> funcUnderTest, IList<string> typeList) {
            var totalElapsed = 0L;
            var indRuns = new BlockingCollection<long>();

            for (var i = 0; i < 10; i++) {
                var elapsed = FindTypeFromLoadedAssemblies(funcUnderTest, typeList);
                totalElapsed += elapsed;
                indRuns.Add(elapsed);
            }

            return new Runs {IndividualRuns = indRuns.ToArray(), TotalRun = totalElapsed};
        }

        private static Task<long> CreateTask(Func<string, Type> funcUnderTest, IList<string> typeList, BlockingCollection<long> indRuns) {
            return Task<long>.Factory.StartNew(() => {
                var elapsed = FindTypeFromLoadedAssemblies(funcUnderTest, typeList);
                indRuns.Add(elapsed);
                return elapsed;
            });
        }

        private static Runs FindTypeFromLoadedAssembliesInParallel(Func<string, Type> funcUnderTest, IList<string>[] typeLists) {
            var indRuns = new BlockingCollection<long>();

            var tasks = typeLists.Select(list => CreateTask(funcUnderTest, list, indRuns)).ToArray();

            var sw = new Stopwatch();

            sw.Start();
            Task.WaitAll(tasks);
            sw.Stop();

            return new Runs {IndividualRuns = indRuns.ToArray(), TotalRun = sw.ElapsedMilliseconds};
        }

        private static Runs[] FindTypeFromLoadedAssembliesInParallelTenTimes(Func<string, Type> funcUnderTest, IList<string>[] typeLists) {
            var runsList = new List<Runs>();
            for (var i = 0; i < 10; i++) {
                runsList.Add(FindTypeFromLoadedAssembliesInParallel(funcUnderTest, typeLists));
            }

            return runsList.ToArray();
        }

        #region Nested type: Runs

        private class Runs {
            public long[] IndividualRuns { get; set; }
            public long TotalRun { get; set; }
        }

        #endregion

        #region Nested type: ThreadSafeRandom

        private static class ThreadSafeRandom {
            [ThreadStatic] private static Random local;

            public static Random ThisThreadsRandom => local ??= new Random(unchecked(Environment.TickCount * 31 + Thread.CurrentThread.ManagedThreadId));
        }

        #endregion

        #region tests

        public void TestHarnessFindTypeFromLoadedAssembliesOnce(Func<string, Type> funcUnderTest) {
            // find each type in order
            var elapsed = FindTypeFromLoadedAssembliesOnce(funcUnderTest, MasterTypeList);
            CollateResults(GetCurrentMethod(), new Runs {IndividualRuns = new[] {elapsed}, TotalRun = elapsed});
        }

        public void TestHarnessFindTypeFromLoadedAssembliesOnceRandomOrder(Func<string, Type> funcUnderTest) {
            // find each type in random order
            var randomList = Shuffle(MasterTypeList);
            var elapsed = FindTypeFromLoadedAssembliesOnce(funcUnderTest, randomList);
            CollateResults(GetCurrentMethod(), new Runs {IndividualRuns = new[] {elapsed}, TotalRun = elapsed});
        }

        public void TestHarnessFindTypeFromLoadedAssembliesOnceRandomSelection(Func<string, Type> funcUnderTest) {
            // find a random selection of types 
            var randomList = RandomSelection(MasterTypeList);
            var elapsed = FindTypeFromLoadedAssembliesOnce(funcUnderTest, randomList);
            CollateResults(GetCurrentMethod(), new Runs {IndividualRuns = new[] {elapsed}, TotalRun = elapsed});
        }

        public void TestHarnessFindTypeFromLoadedAssembliesTenTimes(Func<string, Type> funcUnderTest) {
            var runs = FindTypeFromLoadedAssembliesTenTimes(funcUnderTest, MasterTypeList);
            CollateResults(GetCurrentMethod(), runs);
        }

        public void TestHarnessFindTypeFromLoadedAssembliesTenTimesRandomOrder(Func<string, Type> funcUnderTest) {
            var randomList = Shuffle(MasterTypeList);
            var runs = FindTypeFromLoadedAssembliesTenTimes(funcUnderTest, randomList);
            CollateResults(GetCurrentMethod(), runs);
        }

        public void TestHarnessFindTypeFromLoadedAssembliesTenTimesRandomSelection(Func<string, Type> funcUnderTest) {
            var randomList = RandomSelection(MasterTypeList);
            var runs = FindTypeFromLoadedAssembliesTenTimes(funcUnderTest, randomList);
            CollateResults(GetCurrentMethod(), runs);
        }

        public void TestHarnessFindTypeFromLoadedAssembliesInParallel(Func<string, Type> funcUnderTest) {
            var runs = FindTypeFromLoadedAssembliesInParallel(funcUnderTest, Enumerable.Repeat(MasterTypeList, 10).ToArray());
            CollateResults(GetCurrentMethod(), runs);
        }

        public void TestHarnessFindTypeFromLoadedAssembliesInParallelRandomOrder(Func<string, Type> funcUnderTest) {
            var randomList = Shuffle(MasterTypeList);
            var runs = FindTypeFromLoadedAssembliesInParallel(funcUnderTest, Enumerable.Repeat(randomList, 10).ToArray());
            CollateResults(GetCurrentMethod(), runs);
        }

        public void TestHarnessFindTypeFromLoadedAssembliesInParallelRandomSelection(Func<string, Type> funcUnderTest) {
            var randomList = RandomSelection(MasterTypeList);
            var runs = FindTypeFromLoadedAssembliesInParallel(funcUnderTest, Enumerable.Repeat(randomList, 10).ToArray());
            CollateResults(GetCurrentMethod(), runs);
        }

        public void TestHarnessFindTypeFromLoadedAssembliesInParallelMultiRandomOrder(Func<string, Type> funcUnderTest) {
            var randomLists = Enumerable.Repeat(MasterTypeList, 10).Select(Shuffle).ToArray();
            var runs = FindTypeFromLoadedAssembliesInParallel(funcUnderTest, randomLists);
            CollateResults(GetCurrentMethod(), runs);
        }

        public void TestHarnessFindTypeFromLoadedAssembliesInParallelMultiRandomSelection(Func<string, Type> funcUnderTest) {
            var randomLists = Enumerable.Repeat(MasterTypeList, 10).Select(RandomSelection).ToArray();
            var runs = FindTypeFromLoadedAssembliesInParallel(funcUnderTest, randomLists);
            CollateResults(GetCurrentMethod(), runs);
        }

        public void TestHarnessFindTypeFromLoadedAssembliesInParallelTenTimes(Func<string, Type> funcUnderTest) {
            var runs = FindTypeFromLoadedAssembliesInParallelTenTimes(funcUnderTest, Enumerable.Repeat(MasterTypeList, 10).ToArray());
            CollateResults(GetCurrentMethod(), runs);
        }

        public void TestHarnessFindTypeFromLoadedAssembliesInParallelRandomOrderTenTimes(Func<string, Type> funcUnderTest) {
            var randomList = Shuffle(MasterTypeList);
            var runs = FindTypeFromLoadedAssembliesInParallelTenTimes(funcUnderTest, Enumerable.Repeat(randomList, 10).ToArray());
            CollateResults(GetCurrentMethod(), runs);
        }

        public void TestHarnessFindTypeFromLoadedAssembliesInParallelRandomSelectionTenTimes(Func<string, Type> funcUnderTest) {
            var randomList = RandomSelection(MasterTypeList);
            var runs = FindTypeFromLoadedAssembliesInParallelTenTimes(funcUnderTest, Enumerable.Repeat(randomList, 10).ToArray());
            CollateResults(GetCurrentMethod(), runs);
        }

        public void TestHarnessFindTypeFromLoadedAssembliesInParallelMultiRandomOrderTenTimes(Func<string, Type> funcUnderTest) {
            var randomLists = Enumerable.Repeat(MasterTypeList, 10).Select(Shuffle).ToArray();
            var runs = FindTypeFromLoadedAssembliesInParallelTenTimes(funcUnderTest, randomLists);
            CollateResults(GetCurrentMethod(), runs);
        }

        public void TestHarnessFindTypeFromLoadedAssembliesInParallelMultiRandomSelectionTenTimes(Func<string, Type> funcUnderTest) {
            var randomLists = Enumerable.Repeat(MasterTypeList, 10).Select(RandomSelection).ToArray();
            var runs = FindTypeFromLoadedAssembliesInParallelTenTimes(funcUnderTest, randomLists);
            CollateResults(GetCurrentMethod(), runs);
        }

        #endregion
    }
}