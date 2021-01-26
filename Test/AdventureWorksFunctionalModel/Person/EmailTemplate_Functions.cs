using System;
using AW.Types;
using NakedFunctions;

namespace AW.Functions
{
    public static class EmailTemplate_Functions
    {
        public static string[] DeriveKeys(this EmailTemplate em) => new[] {
                em.To,
                em.From,
                em.Subject,
                em.Message,
                em.Status.ToString() };

        public static EmailTemplate CreateFromKeys(string[] keys) =>
            new EmailTemplate {
                To = keys[0],
                From = keys[1], 
                Subject = keys[2], 
                Message = keys[3], 
                Status = (EmailStatus)Enum.Parse(typeof(EmailStatus), keys[4]) };
                
       public static (EmailTemplate, IContext) Send(this EmailTemplate et, IContext context) =>
            (et with { Status = EmailStatus.Sent }, context.WithInformUser("Email sent"));
    }
}
