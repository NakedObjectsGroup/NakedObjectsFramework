namespace NakedFunctions
{
    //Defines a well-known service that may be accessed via the GetService method on IContainer.
    //NakedFunctions provides a default implementation, which may be replaced with a custom one, registered in Services Configuration.
    //IAlert allows messages to be displayed to the user.
    public interface IAlert
    {
        //This method has side-effects.
        void WarnUser(string message);

        //This method has side-effects.
        void InformUser(string message);
    }
}
