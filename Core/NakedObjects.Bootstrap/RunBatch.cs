using System;
using NakedObjects.Architecture.Component;
using NakedObjects.Core.NakedObjectsSystem;

namespace NakedObjects.Boot {
    public class BatchClient : INakedObjectsClient {
        private readonly IBatchStartPoint batchStartPoint;

        public BatchClient(IBatchStartPoint batchStartPoint) {
            this.batchStartPoint = batchStartPoint;
        }

        protected void Inject(object obj) {
            //object[] services = NakedObjectsContext.LifecycleManager.GetServices().Select(no => no.Object).ToArray();
            //IContainerInjector injector = new DotNetDomainObjectContainerInjector(NakedObjectsContext.Reflector,
            //    services);
            //injector.InitDomainObject(obj);
        }

        protected static void StartTransaction() {
           // NakedObjectsContext.LifecycleManager.StartTransaction();
        }

        protected static void EnsureReady() {
           // NakedObjectsContext.EnsureReady();
        }

        protected void SetSession() {
          //  NakedObjectsContext.Instance.SetSession(new WindowsSession(Thread.CurrentPrincipal));
        }

        protected static void EndTransaction() {
         //   NakedObjectsContext.LifecycleManager.EndTransaction();
        }

        protected static void AbortTransaction() {
         //   NakedObjectsContext.LifecycleManager.AbortTransaction();
        }

        #region INakedObjectsClient Members

        public void StartClient(NakedObjectsSystem system) {
           // ISession session = system.AuthenticationManager.Authenticate();
          //  system.Connect(session);
            Inject(batchStartPoint);

            try {
                EnsureReady();
                SetSession();
                StartTransaction();
                batchStartPoint.Execute();
                EndTransaction();
            }
            catch (Exception) {
                // log.ErrorFormat("WorkItem: {0} threw exception {1}", wi.Id, e.Message);
                AbortTransaction();
            }
        }

        #endregion
    }

    public abstract class RunBatch : RunStandaloneBase {
        //protected override NakedObjectsContext Context {
        //    get { return ThreadContext.CreateInstance(); }
        //}

        protected override INakedObjectsClient Client {
            get { return new BatchClient(BatchStartPoint); }
        }

        private IBatchStartPoint BatchStartPoint { get; set; }

        protected override void ProductSpecificStart(NakedObjectsSystem system) {
            base.ProductSpecificStart(system);
            InitialiseLogging();
        }

        protected abstract void InitialiseLogging();

        protected void Start(IBatchStartPoint batchStartPoint) {
            if (batchStartPoint == null) {
                throw new InitialisationException("batchStartPoint may not be null");
            }

            BatchStartPoint = batchStartPoint;
            Start();
        }
    }
}