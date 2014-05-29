using System;
using System.Threading;
using System.Threading.Tasks;
using Common.Logging;
using NakedObjects.Core.Context;
using NakedObjects.Core.Security;


namespace NakedObjects.Async {
    /// <summary>
    ///     A service to be injected into domain code that allows multiple actions to be initiated
    ///     asynchronously -  each running within its own separate NakedObjects contexct.
    /// </summary>
    public class AsyncService : IAsyncService {
        private readonly ILog log = LogManager.GetLogger(typeof (AsyncService));

        protected static void AbortTransaction() {
            NakedObjectsContext.ObjectPersistor.AbortTransaction();
        }

        protected static void EndTransaction() {
            NakedObjectsContext.ObjectPersistor.EndTransaction();
        }

        protected static void StartTransaction() {
            NakedObjectsContext.ObjectPersistor.StartTransaction();
        }

        protected static void EnsureReady() {
            NakedObjectsContext.EnsureReady();
        }

        protected void SetSession() {
            NakedObjectsContext.Instance.SetSession(new WindowsSession(Thread.CurrentPrincipal));
        }

        protected Action WorkWrapper(Action action) {
            return () => {
                try {
                    EnsureReady();
                    SetSession();
                    StartTransaction();
                    action();
                    EndTransaction();
                }
                catch (Exception e) {
                    log.ErrorFormat("Action threw exception {0}", e.Message);
                    AbortTransaction();
                    throw;
                }
            };
        }

        protected Task TaskWrapper(Action action) {
            var task = new Task(WorkWrapper(action));
            task.Start();
            return task;
        }

        /// <summary>
        ///     Domain programmers must take care to ensure thread safety.
        ///     The action passed in must not include any references to stateful objects (e.g. persistent domain objects).
        ///     Typically the action should be on a (stateless) service; it may include primitive references
        ///     such as object Ids that can be used within the called method to retrieve and action on specific
        ///     object instances.
        /// </summary>
        /// <param name="toRun"></param>
        public  Task RunAsync(Action toRun) {
            return TaskWrapper(toRun);
        }
    }
}