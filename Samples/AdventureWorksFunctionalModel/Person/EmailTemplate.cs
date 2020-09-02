using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NakedObjects;
using NakedFunctions;

namespace AdventureWorksModel
{
    [ViewModelEdit]
    public class EmailTemplate 
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
        public virtual string To { get; set; }

        [MemberOrder(20), Optionally]
        public virtual string From { get; set; }

        [MemberOrder(30), Optionally]
        public virtual string Subject { get; set; }

        [MemberOrder(40), Optionally]
        public virtual string Message { get; set; }

        [Disabled]
        public virtual EmailStatus Status { get; set; }
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

        public static string Title(this EmailTemplate et)
        {
            return et.CreateTitle($"{((EmailStatus)et.Status).ToString()} email");
        }

        public static (EmailTemplate, EmailTemplate) Send(this EmailTemplate et)
        {
            return Result.DisplayAndPersist(et.With(x => x.Status, EmailStatus.Sent));
        }

        public static IQueryable<string> AutoCompleteSubject(this EmailTemplate et, [MinLength(2)] string value)
        {
            var matchingNames = new List<string> { "Subject1", "Subject2", "Subject3" };
            return from p in matchingNames.AsQueryable() select p.Trim();
        }

    }
    public enum EmailStatus
    {
        New, Sent, Failed
    }

    public enum EmailPromotion
    {
        NoPromotions = 0,
        AdventureworksOnly = 1,
        AdventureworksAndPartners = 2
    }
}
