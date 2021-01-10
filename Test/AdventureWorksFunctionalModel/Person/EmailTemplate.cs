using System;
using System.Collections.Generic;
using System.Linq;
using NakedFunctions;

namespace AW.Types
{
    [ViewModelEdit]
    public record EmailTemplate 
    {

        public EmailTemplate(
            string to,
            string from,
            string subject,
            string message,
            EmailStatus status)
        {
            To = to;
            From = from;
            Subject = subject;
            Message = message;
            Status = status;
        }

        public EmailTemplate() { }


        [MemberOrder(10), Optionally]
        public virtual string To { get; init; }

        [MemberOrder(20)]
        public virtual string From { get; init; }

        [MemberOrder(30)]
        public virtual string Subject { get; init; }

        [MemberOrder(40)]
        public virtual string Message { get; init; }

        
        public virtual EmailStatus Status { get; init; }

        public override string ToString() => $"{Status} email";
    }

    public static class EmailTemplateFunctions {

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
            return new EmailTemplate(keys[0],keys[1],keys[2],keys[3],(EmailStatus)Enum.Parse(typeof(EmailStatus), keys[4]));
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
    public enum EmailStatus
    {
        New, Sent, Failed
    }
}
