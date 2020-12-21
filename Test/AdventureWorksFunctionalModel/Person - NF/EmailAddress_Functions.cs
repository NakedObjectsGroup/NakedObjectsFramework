using NakedFunctions;

namespace AdventureWorksModel
{
    public static class EmailAddressFunctions
    {
        public static EmailAddress Updating(EmailAddress x, IContainer container) => x with { ModifiedDate = container.Now() };
    }
}
