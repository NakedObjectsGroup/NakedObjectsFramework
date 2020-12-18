using System;

namespace NakedFunctions.ProgrammingModel.Interfaces
{
    //Defines a well-known service that may be accessed via the GetService method on IContainer.
    //NakedFunctions provides a default implementation, which may be replaced with a custom one, registered in Services Configuration.
    //The IClock service is used to allow functions needing Today or Now to be dependent only on values passed in as parameters.
    interface IClock
    {
        //Equivalent to calling System.DateTime.Today
        DateTime Today();

        //Equivalent to calling System.DateTime.Now
        DateTime Now();


    }
}
