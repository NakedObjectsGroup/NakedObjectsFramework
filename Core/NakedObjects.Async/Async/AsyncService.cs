using System;
using System.Threading.Tasks;
using Common.Logging;

namespace NakedObjects.Async {
    /// <summary>
    ///     A service to be injected into domain code that allows multiple actions to be initiated
    ///     asynchronously -  each running within its own separate NakedObjects contexct.
    /// </summary>
    public class AsyncService : IAsyncService {
        private readonly ILog log = LogManager.GetLogger(typeof (AsyncService));

        public INakedObjectsFramework Framework { set; protected get; }

        /// <summary>
        ///     Domain programmers must take care to ensure thread safety.
        ///     The action passed in must not include any references to stateful objects (e.g. persistent domain objects).
        ///     Typically the action should be on a (stateless) service; it may include primitive references
        ///     such as object Ids that can be used within the called method to retrieve and action on specific
        ///     object instances.
        /// </summary>
        /// <param name="toRun"></param>
        public Task RunAsync(Action toRun) {
            return TaskWrapper(toRun);
        }

        protected void AbortTransaction() {
            Framework.TransactionManager.AbortTransaction();
        }

        protected void EndTransaction() {
            Framework.TransactionManager.EndTransaction();
        }

        protected void StartTransaction() {
            Framework.TransactionManager.StartTransaction();
        }

        protected void EnsureReady() {
            // Framework.EnsureReady();
        }

        protected void SetSession() {
            // Framework.SetSession(new WindowsSession(Thread.CurrentPrincipal));
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
    }
}