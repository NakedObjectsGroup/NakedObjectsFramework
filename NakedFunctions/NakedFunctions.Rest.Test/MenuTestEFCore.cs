// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NakedFramework.DependencyInjection.Extensions;
using NakedFramework.Persistor.EFCore.Extensions;
using NakedFunctions.Rest.Test.Data;

namespace NakedFunctions.Rest.Test {
    public class MenuTestEFCore : MenuTestEF6 {
        protected new Func<IConfiguration, DbContext>[] ContextInstallers => new Func<IConfiguration, DbContext>[] {
            config => {
                var context = new EFCoreMenuDbContext();
                context.Create();
                return context;
            }
        };

        protected virtual Action<EFCorePersistorOptions> EFCorePersistorOptions =>
            options => { options.ContextInstallers = ContextInstallers; };

        protected override Action<NakedCoreOptions> AddPersistor => builder => { builder.AddEFCorePersistor(EFCorePersistorOptions); };

        protected override void CleanUpDatabase() {
            new EFCoreMenuDbContext().Delete();
        }
    }
}