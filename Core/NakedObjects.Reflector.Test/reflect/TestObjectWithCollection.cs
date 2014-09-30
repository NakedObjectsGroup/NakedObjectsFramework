// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections;

namespace NakedObjects.Reflector.DotNet.Reflect {
    public class TestObjectWithCollection {
        private readonly ArrayList arrayList;
        private readonly bool throwException;

        public TestObjectWithCollection(ArrayList arrayList, bool throwException) {
            this.arrayList = arrayList;
            this.throwException = throwException;
        }

        public object getList() {
            ThrowException();
            return arrayList;
        }

        public void addToList(object obj) {
            ThrowException();
            arrayList.Add(obj);
        }

        private void ThrowException() {
            if (throwException) {
                throw new Exception("cause invocation failure");
            }
        }

        public void removeFromList(object obj) {
            ThrowException();
            arrayList.Remove(obj);
        }

        public void clearList() {
            ThrowException();
            arrayList.Clear();
        }

        public string validateAddToList(object obj) {
            ThrowException();
            if (obj is TestObjectWithCollection) {
                return "can't Add this Type of object";
            }
            return null;
        }

        public string validateRemoveFromList(object obj) {
            ThrowException();
            if (obj is TestObjectWithCollection) {
                return "can't Remove this Type of object";
            }
            return null;
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}