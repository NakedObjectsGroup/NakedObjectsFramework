using NakedFunctions;

namespace AdventureWorksModel
{
    public static class EmailAddressFunctions
    {
        public static EmailAddress Updating(EmailAddress x, IContext context) => x with { ModifiedDate = context.Now() };
    }
}
