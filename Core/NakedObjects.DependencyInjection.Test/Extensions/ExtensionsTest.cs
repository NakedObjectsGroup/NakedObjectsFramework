// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.DependencyInjection.Extensions;

namespace NakedObjects.DependencyInjection.Test.Extensions {
    [TestClass]
    public class ExtensionsTest {
        [TestMethod]
        public void TestFramework() {
            IServiceCollection services = new ServiceCollection();
            services.AddNakedFramework(options => { });
        }

        [TestMethod]
        public void TestNakedObjects() {
            IServiceCollection services = new ServiceCollection();
            services.AddNakedFramework(options => { options.AddNakedObjects(options => { options.Types = Array.Empty<Type>(); }); });
        }

        [TestMethod]
        public void TestNakedFunctions() {
            IServiceCollection services = new ServiceCollection();
            services.AddNakedFramework(options => { options.AddNakedFunctions(options => { options.Functions = Array.Empty<Type>(); }); });
        }

        [TestMethod]
        public void TestRestfulObjects()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddNakedFramework(options => { options.AddRestfulObjects(options => { options.AcceptHeaderStrict = true; }); });
        }

        [TestMethod]
        public void TestAll() {
            IServiceCollection services = new ServiceCollection();
            services.AddNakedFramework(options => {
                options.AddNakedObjects(options => { options.Types = Array.Empty<Type>(); });
                options.AddNakedFunctions(options => { options.Functions = Array.Empty<Type>(); });
                options.AddRestfulObjects(options => { options.AcceptHeaderStrict = true; });
            });
        }
    }
}