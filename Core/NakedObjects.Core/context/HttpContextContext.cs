// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Web;
using NakedObjects.Core.Service;

namespace NakedObjects.Core.Context {
    /// <summary>
    ///     Basic multi-user implementation of NakedObjects that stores a set of components for each thread in use
    /// </summary>
    public class HttpContextContext : MultiUserContext {
        protected internal HttpContextContext() {}

        protected override NakedObjectsData Local {
            get {
                if (InitialisationFatalError != null) {
                    throw InitialisationFatalError;
                }

                if (!HttpContext.Current.Items.Contains("NakedObjectsData")) {
                    HttpContext.Current.Items.Add("NakedObjectsData", InitialiseNewData());
                    ObjectPersistor.UpdateNotifier = UpdateNotifier;
                }
                return HttpContext.Current.Items["NakedObjectsData"] as NakedObjectsData;
            }
        }

        private static NakedObjectsData InitialiseNewData() {
            var local = new NakedObjectsData {ObjectPersistor = PersistorInstaller.CreateObjectPersistor()};
           
            local.ObjectPersistor.AddServices(MenuServicesInstaller, ContributedActionsInstaller, SystemServicesInstaller);
            local.ObjectPersistor.Init();
            return local;
        }

        protected internal override void TerminateSession() {}

        protected internal override string[] GetAllExecutionContextIds() {
            return new string[] {};
        }

        public override void ShutdownSession() {}

        public static NakedObjectsContext CreateInstance() {
            return new HttpContextContext();
        }

        protected internal override void EnsureContextReady() {
            if (InitialisationFatalError != null) {
                throw InitialisationFatalError;
            }

            Local.ObjectPersistor.Reset();
            Local.UpdateNotifier.AllChangedObjects();
            ClearSession();
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}