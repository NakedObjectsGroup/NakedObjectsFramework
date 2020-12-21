using System;
using System.Security.Principal;

namespace NakedFunctions
{
    public static class ContextExtensions
    {
        public static IContext WithWarnUser(this IContext context, string message) => context.WithOutput((IAlert ua) => ua.WarnUser(message));

        public static IContext WithInformUser(this IContext context, string message) => context.WithOutput((IAlert ua) => ua.InformUser(message));

        public static DateTime Now(this IContext context) => context.GetService<IClock>().Now();

        public static DateTime Today(this IContext context) => context.GetService<IClock>().Today();

        public static IPrincipal CurrentUser(this IContext context) => context.GetService<IPrincipalProvider>().CurrentUser;

        public static IRandom RandomSeed(this IContext context) => context.GetService<IRandomSeedGenerator>().Random;

        public static Guid NewGuid(this IContext context) => context.GetService<IGuidGenerator>().NewGuid();
    }
}