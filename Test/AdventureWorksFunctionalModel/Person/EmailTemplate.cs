using AW.Functions;
using NakedFunctions;

namespace AW.Types
{
    [ViewModel(typeof(EmailTemplate_Functions), VMEditability.EditOnly)]
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

    public enum EmailStatus
    {
        New, Sent, Failed
    }
}
