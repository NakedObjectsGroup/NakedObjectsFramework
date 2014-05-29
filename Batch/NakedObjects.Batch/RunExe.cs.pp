using NakedObjects.Async;
using NakedObjects.Boot;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.EntityObjectStore;

namespace $rootnamespace$ {
    public class RunExe : RunBatch {

        protected override IServicesInstaller MenuServices {
            get {
                return new ServicesInstaller(new AsyncService());
            }
        }

        protected override IServicesInstaller ContributedActions {
            get { return new ServicesInstaller(); }
        }

        //protected override IServicesInstaller SystemServices {
        //    get { return new ServicesInstaller(new SimpleEncryptDecrypt()); }
        // }


		protected override void InitialiseLogging() {
            // uncomment and add appropriate Common.Logging package
            // http://netcommon.sourceforge.net/docs/2.1.0/reference/html/index.html

            //var properties = new NameValueCollection();

            //properties["configType"] = "INLINE";
            //properties["configFile"] = @"C:\Naked Objects\nologfile.txt";

            //LogManager.Adapter = new Log4NetLoggerFactoryAdapter(properties);
        }

        protected override IObjectPersistorInstaller Persistor {
            get {
                // Database.DefaultConnectionFactory = new SqlCeConnectionFactory("System.Data.SqlServerCe.4.0"); //For in-memory database
                // Database.SetInitializer(new DropCreateDatabaseIfModelChanges<MyDbContext>()); //Optional behaviour for CodeFirst
                var installer = new EntityPersistorInstaller();
                // installer.AddCodeFirstDbContextConstructor(() => new MyDbContext());  //For Code First
                return installer;
            }
        }

        public static void Run() {
            new RunExe().Start(new BatchStartPoint());
        }
    }
}