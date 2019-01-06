using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NakedFunctions
{
    public enum FeedbackType
    {
        Inform,Warn,Error
    }
    public class Feedback
    {
        public Feedback(string message, FeedbackType type)
        {

        }

        //TODO: Helper to create new Feedback from existing plus new message
        public string Inform { get; set; }
        public string Warn { get; set; }
        public string Error { get; set; }
    }
    public abstract class ActionResult
    {


        //TODO: Overloaded constructors with fewer options
    }

    public class ActionResultSingle<T> : ActionResult where T : IFunctionalType
    {
        public ActionResultSingle(T result)
        {

        }

        public ActionResultSingle(T result, IEnumerable<object> toSave)
        {

        }

        public ActionResultSingle(T result, Feedback feedback)
        {

        }

        public ActionResultSingle(T result, IEnumerable<object> toSave, Feedback feedback)
        {

        }
    }

    public class ActionResultList<T> : ActionResult where T : IFunctionalType
    {
        public ActionResultList(IEnumerable<T> results)
        {

        }

        public ActionResultList(IEnumerable<T> results, IEnumerable<object> toSave)
        {

        }
        public ActionResultList(IEnumerable<T> results, Feedback feedback)
        {

        }
        public ActionResultList(IEnumerable<T> results, IEnumerable<object> toSave, Feedback feedback)
        {

        }
    }

 
}
