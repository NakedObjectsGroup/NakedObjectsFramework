using System;
using System.Collections.Generic;
using System.Linq;
using AW.Types;
using NakedFunctions;

namespace AdventureWorksFunctionalModel.Person
{
    public static class EmailTemplateFunctions
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

        public static EmailTemplate PopulateUsingKeys(EmailTemplate em, string[] keys)
        {
            return new EmailTemplate(keys[0], keys[1], keys[2], keys[3], (EmailStatus)Enum.Parse(typeof(EmailStatus), keys[4]));
        }


        public static (EmailTemplate, EmailTemplate) Send(this EmailTemplate et)
        {
            var et2 = et with { Status = EmailStatus.Sent };
            return (et2, et2);
        }

        public static IQueryable<string> AutoCompleteSubject(this EmailTemplate et, [Length(2)] string value)
        {
            var matchingNames = new List<string> { "Subject1", "Subject2", "Subject3" };
            return from p in matchingNames.AsQueryable() select p.Trim();
        }

    }
}
