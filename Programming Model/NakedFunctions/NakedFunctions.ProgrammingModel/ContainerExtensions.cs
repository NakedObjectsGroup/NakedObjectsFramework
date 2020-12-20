using System;
using System.Security.Principal;

namespace NakedFunctions
{
    public static class ContainerExtensions
    {
        public static IContainer WithWarnUser(this IContainer container, string message) => container.WithOutput((IAlert ua) => ua.WarnUser(message));

        public static IContainer WithInformUser(this IContainer container, string message) => container.WithOutput((IAlert ua) => ua.InformUser(message));

        public static DateTime Now(this IContainer container) => container.GetService<IClock>().Now();

        public static DateTime Today(this IContainer container) => container.GetService<IClock>().Today();

        public static IPrincipal CurrentUser(this IContainer container) => container.GetService<IPrincipalProvider>().CurrentUser;

        public static IRandom RandomSeed(this IContainer container) => container.GetService<IRandomSeedGenerator>().Random;

        public static Guid NewGuid(this IContainer container) => container.GetService<IGuidGenerator>().NewGuid();
    }
}