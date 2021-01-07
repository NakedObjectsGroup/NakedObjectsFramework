// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Security.Principal;

namespace NakedFunctions {
    public static class ContextExtensions {
        public static IContext WithWarnUser(this IContext context, string message)
            => context.WithAction((IAlert ua) => ua.WarnUser(message));

        public static IContext WithInformUser(this IContext context, string message)
            => context.WithAction((IAlert ua) => ua.InformUser(message));

        public static DateTime Now(this IContext context)
            => context.GetService<IClock>().Now();

        public static DateTime Today(this IContext context)
            => context.GetService<IClock>().Today();

        public static IPrincipal CurrentUser(this IContext context)
            => context.GetService<IPrincipalProvider>().CurrentUser;

        public static IRandom RandomSeed(this IContext context)
            => context.GetService<IRandomSeedGenerator>().Random;

        public static Guid NewGuid(this IContext context)
            => context.GetService<IGuidGenerator>().NewGuid();
    }
}