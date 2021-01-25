using System;
using System.Collections.Generic;
using System.Linq;
using AW.Types;
using NakedFunctions;

namespace AW.Functions
{
    public static class EmailTemplate_Functions
    {

        public static string[] DeriveKeys(EmailTemplate em)
        {
            return new[] {
                em.To,
                em.From,
                em.Subject,
                em.Message,
                em.Status.ToString() };
        }

        public static EmailTemplate PopulateUsingKeys(EmailTemplate em, string[] keys) =>
            new EmailTemplate(keys[0], keys[1], keys[2], keys[3], (EmailStatus)Enum.Parse(typeof(EmailStatus), keys[4]));

        public static (EmailTemplate, IContext) Send(this EmailTemplate et, IContext context) =>
            (et with { Status = EmailStatus.Sent }, context.WithInformUser("Email sent"));

        public static IQueryable<string> AutoCompleteSubject(this EmailTemplate et, [MinLength(2)] string value)
        {
            var matchingNames = new List<string> { "Subject1", "Subject2", "Subject3" };
            return from p in matchingNames.AsQueryable() select p.Trim();
        }

    }
}
