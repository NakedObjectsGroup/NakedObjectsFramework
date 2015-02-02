// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using Common.Logging;
using NakedObjects.Surface;
using RestfulObjects.Snapshot.Utility;

namespace RestfulObjects.Mvc.Model {
    public class ReferenceValue : IValue {
        private readonly string internalValue;

        private static readonly ILog Logger = LogManager.GetLogger(typeof(ReferenceValue));

        public ReferenceValue(object value, string name) {
            internalValue = value as string;

            if (string.IsNullOrWhiteSpace(internalValue)) {
                Logger.ErrorFormat("Malformed json name: {0} arguments: href = null or empty", name);
                throw new ArgumentException("malformed arguments");
            }
        }

        #region IValue Members

        public object GetValue(INakedObjectsSurface surface, UriMtHelper helper) {
            return GetObjectByHref(internalValue, surface, helper);
        }

        #endregion

        private object GetObjectByHref(string href, INakedObjectsSurface surface, UriMtHelper helper) {
            string[] oids = helper.GetObjectId(href);
            if (oids != null) {
                var oid = new LinkObjectId(oids[0], oids[1]);
                return surface.GetObject(oid).Target.Object;
            }
            string typeName = helper.GetTypeId(href);
            return surface.GetDomainType(typeName);
        }
    }
}