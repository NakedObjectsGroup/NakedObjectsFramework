// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using NakedObjects.Value;
using RestfulObjects.Test.Data;

namespace NakedObjects.Rest.Test.App {
    public class NakedObjectsRunSettings {
        private static Type[] AllTypes =>
            Assembly.GetAssembly(typeof(CodeFirstContext)).GetTypes().
                     Where(t => t.IsPublic).
                     Where(t => t.Namespace == "RestfulObjects.Test.Data").
                     ToArray();


        public static Type[] Types => AllTypes;

        public static Type[] Services {
            get {
                return new[] {
                    typeof(RestDataRepository),
                    typeof(WithActionService),
                    typeof(ContributorService),
                    typeof(TestTypeCodeMapper)
                };
            }
        }

        public static string[] NameSpaces => Types.Select(t => t.Namespace).Distinct().ToArray();

        public static Func<IConfiguration, DbContext> DbContextInstaller => c => {
            var cs = c.GetConnectionString("RestTest");
            return cs.Contains("(localdb)") ? (DbContext) new CodeFirstContextLocal(cs) : new CodeFirstContext(cs);
        };
    }
}